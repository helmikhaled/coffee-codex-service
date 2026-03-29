# Plan - 002 Recipe Listing API

This spec delivers the first public recipe vertical slice for the backend.

The implementation should expose `GET /recipes`, return paginated `RecipeSummaryDto` results, and preserve the admin-defined curated order.

---

# Scope

In scope

- public `GET /recipes` endpoint
- pagination via `page` and `pageSize`
- deterministic ordering by `display_order`
- thumbnail selection from `recipe_images.position = 1`
- author name and difficulty in the listing payload
- total count in the paginated response

Out of scope

- filtering
- search
- random recipe
- admin APIs
- analytics side effects

---

# Planning Notes

- The PRD requires `display_order`, but `docs/architecture.md` does not currently list that column in the `recipes` table. The implementation should treat the PRD as the feature source of truth and add `display_order` to the domain and persistence model.
- The current codebase is still scaffolded, so this feature will establish the first real end-to-end query slice across Domain, Application, Infrastructure, and API.
- The API project currently uses minimal API startup only. The implementation should add the recipe listing endpoint in a way that stays consistent with the existing API style unless the team explicitly decides to introduce controllers now.

---

# Phase 1 - Contract And Model Alignment

Define the backend contract and the minimum data model needed for recipe summaries.

Work

- Confirm the response envelope shape for `PagedResponse<RecipeSummaryDto>`.
- Standardize pagination rules:
  - `page` is 1-based
  - `pageSize` has a safe default
  - `pageSize` has a maximum limit to protect query cost
- Confirm `RecipeSummaryDto` fields from `architecture.md`:
  - `id`
  - `slug`
  - `title`
  - `category`
  - `thumbnailUrl`
  - `brewCount`
  - `authorName`
  - `difficulty`
- Define how `difficulty` is sourced for listing queries through `recipe_brew_specs`.
- Decide tie-break ordering for stable pagination when multiple rows share the same `display_order`.
  Recommendation: `display_order ASC`, then `id ASC`.

Outcome

- A stable query contract exists before code is written.
- Domain and schema requirements for listing are unambiguous.

---

# Phase 2 - Domain And Application Abstractions

Introduce the application slice that represents listing recipes without leaking persistence concerns upward.

Work

- Add recipe-related domain types needed by this feature:
  - `Recipe`
  - `Author`
  - `RecipeImage`
  - `RecipeBrewSpecs`
  - supporting enums such as `RecipeCategory` and `DifficultyLevel`
- Add an application query contract for listing recipes.
  Suggested location:
  `src/CoffeeCodex.Application/Recipes/Queries/GetRecipes`
- Define:
  - query/request model with `page` and `pageSize`
  - response DTO or shared paging envelope usage
  - handler contract
- Add validation rules for pagination inputs.
  Examples:
  - `page >= 1`
  - `1 <= pageSize <= configured max`
- Add an abstraction for the data access required by the query.
  This can be:
  - a repository dedicated to recipe reads, or
  - a query service/read model interface optimized for projections

Outcome

- The Application layer owns the use case and validation.
- The query can be executed without the API knowing EF Core details.

---

# Phase 3 - Persistence Model And EF Core Mapping

Model the recipe listing data in PostgreSQL and EF Core so the query can be executed efficiently.

Work

- Expand `CoffeeCodexDbContext` with the DbSets required for listing:
  - recipes
  - authors
  - recipe_brew_specs
  - recipe_images
- Add EF Core entity configurations for:
  - recipe table and key fields
  - author relationship
  - one-to-one or owned relationship for brew specs
  - one-to-many relationship for recipe images
- Include the `display_order` column in the recipe mapping.
- Ensure the image position field is mapped so the query can reliably select the thumbnail.
- Add schema constraints and indexes that support listing performance.
  Recommended targets:
  - index on `recipes.display_order`
  - index on `recipe_images(recipe_id, position)`
- Add an EF Core migration for the initial recipe-listing schema changes if migrations are part of the repository workflow.
- Seed or prepare representative data for local verification:
  - multiple recipes
  - overlapping `display_order` edge case
  - recipes without images
  - recipes without brew specs if allowed by the model

Outcome

- The persistence layer can represent all data required by the endpoint.
- The database model supports deterministic ordering and efficient thumbnail lookup.

---

# Phase 4 - Query Implementation

Implement the listing query with projection-first data access to keep the payload lightweight.

Work

- Implement the recipe listing read path in Infrastructure.
- Use `AsNoTracking()` and project directly to the summary model instead of materializing full aggregates.
- Execute pagination with:
  - total count query
  - ordered page query
- Order by:
  - `display_order ASC`
  - stable secondary key
- Select the thumbnail from the image row where `position = 1`.
- Join or project the author name and recipe difficulty into the result.
- Handle null cases safely:
  - missing thumbnail returns `null`
  - missing brew specs returns `null` difficulty if the API contract permits it
- Keep the query shaped to avoid N+1 behavior.
  A single projection query for page data is preferred.

Outcome

- The query returns a lightweight, paginated recipe summary list.
- The implementation satisfies the PRD risk mitigation around image retrieval.

---

# Phase 5 - API Endpoint

Expose the application query through the public HTTP API.

Work

- Add a dedicated recipe listing endpoint in the API project.
  Suggested location:
  `src/CoffeeCodex.API/Recipes`
- Bind query string parameters:
  - `page`
  - `pageSize`
- Delegate execution to the Application layer.
- Return `200 OK` with `PagedResponse<RecipeSummaryDto>`.
- Return validation failures as `400 Bad Request` using the repo's chosen validation/error pattern.
- Register the endpoint in `Program.cs` or through an endpoint/module registration extension.
- Ensure the endpoint remains publicly accessible and does not require Auth0 authentication.

Outcome

- Frontend clients can call `GET /recipes?page=1&pageSize=12`.
- The endpoint shape is aligned with the shared architecture documents.

---

# Phase 6 - Observability And Performance Guardrails

Make the new endpoint measurable and keep it aligned with MVP performance goals.

Work

- Ensure the existing OpenTelemetry ASP.NET Core instrumentation captures the request.
- Add structured logging around recipe-list execution only if the project establishes a clear logging convention.
- Verify the endpoint avoids over-fetching:
  - no full recipe graph loading
  - no unnecessary blob metadata fields
- Validate that pagination defaults prevent oversized result sets.
- Check generated SQL during development to confirm:
  - ordered pagination is pushed to SQL
  - thumbnail selection does not trigger per-row follow-up queries

Outcome

- The feature remains observable through the existing telemetry pipeline.
- The listing path is designed to stay within the `< 100ms` query target for MVP-sized datasets.

---

# Phase 7 - Testing And Verification

Add coverage for behavior, contract stability, and edge cases before extending the recipes surface further.

Work

- Add application-level tests for pagination validation.
- Add persistence or integration tests for:
  - returns paginated results
  - returns total count
  - orders by `display_order`
  - uses the stable secondary sort
  - selects thumbnail from image `position = 1`
  - returns `null` thumbnail when no image exists
  - handles empty result sets
- Add API-level tests or end-to-end tests for:
  - `GET /recipes` returns `200 OK`
  - query string binding works
  - invalid pagination inputs return `400`
- Perform a manual smoke check with sample data against the running API.

Outcome

- The feature is protected against regressions before recipe detail, filters, and search are added.

---

# Suggested Implementation Order

1. Finalize paging contract and validation rules.
2. Add domain types and application query slice.
3. Add EF Core mappings, schema support, and sample data.
4. Implement the projection-based listing query.
5. Expose `GET /recipes` in the API layer.
6. Add tests and verify generated SQL and endpoint behavior.

---

# Completion Criteria

The feature is complete when:

- `GET /recipes` is publicly available
- results are paginated and include total count
- recipe summaries match `RecipeSummaryDto`
- ordering is deterministic by `display_order`
- thumbnail selection uses image position `1`
- the implementation avoids N+1 queries
- automated or integration coverage exists for the core listing behavior

---

# Follow-On Dependencies

This feature should leave the codebase ready for:

- `003-recipe-detail`
- `005-recipe-filters`
- `006-recipe-search`

The recipe listing slice should therefore establish reusable patterns for:

- recipe query organization
- paging response types
- EF Core entity configuration
- public recipe endpoint registration
