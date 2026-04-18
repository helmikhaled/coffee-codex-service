# Tasks - 008 Recipe View Tracking API

Tasks must be executed sequentially.

---

# Task 1 - Finalize View Tracking Contract [ ]

Lock the API behavior before implementation.

Define:

- endpoint: `POST /recipes/{id}/view`
- success response: `204 No Content`
- missing-recipe response: `404 Not Found`
- invalid empty-id response: `400 Bad Request`
- endpoint accessibility: public
- semantics: non-idempotent, one increment per successful call
- MVP decision: direct per-request increment, not batched aggregation

Verify:

- contract matches `docs/specs/008-recipe-views/prd.md`
- contract remains aligned with `docs/vision.md` and `docs/architecture.md`

---

# Task 2 - Add Application Command Model [ ]

Create the write-side request model for the feature.

Add:

- `RecordRecipeViewCommand`

Suggested location:

- `src/CoffeeCodex.Application/Recipes/Commands/RecordRecipeView`

Verify:

- command carries only the recipe identifier and no persistence types

---

# Task 3 - Add Command Validation [ ]

Protect the command contract with lightweight validation.

Add:

- `RecordRecipeViewCommandValidator`

Rules:

- recipe id must not be `Guid.Empty`

Verify:

- invalid identifiers produce validation failures suitable for `400 Bad Request`

---

# Task 4 - Add Application Handler And Recorder Contracts [ ]

Define the Application-side abstractions for the write path.

Add:

- `IRecordRecipeViewHandler`
- `IRecipeViewRecorder`

Behavior:

- handler returns a lightweight success/not-found outcome
- recorder hides EF Core and database update details

Verify:

- contracts keep controller and Infrastructure concerns separated

---

# Task 5 - Implement Application Handler [ ]

Implement the recipe-view use case orchestration.

Add:

- `RecordRecipeViewHandler`

Behavior:

- validate the command
- delegate increment work to `IRecipeViewRecorder`
- propagate success/not-found result without HTTP coupling

Verify:

- handler contains orchestration only and no direct EF/database code

---

# Task 6 - Register Application Services [ ]

Wire the new command slice into DI.

Update:

- `src/CoffeeCodex.Application/DependencyInjection.cs`

Register:

- `IValidator<RecordRecipeViewCommand>` -> `RecordRecipeViewCommandValidator`
- `IRecordRecipeViewHandler` -> `RecordRecipeViewHandler`

Verify:

- Application service provider can resolve the new validator and handler

---

# Task 7 - Implement Infrastructure View Recorder [ ]

Add the database update path for view tracking.

Add:

- `src/CoffeeCodex.Infrastructure/Recipes/RecipeViewRecorder.cs`

Implementation requirements:

- target exactly one recipe by `id`
- increment `brew_count` atomically
- return not-found outcome when no row is updated
- keep `updated_at` unchanged
- avoid loading related entities or full recipe aggregates
- contain any provider-specific fallback needed for in-memory tests inside this class

Verify:

- the write path is narrow and matches the PRD performance intent

---

# Task 8 - Register Infrastructure Recorder [ ]

Wire the Infrastructure implementation into DI.

Update:

- `src/CoffeeCodex.Infrastructure/DependencyInjection.cs`

Register:

- `IRecipeViewRecorder` -> `RecipeViewRecorder`

Verify:

- Infrastructure service provider can resolve the recorder implementation

---

# Task 9 - Add POST /recipes/{id}/view Controller Action [ ]

Expose the new use case through the existing recipes controller.

Update:

- `src/CoffeeCodex.API/Recipes/RecipesController.cs`

Add:

- `HttpPost("{id:guid}/view")` action
- handler call to `IRecordRecipeViewHandler`
- response mapping:
  - `NoContent()` on success
  - `NotFound()` when recipe is missing
  - validation-problem response for invalid command input

Verify:

- the new action fits the current controller routing and error-handling style

---

# Task 10 - Add Endpoint Metadata [ ]

Document the new API surface in controller metadata.

Update:

- `src/CoffeeCodex.API/Recipes/RecipesController.cs`

Add or verify:

- `ProducesResponseType` for `204`
- `ProducesResponseType` for `400`
- `ProducesResponseType` for `404`

Verify:

- OpenAPI metadata reflects the command-only endpoint contract

---

# Task 11 - Add Application Handler Tests [ ]

Protect the Application-layer behavior for the new command slice.

Add:

- `tests/CoffeeCodex.RecipeListing.Tests/Application/RecordRecipeViewHandlerTest.cs`

Cover:

- handler returns success when recorder reports an update
- handler returns not-found when recorder reports no update
- empty recipe id throws validation exception

Verify:

- tests exercise orchestration behavior without database dependencies

---

# Task 12 - Add Infrastructure Recorder Tests [ ]

Protect the persistence behavior for brew-count increments.

Add:

- `tests/CoffeeCodex.RecipeListing.Tests/Infrastructure/RecipeViewRecorderTest.cs`

Cover:

- existing recipe increments by exactly one
- missing recipe returns not-found outcome
- two successful calls increment the stored count twice

Verify:

- tests confirm both update correctness and cumulative behavior

---

# Task 13 - Add API Integration Test For Success Path [ ]

Protect the endpoint-level success behavior.

Update:

- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`

Cover:

- `POST /recipes/{id}/view` returns `204`

Verify:

- end-to-end command path succeeds against seeded test data

---

# Task 14 - Add API Integration Test For Missing Recipe [ ]

Protect the endpoint-level not-found behavior.

Update:

- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`

Cover:

- `POST /recipes/{unknownId}/view` returns `404`

Verify:

- missing recipes are handled without side effects

---

# Task 15 - Add API Read-After-Write Regression Tests [ ]

Verify the integration requirement that updated counts appear in existing recipe reads.

Update:

- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`

Cover:

- `POST /recipes/{id}/view` followed by `GET /recipes/{id}` shows incremented `brewCount`
- `POST /recipes/{id}/view` followed by `GET /recipes` shows incremented summary `brewCount`
- repeated view calls produce cumulative increments visible through reads

Verify:

- list/detail endpoints reflect the stored increment without DTO contract changes

---

# Task 16 - Add Validation Regression Test For Empty Guid [ ]

Protect the invalid-input edge case on the new endpoint.

Update:

- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`

Cover:

- `POST /recipes/00000000-0000-0000-0000-000000000000/view` returns `400`

Verify:

- controller validation behavior remains consistent with existing query endpoints

---

# Task 17 - Run Automated Verification [ ]

Run the repository tests that cover the modified slice.

Execute:

- `dotnet test tests/CoffeeCodex.RecipeListing.Tests/CoffeeCodex.RecipeListing.Tests.csproj`

Verify:

- new Application, Infrastructure, and API tests pass
- existing recipe listing/detail/random tests remain green

---

# Task 18 - Run Runtime, Performance, And Observability Checks [ ]

Perform final non-functional verification for the feature.

Validate:

- representative `POST /recipes/{id}/view` latency is aligned with the `< 50ms` target
- update path stays narrow and avoids unnecessary entity loading
- endpoint is captured by existing OpenTelemetry request tracing
- no batching infrastructure was introduced into this MVP slice

Verify:

- the feature is ready for handoff to frontend integration

---

# Completion

Recipe view tracking is complete when:

- public `POST /recipes/{id}/view` is available
- each successful call increments the stored `brew_count`
- missing recipes return `404 Not Found`
- invalid empty identifiers return `400 Bad Request`
- existing list/detail endpoints surface the incremented count
- update logic remains isolated in Application and Infrastructure
- automated and non-functional checks are complete
