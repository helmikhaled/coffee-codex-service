# Plan - 003 Recipe Detail API

This spec delivers the public recipe detail vertical slice for the backend.

The implementation should expose `GET /recipes/{id}`, return `RecipeDetailDto`, include all related recipe data needed by the frontend detail page, and return `404` when the recipe does not exist.

---

# Scope

In scope

- public `GET /recipes/{id}` endpoint
- full recipe detail payload in one response
- author metadata in detail response
- brew specs in detail response
- ordered ingredients response
- ordered steps response
- ordered images response
- tags response
- `404 Not Found` behavior for missing recipe

Out of scope

- recipe editing
- admin APIs
- view tracking side effects
- caching strategy

---

# Planning Notes

- `docs/architecture.md` defines detail DTO shape and relational model, but the current implemented domain is still listing-focused and does not yet include ingredients, steps, or tags.
- The current API style uses controllers, query handlers, and dedicated read abstractions per feature slice. This feature should follow the same structure used by `002-recipe-listing`.
- Existing seed and persistence configuration currently target listing data only. Detail behavior will require expanding seed coverage to include ingredients, steps, and tags.
- The database bootstrap currently relies on `EnsureCreated`. If schema changes are introduced, local verification should account for database recreation/reset when needed.
- IDs are GUIDs in the current model, so route binding and validation should align with GUID-based identifiers.

---

# Phase 1 - Contract And Endpoint Semantics

Define the exact request and response contract before coding.

Work

- Confirm endpoint contract: `GET /recipes/{id}` with `id` as `Guid`.
- Confirm response model fields for `RecipeDetailDto`:
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
- Confirm nested DTO contracts and ordering requirements:
  - ingredients ordered by `position`
  - steps ordered by `stepNumber`
  - images ordered by `position`
- Define missing recipe behavior as `404 Not Found` with consistent error payload style for the API.
- Decide nullability expectations for optional values:
  - brew specs coffee fields may be `null`
  - optional image captions may be `null`
  - optional author avatar may be `null` if model allows

Outcome

- A stable response contract exists and can be implemented without ambiguity.

---

# Phase 2 - Domain And Application Slice Expansion

Add domain and application abstractions required for detail retrieval.

Work

- Add recipe detail domain types not yet present:
  - `RecipeIngredient`
  - `RecipeStep`
  - `Tag`
  - junction model for recipe-tag relationship if represented explicitly
- Extend existing domain types where needed:
  - `Author` to support `avatarUrl` (if included in detail DTO)
  - `Recipe` navigation properties for ingredients, steps, tags
- Add application query slice for recipe detail.
  Suggested location:
  `src/CoffeeCodex.Application/Recipes/Queries/GetRecipeDetail`
- Define:
  - `GetRecipeDetailQuery`
  - `RecipeDetailDto` and nested DTOs
  - handler interface and implementation contract
  - read abstraction interface for infrastructure
- Add validation for route input (`id` must be a non-empty GUID).

Outcome

- Application layer owns the recipe detail use case and contracts.

---

# Phase 3 - Persistence Model And EF Core Mapping

Expand EF Core model to support complete recipe detail retrieval.

Work

- Extend `CoffeeCodexDbContext` with missing DbSets:
  - `RecipeIngredients`
  - `RecipeSteps`
  - `Tags`
  - `RecipeTags` (or implicit many-to-many mapping)
- Add entity configurations for:
  - `recipe_ingredients`
  - `recipe_steps`
  - `tags`
  - `recipe_tags`
- Ensure column mappings align with `architecture.md`:
  - ingredient quantity/unit/position
  - step number/instruction
  - image caption/position
  - tag name
- Add indexes that support fast detail reads and ordered child retrieval:
  - `recipe_ingredients(recipe_id, position)`
  - `recipe_steps(recipe_id, step_number)`
  - `recipe_images(recipe_id, position)` (already required by listing, verify preserved)
  - `recipe_tags(recipe_id, tag_id)`
- Update seed data to include realistic ingredients, steps, and tags for multiple recipes, including edge cases:
  - recipe with no tags
  - recipe with optional brew spec values as null
  - recipe with multiple images and steps

Outcome

- Persistence model can represent and retrieve all recipe detail fields efficiently.

---

# Phase 4 - Detail Query Implementation

Implement the infrastructure read path for detail retrieval.

Work

- Implement detail reader in Infrastructure using `AsNoTracking()`.
- Query by recipe id and project directly to `RecipeDetailDto`.
- Include nested data with deterministic ordering in projection:
  - ingredients by `position`
  - steps by `stepNumber`
  - images by `position`
  - tags by stable order (for example, name or insertion order)
- Include author and brew specs in the same detail response.
- Avoid N+1 behavior and avoid cartesian explosion risks from multi-collection joins.
  Use a projection-first approach and query shape that remains predictable.
- Return `null` from reader when recipe is not found so handler/controller can map to `404`.

Outcome

- A single detail use case returns complete recipe data with stable ordering and no N+1 query pattern.

---

# Phase 5 - API Endpoint Integration

Expose the application query through the recipes API surface.

Work

- Add `GET /recipes/{id}` to `RecipesController`.
- Bind route id and delegate to `GetRecipeDetail` handler.
- Return:
  - `200 OK` with `RecipeDetailDto` when found
  - `404 Not Found` when missing
  - `400 Bad Request` for invalid input when applicable under controller binding/validation flow
- Add OpenAPI response metadata attributes for 200/400/404.
- Register any new validators/handlers/readers in Application and Infrastructure DI extensions.

Outcome

- Frontend can retrieve full recipe detail through a stable public endpoint.

---

# Phase 6 - Testing And Verification

Add coverage for behavior, data ordering, and error handling.

Work

- Add application-level tests for query validation behavior.
- Add infrastructure tests for detail projection:
  - returns full detail payload for existing recipe
  - ingredients ordered by position
  - steps ordered by step number
  - images ordered by position
  - tags included correctly
  - optional brew fields preserved as null when applicable
  - missing recipe returns null from reader
- Add API integration tests:
  - `GET /recipes/{id}` returns `200` with expected contract
  - unknown id returns `404`
  - invalid id route returns framework-consistent client error
- Run manual smoke verification using seeded data.

Outcome

- Core recipe detail behavior is protected against regressions.

---

# Phase 7 - Performance And Observability Checks

Verify the endpoint aligns with MVP non-functional requirements.

Work

- Confirm endpoint is captured by existing OpenTelemetry ASP.NET instrumentation.
- Review generated SQL/query behavior to confirm no N+1 pattern.
- Validate payload size stays reasonable for frontend rendering.
- Spot check response time against the `< 100ms` target on local representative dataset.

Outcome

- Endpoint is observable and performance expectations are validated for MVP scale.

---

# Suggested Implementation Order

1. Finalize detail DTO contract and endpoint semantics.
2. Add missing domain entities and application query slice.
3. Extend EF model/configuration and seed data for detail relations.
4. Implement projection-based detail reader.
5. Add `GET /recipes/{id}` API action and DI wiring.
6. Add automated tests and run manual smoke checks.
7. Perform SQL/performance sanity checks.

---

# Completion Criteria

The feature is complete when:

- `GET /recipes/{id}` is publicly available
- response includes complete `RecipeDetailDto` data
- related collections are returned in deterministic order
- missing recipe returns `404`
- query path avoids N+1 behavior
- automated tests cover happy path and not-found behavior

---

# Follow-On Dependencies

This feature should leave the codebase ready for:

- `004-recipe-images`
- `005-recipe-filters`
- `006-recipe-search`

The recipe detail slice should establish reusable patterns for:

- rich read-model projection
- ordered child collection mapping
- not-found handling for public recipe APIs
