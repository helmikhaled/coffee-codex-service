# Plan - 007 Random Recipe API

This spec adds the random recipe discovery endpoint to the existing public recipes surface.

The implementation should expose `GET /recipes/random`, return a minimal payload containing a valid recipe identifier, and keep the feature lightweight, testable, and aligned with the current Clean Architecture slice pattern.

---

# Scope

In scope

- public `GET /recipes/random` endpoint
- random selection from existing recipes
- minimal response contract for frontend navigation (`id`)
- explicit behavior when no recipes exist
- automated coverage across Application, Infrastructure, and API layers

Out of scope

- recommendation or ranking logic
- popularity weighting
- personalization
- adding a new discovery endpoint family outside `/recipes`
- advanced randomization optimization for very large datasets

---

# Planning Notes

- `docs/vision.md` and `docs/architecture.md` already define `GET /recipes/random` as part of MVP discovery; this feature should extend the current recipes controller, not introduce a new controller.
- Existing slices (`GetRecipes`, `GetRecipeDetail`) use handler + reader abstractions and DI wiring in `Application` and `Infrastructure`. This feature should follow the same structure for consistency.
- Existing detail navigation is ID-based (`GET /recipes/{id}`), so returning `{ "id": "<guid>" }` is sufficient for frontend follow-up calls.
- PRD asks for an "appropriate error" when no recipes exist. For consistency with current API patterns, return `404 Not Found` when no random candidate can be selected.
- No schema changes are required for MVP random selection.

---

# Planned File Touchpoints

API

- `src/CoffeeCodex.API/Recipes/RecipesController.cs`

Application

- `src/CoffeeCodex.Application/Recipes/Queries/GetRandomRecipe/GetRandomRecipeQuery.cs`
- `src/CoffeeCodex.Application/Recipes/Queries/GetRandomRecipe/RandomRecipeDto.cs`
- `src/CoffeeCodex.Application/Recipes/Queries/GetRandomRecipe/IGetRandomRecipeHandler.cs`
- `src/CoffeeCodex.Application/Recipes/Queries/GetRandomRecipe/GetRandomRecipeHandler.cs`
- `src/CoffeeCodex.Application/Recipes/Queries/GetRandomRecipe/IRecipeRandomReader.cs`
- `src/CoffeeCodex.Application/DependencyInjection.cs`

Infrastructure

- `src/CoffeeCodex.Infrastructure/Recipes/RecipeRandomReader.cs`
- `src/CoffeeCodex.Infrastructure/DependencyInjection.cs`

Tests

- `tests/CoffeeCodex.RecipeListing.Tests/Application/GetRandomRecipeHandlerTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/Infrastructure/RecipeRandomReaderTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs` (extend with dedicated no-data factory/fixture)

---

# Phase 1 - Contract And Behavior Definition

Define endpoint contract and empty-data behavior before implementation.

Work

- Confirm endpoint route: `GET /recipes/random`.
- Define success response contract as:
  - `200 OK`
  - payload: `{ "id": "guid" }`
- Define no-data behavior:
  - `404 Not Found` when no recipes exist.
- Keep endpoint public (no Auth0 requirement), consistent with other public recipe discovery APIs.
- Add OpenAPI metadata annotations for `200` and `404`.

Outcome

- Random endpoint behavior is explicit and stable for frontend integration.

---

# Phase 2 - Application Slice Setup

Add an explicit query slice for random selection without leaking persistence concerns.

Work

- Add `GetRandomRecipeQuery` as a dedicated use-case request model (empty marker query for now).
- Add `RandomRecipeDto` in Application with the minimal `Id` field.
- Add application contracts:
  - `IGetRandomRecipeHandler`
  - `IRecipeRandomReader`
- Implement `GetRandomRecipeHandler`:
  - delegates to reader abstraction
  - returns `RandomRecipeDto?` to allow no-data mapping at API layer
- Register new handler in `AddApplicationServices()`.

Outcome

- Application layer owns the random-recipe use case and response contract.

---

# Phase 3 - Infrastructure Random Selection Strategy

Implement random selection in Infrastructure with predictable SQL translation and no schema changes.

Work

- Implement `RecipeRandomReader` using `CoffeeCodexDbContext`.
- Use an efficient two-step strategy for MVP:
  1. `CountAsync` total recipes.
  2. If zero, return `null`.
  3. Generate random offset in `[0, totalCount - 1]`.
  4. Select one recipe ID using deterministic ordering (`id`) with `Skip(offset).Take(1)`.
- Project directly to `RandomRecipeDto` (avoid loading full recipe graphs).
- Keep query `AsNoTracking()`.
- Register reader in `AddInfrastructureServices()`.
- Handle edge case where concurrent data change causes offset query miss by returning `null` (API maps to `404`).

Outcome

- Infrastructure provides a lightweight random ID read path compatible with existing stack and tests.

---

# Phase 4 - API Endpoint Integration

Expose random query use case through `RecipesController`.

Work

- Inject `IGetRandomRecipeHandler` into `RecipesController`.
- Add `GET /recipes/random` action.
- Call handler with `GetRandomRecipeQuery`.
- Map responses:
  - `null` -> `NotFound()`
  - value -> `Ok(RandomRecipeDto)`
- Keep current controller error handling style consistent with existing actions.

Outcome

- Frontend can fetch a random recipe ID through the public recipes API surface.

---

# Phase 5 - Testing And Verification

Add coverage for random behavior and regressions.

Work

- Application tests (`GetRandomRecipeHandlerTest`):
  - returns DTO when reader returns a candidate
  - returns null when reader has no candidate
- Infrastructure tests (`RecipeRandomReaderTest`):
  - returns one valid seeded recipe ID when recipes exist
  - returns null when database is empty
  - does not require deterministic exact ID assertion (assert membership in seeded ID set)
- API tests (`RecipesEndpointTest`):
  - `GET /recipes/random` returns `200` and non-empty `id`
  - returned id can be used with `GET /recipes/{id}` and resolves successfully
  - empty-dataset scenario returns `404` (use dedicated test host/database setup)
- Regression check:
  - existing `/recipes` and `/recipes/{id}` tests remain unchanged and passing.

Outcome

- Random endpoint behavior is protected without introducing flaky test expectations.

---

# Phase 6 - Performance And Observability Checks

Confirm random endpoint aligns with MVP non-functional expectations.

Work

- Verify random endpoint participates in existing OpenTelemetry ASP.NET request tracing.
- Ensure query path remains projection-first and avoids loading related entities.
- Validate endpoint response time against PRD target on representative local data.
- Capture any scalability note for future optimization if dataset growth makes offset strategy insufficient.

Outcome

- Endpoint is observable, lightweight, and suitable for MVP scale.

---

# Suggested Implementation Order

1. Add Application contracts, query model, DTO, and handler for random recipe retrieval.
2. Implement Infrastructure random reader and DI registration.
3. Add `GET /recipes/random` controller action and response metadata.
4. Add Application/Infrastructure/API tests for success and no-data behavior.
5. Run full existing test suite for regression confidence.

---

# Completion Criteria

The feature is complete when:

- `GET /recipes/random` is publicly available
- successful calls return `{ "id": "<guid>" }`
- no-recipe state returns `404 Not Found`
- returned IDs correspond to existing recipes
- random selection logic is implemented in Infrastructure behind an Application abstraction
- automated tests cover success and empty-data scenarios without flaky randomness assumptions

---

# Follow-On Dependencies

This feature should leave the codebase ready for:

- frontend "Surprise Me" navigation flow (`random -> detail`)
- optional future response expansion (`slug`) without endpoint redesign
- future large-dataset randomization strategy replacement within `IRecipeRandomReader` without API contract changes
