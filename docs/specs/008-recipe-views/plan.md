# Plan - 008 Recipe View Tracking API

This spec adds the missing write-side brew tracking flow to the existing public recipes surface.

The implementation should expose `POST /recipes/{id}/view`, increment the existing `recipes.brew_count` efficiently, keep the endpoint public and lightweight, and rely on the current recipe list/detail reads so updated counts appear everywhere the frontend already consumes them.

---

# Scope

In scope

- public `POST /recipes/{id}/view`
- increment stored `brew_count` for an existing recipe
- return `404 Not Found` when the recipe does not exist
- keep existing `brewCount` fields in recipe listing and detail responses as the read source of truth
- automated coverage across Application, Infrastructure, and API layers

Out of scope

- user-specific or session-specific deduplication
- analytics event streams, dashboards, or reporting
- popularity ranking and leaderboard features
- authentication changes
- queue-based or batched write aggregation for MVP

---

# Planning Notes

- `BrewCount` already exists end-to-end in the current stack:
  - `Recipe.BrewCount` exists in Domain
  - `RecipeConfiguration` maps `brew_count`
  - existing migrations already create the column
  - `RecipeSummaryDto` and `RecipeDetailDto` already expose the value
- This feature is therefore a missing command path, not a schema-design project.
- Current recipe features use Application handlers plus Infrastructure readers. For consistency, this feature should add a command slice in Application and keep EF update logic behind an Infrastructure abstraction rather than placing database code in `RecipesController`.
- The PRD contains a conflict:
  - sections F/G/H describe a lightweight per-request increment endpoint
  - section N says view increments should be batched
- Because the current architecture has no queue or background aggregation pipeline, the MVP plan assumes direct per-request atomic increments and treats batching as a future optimization.
- Success response should be `204 No Content` to keep the write path minimal. The frontend can read the updated `brewCount` from existing `GET /recipes` and `GET /recipes/{id}` responses.
- Recording a view should not change `updated_at`; the action tracks engagement, not recipe content edits.
- No database migration is planned. `brew_count` already exists, and updates target a single recipe row by primary key. Only add a migration if implementation uncovers schema drift.
- Prefer a single-statement increment strategy in Infrastructure (for example `ExecuteUpdateAsync`) so PostgreSQL can update `brew_count = brew_count + 1` without loading full recipe graphs. Keep that optimization behind an abstraction so test-provider compatibility can be handled without changing API/Application code.

---

# Planned File Touchpoints

API

- `src/CoffeeCodex.API/Recipes/RecipesController.cs`

Application

- `src/CoffeeCodex.Application/Recipes/Commands/RecordRecipeView/RecordRecipeViewCommand.cs`
- `src/CoffeeCodex.Application/Recipes/Commands/RecordRecipeView/RecordRecipeViewCommandValidator.cs`
- `src/CoffeeCodex.Application/Recipes/Commands/RecordRecipeView/IRecordRecipeViewHandler.cs`
- `src/CoffeeCodex.Application/Recipes/Commands/RecordRecipeView/RecordRecipeViewHandler.cs`
- `src/CoffeeCodex.Application/Recipes/Commands/RecordRecipeView/IRecipeViewRecorder.cs`
- `src/CoffeeCodex.Application/DependencyInjection.cs`

Infrastructure

- `src/CoffeeCodex.Infrastructure/Recipes/RecipeViewRecorder.cs`
- `src/CoffeeCodex.Infrastructure/DependencyInjection.cs`

Tests

- `tests/CoffeeCodex.RecipeListing.Tests/Application/RecordRecipeViewHandlerTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/Infrastructure/RecipeViewRecorderTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/RecipeListingTestData.cs` (only if additional seed assertions are needed)

Schema

- no schema files are expected to change unless implementation discovers drift between the model and migrations

---

# Phase 1 - Endpoint Contract And Command Semantics

Define the API contract and command behavior before wiring code.

Work

- confirm route: `POST /recipes/{id}/view`
- define success response:
  - `204 No Content`
  - no response body
- define failure responses:
  - `404 Not Found` when the recipe does not exist
  - `400 Bad Request` when command validation fails (for example `Guid.Empty`)
- keep endpoint public, consistent with other public recipe discovery APIs
- keep operation intentionally non-idempotent; repeated calls may increment repeatedly
- define existing listing/detail endpoints as the source for reading the updated count

Outcome

- endpoint behavior is explicit for controller code, tests, and frontend integration

---

# Phase 2 - Application Command Slice Setup

Add a dedicated write-side use case without leaking persistence concerns.

Work

- add `RecordRecipeViewCommand` with recipe identifier input
- add `RecordRecipeViewCommandValidator` to reject invalid identifiers
- add Application contracts:
  - `IRecordRecipeViewHandler`
  - `IRecipeViewRecorder`
- implement `RecordRecipeViewHandler` so it:
  - validates the command
  - delegates persistence work to `IRecipeViewRecorder`
  - returns a lightweight success/not-found outcome suitable for API mapping
- register validator and handler in `AddApplicationServices()`

Outcome

- Application owns the recipe-view use case and keeps controllers free of update logic

---

# Phase 3 - Infrastructure Atomic Increment Path

Implement the write operation with efficient database behavior.

Work

- implement `RecipeViewRecorder` using `CoffeeCodexDbContext`
- update exactly one recipe row by `id`
- increment `brew_count` atomically (`brew_count = brew_count + 1`)
- detect missing recipes from the affected-row result instead of issuing a separate existence query when possible
- keep the update path narrow:
  - no related entity loading
  - no list/detail projection reuse
  - no unnecessary `SaveChanges` on large tracked graphs
- keep `updated_at` unchanged for view events
- if the chosen EF update strategy has provider limitations in in-memory tests, contain the fallback inside `RecipeViewRecorder` so the API/Application contract stays unchanged
- register Infrastructure implementation in `AddInfrastructureServices()`

Outcome

- the backend gets a fast, isolated write path aligned with the PRD latency goal

---

# Phase 4 - API Endpoint Integration

Expose the new command slice through the existing recipes controller.

Work

- inject `IRecordRecipeViewHandler` into `RecipesController`
- add `POST /recipes/{id:guid}/view` action
- map command outcomes to HTTP responses:
  - recorded -> `NoContent()`
  - missing recipe -> `NotFound()`
  - validation error -> `ValidationProblem(...)`
- add response metadata for `204`, `400`, and `404`
- keep routing and error handling style consistent with existing recipe actions

Outcome

- frontend clients can record a recipe view through the public recipes API surface

---

# Phase 5 - Read-Side Integration And Regression Protection

Ensure the new write path is visible through existing read endpoints without unnecessary DTO churn.

Work

- leave `RecipeSummaryDto` and `RecipeDetailDto` contracts unchanged unless implementation proves a gap
- verify current readers continue projecting stored `BrewCount`
- add end-to-end regression coverage showing that after `POST /recipes/{id}/view`:
  - `GET /recipes/{id}` returns the incremented `brewCount`
  - `GET /recipes` returns the incremented `brewCount` for that recipe summary
- confirm repeated view calls accumulate correctly across read-after-write scenarios

Outcome

- PRD integration requirement is satisfied through the current read models

---

# Phase 6 - Testing And Verification

Add focused coverage across layers and protect against regressions.

Work

- Application tests:
  - valid command returns success when recorder reports update
  - missing recipe result is propagated correctly
  - empty identifier fails validation
- Infrastructure tests:
  - existing recipe increments by exactly one call
  - missing recipe returns not-found outcome
  - repeated calls increment cumulatively
- API integration tests:
  - `POST /recipes/{id}/view` returns `204`
  - `POST /recipes/{id}/view` returns `404` for unknown recipe
  - `POST /recipes/{id}/view` followed by `GET /recipes/{id}` shows incremented `brewCount`
  - `POST /recipes/{id}/view` followed by `GET /recipes` shows incremented summary `brewCount`
  - invalid empty-guid input returns `400`
- regression check:
  - existing `/recipes`, `/recipes/{id}`, and `/recipes/random` behavior remains unchanged apart from the stored count increasing after view calls

Outcome

- the feature is protected end-to-end and current recipe discovery behavior stays stable

---

# Phase 7 - Performance And Observability Checks

Confirm the implementation meets the non-functional expectations in the PRD.

Work

- inspect the SQL/update shape to confirm the write path stays narrow
- validate representative local latency against the `< 50ms` target for the update path
- confirm the endpoint is captured by existing OpenTelemetry ASP.NET request tracing
- document batching as a future optimization note rather than introducing new infrastructure in this spec

Outcome

- the view-tracking endpoint is ready for MVP use without expanding architectural scope

---

# Suggested Implementation Order

1. Lock the endpoint contract and non-batching MVP semantics.
2. Add the Application command, validator, handler, and DI wiring.
3. Implement the Infrastructure atomic increment path and DI registration.
4. Add the `POST /recipes/{id}/view` controller action and metadata.
5. Add Application, Infrastructure, and API tests.
6. Verify read-after-write behavior through list/detail endpoints.
7. Perform performance and observability checks.

---

# Completion Criteria

The feature is complete when:

- public `POST /recipes/{id}/view` is available
- successful calls increment the stored `brew_count`
- unknown recipe IDs return `404 Not Found`
- invalid empty identifiers return `400 Bad Request`
- existing list and detail endpoints show the updated `brewCount`
- update logic is isolated behind Application and Infrastructure abstractions
- automated tests protect success, not-found, validation, and read-after-write scenarios
- local verification confirms the write path remains lightweight and aligned with the PRD target

---

# Follow-On Dependencies

This feature should leave the codebase ready for:

- frontend recipe-detail pages that fire a view event on open
- future batching or asynchronous aggregation if write volume grows
- future popularity-based discovery features that build on the stored `brew_count`
