# Tasks - 003 Recipe Detail API

Tasks must be executed sequentially.

---

# Task 1 - Finalize Recipe Detail Contract [ ]

Lock the endpoint and payload contract before implementation.

Define:

- `GET /recipes/{id}` with `id` as `Guid`
- `RecipeDetailDto` fields:
  - `id`
  - `slug`
  - `title`
  - `description`
  - `category`
  - `brewCount`
  - `author`
  - `brewSpecs`
  - `ingredients`
  - `steps`
  - `images`
  - `tags`
- nested DTO shapes:
  - `AuthorDto`
  - `BrewSpecsDto`
  - `IngredientDto`
  - `StepDto`
  - `ImageDto`

Verify:

- required fields from `prd.md` and `architecture.md` are represented
- ordering rules are explicit for ingredients, steps, and images

---

# Task 2 - Add Missing Domain Entities [ ]

Create domain models needed by recipe detail.

Add:

- `RecipeIngredient`
- `RecipeStep`
- `Tag`
- `RecipeTag` (if explicit join entity is used)

Verify:

- entities represent all detail relations required by the contract

---

# Task 3 - Extend Existing Recipe Domain Relationships [ ]

Update current domain entities for detail navigation.

Update:

- `Recipe` with ingredient, step, and tag navigation collections
- `Author` with `AvatarUrl` if included by `AuthorDto`
- any required value/nullability alignment for detail fields

Verify:

- `Recipe` aggregate can model full detail payload

---

# Task 4 - Extend DbContext With Detail DbSets [ ]

Add detail tables to persistence surface.

Update `CoffeeCodexDbContext` with:

- `RecipeIngredients`
- `RecipeSteps`
- `Tags`
- `RecipeTags` (if explicit join entity is used)

Verify:

- DbContext builds and includes all required sets

---

# Task 5 - Configure EF Core Mappings For Detail Tables [ ]

Map new entities to architecture-defined schema.

Add configurations for:

- `recipe_ingredients`
- `recipe_steps`
- `tags`
- `recipe_tags`

Include:

- key definitions
- relationships
- column names
- ordering-related columns (`position`, `step_number`)

Verify:

- model snapshot/configuration supports all required joins

---

# Task 6 - Add Detail Query Indexes And Schema Support [ ]

Add indexes needed for performant detail reads.

Add:

- index on `recipe_ingredients(recipe_id, position)`
- index on `recipe_steps(recipe_id, step_number)`
- index on `recipe_tags(recipe_id, tag_id)`
- verify `recipe_images(recipe_id, position)` remains available

Verify:

- schema can be created with new indexes

---

# Task 7 - Expand Seed Data For Recipe Detail [ ]

Seed representative data for detail scenarios.

Add seed coverage for:

- ingredients per recipe
- ordered steps per recipe
- tags and recipe-tag relations
- recipe with no tags
- recipe with nullable brew-spec coffee fields

Verify:

- seeded dataset supports happy path and edge case tests

---

# Task 8 - Add Application Query Contracts [ ]

Create the recipe detail query slice in Application layer.

Add:

- `GetRecipeDetailQuery`
- `IGetRecipeDetailHandler`
- `IRecipeDetailReader`
- `RecipeDetailDto` and nested DTOs (if housed in Application)

Suggested location:

- `src/CoffeeCodex.Application/Recipes/Queries/GetRecipeDetail`

Verify:

- API can call a single application use case for detail retrieval

---

# Task 9 - Add Query Validation And Handler [ ]

Implement query execution flow in Application layer.

Add:

- validator for `GetRecipeDetailQuery` (`id` must be non-empty GUID)
- `GetRecipeDetailHandler` using validator + reader abstraction

Define behavior:

- return detail result when found
- return null/typed not-found result when missing

Verify:

- invalid ids fail validation
- handler contract supports not-found mapping

---

# Task 10 - Register Application And Infrastructure DI [ ]

Wire all new detail services.

Update:

- `Application.DependencyInjection` registrations:
  - validator
  - handler
- `Infrastructure.DependencyInjection` registrations:
  - detail reader implementation

Verify:

- service provider resolves detail handler and dependencies

---

# Task 11 - Implement Infrastructure Detail Reader [ ]

Build the EF Core read model projection for recipe detail.

Implement:

- query by recipe `id`
- `AsNoTracking()`
- projection to `RecipeDetailDto`
- ordered child collections:
  - ingredients by `position`
  - steps by `stepNumber`
  - images by `position`
- include author, brew specs, and tags

Verify:

- query avoids N+1 behavior
- missing recipe returns null

---

# Task 12 - Add GET /recipes/{id} API Action [ ]

Expose detail query from the API layer.

Update `RecipesController`:

- add `HttpGet("{id:guid}")` action
- delegate to `IGetRecipeDetailHandler`
- return `200` with `RecipeDetailDto`
- return `404` when not found
- keep validation behavior consistent with existing API pattern

Verify:

- route is public and reachable
- OpenAPI metadata includes `200`, `400`, and `404`

---

# Task 13 - Add Application Tests [ ]

Add tests for query validation and handler behavior.

Cover:

- empty GUID validation failure
- valid GUID passes validation
- handler returns expected result for found recipe
- handler not-found behavior

Verify:

- all application tests pass

---

# Task 14 - Add Infrastructure Reader Tests [ ]

Add persistence tests for detail projection correctness.

Cover:

- complete detail response mapping
- ingredient ordering by position
- step ordering by step number
- image ordering by position
- tag mapping
- nullable brew-spec field handling
- not-found returns null

Verify:

- all infrastructure tests pass

---

# Task 15 - Add API Integration Tests [ ]

Add endpoint-level tests for contract and status codes.

Cover:

- `GET /recipes/{id}` returns `200` and expected payload shape
- unknown `id` returns `404`
- invalid route id behavior is consistent with ASP.NET routing/validation

Verify:

- API integration tests pass

---

# Task 16 - Run End-To-End Verification [ ]

Perform final validation for readiness.

Run:

- targeted detail test project(s)
- API smoke test against running app with seeded data
- manual request for existing and missing recipe ids

Confirm:

- feature meets PRD goals
- response shape matches `RecipeDetailDto`
- performance/query behavior is acceptable for MVP target

---

# Completion

Recipe detail is ready when:

- public `GET /recipes/{id}` is working
- full detail payload is returned in one response
- missing recipe returns `404`
- collection ordering is deterministic
- automated tests protect core behavior
