# Tasks - 007 Random Recipe API

Tasks must be executed sequentially.

---

# Task 1 - Finalize Random Endpoint Contract [ ]

Lock the API behavior before implementation.

Define:

- endpoint: `GET /recipes/random`
- success response: `200 OK` with `{ "id": "<guid>" }`
- empty-data response: `404 Not Found`
- endpoint accessibility: public (no admin authentication)

Verify:

- contract matches `docs/specs/007-random-recipe/prd.md`
- contract remains aligned with `docs/vision.md` and `docs/architecture.md`

---

# Task 2 - Add Application Query And DTO Contracts [ ]

Create the random recipe query slice contracts in Application.

Add:

- `GetRandomRecipeQuery`
- `RandomRecipeDto` (minimal `Id` field)
- `IGetRandomRecipeHandler`
- `IRecipeRandomReader`

Suggested location:

- `src/CoffeeCodex.Application/Recipes/Queries/GetRandomRecipe`

Verify:

- contracts do not leak EF/persistence types

---

# Task 3 - Implement Application Handler [ ]

Implement the random-recipe use case orchestration.

Add:

- `GetRandomRecipeHandler` implementing `IGetRandomRecipeHandler`

Behavior:

- delegate to `IRecipeRandomReader`
- return `RandomRecipeDto?` (nullable for no-data case)

Verify:

- handler returns the reader result without API/persistence coupling

---

# Task 4 - Register Application Services [ ]

Wire random-recipe handler contracts into DI.

Update:

- `src/CoffeeCodex.Application/DependencyInjection.cs`

Register:

- `IGetRandomRecipeHandler` -> `GetRandomRecipeHandler`

Verify:

- application service provider resolves `IGetRandomRecipeHandler`

---

# Task 5 - Implement Infrastructure Random Reader [ ]

Implement random selection logic in Infrastructure.

Add:

- `src/CoffeeCodex.Infrastructure/Recipes/RecipeRandomReader.cs`

Implementation steps:

1. query total recipe count with `CountAsync`
2. return `null` if count is `0`
3. generate random offset in `[0, count - 1]`
4. select one recipe id using `OrderBy(r => r.Id).Skip(offset).Take(1)`
5. project to `RandomRecipeDto`

Verify:

- query uses `AsNoTracking()`
- no full recipe graph is materialized

---

# Task 6 - Register Infrastructure Reader [ ]

Wire random reader implementation into DI.

Update:

- `src/CoffeeCodex.Infrastructure/DependencyInjection.cs`

Register:

- `IRecipeRandomReader` -> `RecipeRandomReader`

Verify:

- infrastructure service provider resolves `IRecipeRandomReader`

---

# Task 7 - Add GET /recipes/random Controller Action [ ]

Expose random recipe use case via API.

Update:

- `src/CoffeeCodex.API/Recipes/RecipesController.cs`

Add:

- `HttpGet("random")` action
- action call to `IGetRandomRecipeHandler`
- response mapping:
  - `Ok(RandomRecipeDto)` when found
  - `NotFound()` when not found
- response metadata for `200` and `404`

Verify:

- existing `/recipes` and `/recipes/{id}` actions remain unchanged in behavior

---

# Task 8 - Add Application Handler Tests [ ]

Add unit tests for application-layer random flow.

Add:

- `tests/CoffeeCodex.RecipeListing.Tests/Application/GetRandomRecipeHandlerTest.cs`

Cover:

- handler returns DTO when reader returns candidate
- handler returns `null` when reader returns no candidate

Verify:

- tests assert handler behavior without DB dependencies

---

# Task 9 - Add Infrastructure Reader Tests [ ]

Add persistence-focused tests for random reader behavior.

Add:

- `tests/CoffeeCodex.RecipeListing.Tests/Infrastructure/RecipeRandomReaderTest.cs`

Cover:

- returns a recipe id when seeded data exists
- returned id is one of seeded recipe IDs
- returns `null` when no recipes exist

Verify:

- tests avoid brittle assertions on exact random output

---

# Task 10 - Add API Integration Test For Success Path [ ]

Add endpoint-level test for successful random retrieval.

Update:

- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`

Cover:

- `GET /recipes/random` returns `200`
- payload contains non-empty `id`
- returned id can be retrieved via `GET /recipes/{id}`

Verify:

- end-to-end flow supports frontend "Surprise Me" navigation

---

# Task 11 - Add API Integration Test For Empty Dataset [ ]

Add endpoint-level test for no-data behavior.

Update:

- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`
- API test factory setup (dedicated empty database fixture/factory)

Cover:

- `GET /recipes/random` returns `404` when no recipes exist

Verify:

- test isolates empty-dataset setup from seeded default API tests

---

# Task 12 - Run Automated Verification [ ]

Run repository tests covering the changed slice.

Execute:

- `dotnet test tests/CoffeeCodex.RecipeListing.Tests/CoffeeCodex.RecipeListing.Tests.csproj`

Verify:

- random recipe tests pass
- no regressions in listing/detail/filter/search tests

---

# Task 13 - Run Runtime Smoke And Non-Functional Checks [ ]

Validate runtime behavior against PRD non-functional expectations.

Validate:

- `GET /recipes/random` latency is acceptable for MVP small dataset target
- endpoint is captured by existing OpenTelemetry request tracing
- response payload remains minimal (`id` only)

Verify:

- behavior aligns with PRD reliability and performance goals

---

# Completion

Random recipe API is complete when:

- public `GET /recipes/random` endpoint is available
- success response returns a valid recipe `id`
- no-data state returns `404 Not Found`
- random selection logic is isolated in Infrastructure behind Application contracts
- automated tests protect success and empty-data paths
