# Plan - 006 Recipe Search API

This spec extends the public recipe listing slice with keyword search support.

The implementation should keep `GET /recipes` as the single discovery endpoint, add optional `search` behavior on top of existing category/tag filtering, preserve pagination, and keep curated ordering with lightweight relevance rules.

---

# Scope

In scope

- extend `GET /recipes` with optional `search` query parameter
- keyword matching across recipe title and tags
- ingredient name search (resolved from PRD open question: Yes)
- combine search with existing filters (`category`, repeated `tag`)
- maintain paged response contract and deterministic ordering
- add database support/indexes for search latency target

Out of scope

- fuzzy or typo-tolerant search
- semantic or AI search
- personalized ranking/recommendations
- dedicated `/search` endpoint

---

# Planning Notes

- Existing listing/filtering slice (Spec 002/005) already exists and should be extended in place, not replaced.
- Current listing order is `display_order ASC`, then `id ASC`. PRD open question requires title matches to be prioritized; this introduces a minimal relevance tier while keeping curated order inside each tier.
- PRD has a conflict:
  - section G says search matches title and tags, and ingredient search is future
  - section N says ingredient search should be supported now
- This plan assumes section N is accepted and includes ingredient-name matching in this spec.
- PostgreSQL + EF Core are already in use; `EF.Functions.ILike` is the lowest-complexity path for case-insensitive keyword matching.
- Existing indexes already cover filters (`recipes.category`, `tags.name`, `recipe_tags` join indexes). Search by `ILIKE '%term%'` needs additional index strategy to avoid full scans at scale.

---

# Planned File Touchpoints

API

- `src/CoffeeCodex.API/Recipes/GetRecipesRequest.cs`

Application

- `src/CoffeeCodex.Application/Recipes/Queries/GetRecipes/GetRecipesQuery.cs`
- `src/CoffeeCodex.Application/Recipes/Queries/GetRecipes/GetRecipesQueryValidator.cs`
- `src/CoffeeCodex.Application/Recipes/Queries/GetRecipes/RecipeListingDefaults.cs`

Infrastructure

- `src/CoffeeCodex.Infrastructure/Recipes/RecipeSummaryReader.cs`
- `src/CoffeeCodex.Infrastructure/Persistence/Configurations/RecipeConfiguration.cs` (if index metadata is expressed in config)
- `src/CoffeeCodex.Infrastructure/Persistence/Configurations/TagConfiguration.cs` (if index metadata is expressed in config)
- `src/CoffeeCodex.Infrastructure/Persistence/Configurations/RecipeIngredientConfiguration.cs` (if index metadata is expressed in config)
- `src/CoffeeCodex.Infrastructure/Persistence/Migrations/<timestamp>_AddRecipeSearchIndexes.cs`
- `src/CoffeeCodex.Infrastructure/Persistence/Migrations/CoffeeCodexDbContextModelSnapshot.cs`

Tests

- `tests/CoffeeCodex.RecipeListing.Tests/Application/GetRecipesQueryValidatorTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/Infrastructure/RecipeSummaryReaderTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`
- `tests/CoffeeCodex.RecipeListing.Tests/RecipeListingTestData.cs` (only if additional ranking fixtures are needed)

---

# Phase 1 - Search Contract And Semantics

Define exact search behavior before code changes.

Work

- keep endpoint surface as `GET /recipes`; add optional `search` query parameter
- define normalization rules:
  - trim surrounding whitespace
  - empty/whitespace-only value behaves as no search filter
  - preserve internal spaces (phrase search)
- define matching behavior:
  - case-insensitive contains match (`ILIKE`) on:
    - `recipes.title`
    - `tags.name`
    - `recipe_ingredients.name`
- define combination semantics with existing filters:
  - search predicate is OR across searchable fields
  - search predicate is AND-ed with `category` and `tag` filters when those are present
- define ordering behavior when `search` is present:
  - first tier: recipes with title match
  - second tier: recipes matched by tag/ingredient only
  - within each tier preserve curated ordering:
    - `display_order ASC`
    - `id ASC`
- preserve existing pagination contract (`page`, `pageSize`, `totalCount`)

Outcome

- search semantics are explicit and testable across API, validation, and persistence layers

---

# Phase 2 - API And Application Contract Updates

Extend request/query models while keeping layer boundaries clean.

Work

- add `Search` to `GetRecipesRequest` and map it in `ToQuery()`
- add `Search` to `GetRecipesQuery`
- add shared limit constant for search input length
  Recommendation: `MaxSearchLength = 100`
- extend query validation:
  - enforce max length
  - allow null/empty after trimming as "no filter"
  - keep existing page/pageSize/category/tag validation unchanged
- keep handler and reader abstractions unchanged in shape where possible (same `GetRecipesQuery` flow)

Outcome

- application contract carries search intent without leaking EF or SQL details

---

# Phase 3 - Search Query Implementation

Implement search composition in Infrastructure with deterministic behavior.

Work

- extend `RecipeSummaryReader` query pipeline:
  - start from `Recipes.AsNoTracking()`
  - apply existing category/tag filters
  - apply optional search predicate over title/tags/ingredients
- use SQL-translatable expression style so the predicate stays server-side
- apply ranking tier for title matches before curated ordering when search is active
- keep count and items queries aligned so `totalCount` reflects the same filters
- preserve existing projection behavior:
  - summary DTO only
  - thumbnail from image `position = 1`
  - no N+1 query pattern

Outcome

- listing endpoint supports search with predictable sort and pagination behavior

---

# Phase 4 - PostgreSQL Performance Support

Add index strategy required for keyword-search target latency.

Work

- validate generated SQL/query plans for representative searches
- add migration support for PostgreSQL trigram search acceleration:
  - ensure `pg_trgm` extension is enabled
  - add GIN trigram index for `recipes.title`
  - add GIN trigram index for `tags.name`
  - add GIN trigram index for `recipe_ingredients.name` (if ingredient search is in scope)
- keep existing filter indexes intact
- ensure migration has safe rollback steps

Outcome

- search queries remain performant as dataset size grows and avoid heavy full table scans

---

# Phase 5 - API Surface Finalization

Finalize request binding and outward contract behavior.

Work

- ensure controller binding accepts `search` alongside existing query params
- keep response contract unchanged: `PagedResponse<RecipeSummaryDto>`
- keep validation failure behavior consistent (`400 Bad Request` with problem details)
- add or verify OpenAPI metadata for `search` parameter on `GET /recipes`

Outcome

- frontend can call combined discovery requests such as:
  - `GET /recipes?search=matcha&page=1&pageSize=12`
  - `GET /recipes?search=iced&category=Iced&tag=sparkling&page=1&pageSize=12`

---

# Phase 6 - Testing And Verification

Add regression coverage across validation, query behavior, and API surface.

Work

- application validator tests:
  - overlong `search` fails validation
  - null/empty/whitespace `search` is accepted
  - existing category/tag validation remains unchanged
- infrastructure reader tests:
  - title match returns expected recipes
  - tag-name match returns expected recipes
  - ingredient-name match returns expected recipes
  - search is case-insensitive
  - search + category filter uses AND semantics
  - search + tag filter uses AND semantics
  - pagination and total count remain correct under search
  - title-match tier ordering precedes tag/ingredient-only matches
  - no-match search returns empty items and `totalCount = 0`
- API integration tests:
  - `GET /recipes?search=matcha` returns `200` with expected payload
  - `GET /recipes?search=tonic` returns ingredient/tag/title-backed matches
  - `GET /recipes?search=MaTcHa` validates case-insensitive behavior
  - `GET /recipes?search=<over-max>` returns `400`
  - combined query scenario with search + filters
- extend seeded test data only where needed to prove ranking tier behavior

Outcome

- search behavior is protected end-to-end and existing listing/filter behavior is not regressed

---

# Phase 7 - Performance And Observability Checks

Confirm implementation meets PRD non-functional goals.

Work

- run targeted tests and local smoke calls for search paths
- inspect SQL shape for:
  - search only
  - search + category
  - search + tags
- validate no N+1 patterns are introduced
- verify endpoint remains visible in current OpenTelemetry traces
- measure representative local latency against PRD target (`< 150ms`) and record findings

Outcome

- search path is production-ready for MVP scale with measurable behavior

---

# Suggested Implementation Order

1. Lock final semantics for ingredient coverage and title-priority ordering.
2. Extend request/query contracts and validation.
3. Implement search predicate and sort tiers in `RecipeSummaryReader`.
4. Add PostgreSQL search indexes via migration.
5. Add or adjust automated tests across Application, Infrastructure, and API.
6. Perform query-plan and latency sanity checks.
7. Finalize OpenAPI/contract documentation.

---

# Completion Criteria

The feature is complete when:

- `GET /recipes` supports optional `search` parameter
- search matches title, tags, and ingredients according to agreed scope
- search works with existing category/tag filters and pagination
- ordering preserves curated sort and applies title-priority tiering
- validation and error handling remain consistent
- index/migration support exists for keyword-search performance
- automated tests cover search behavior and regressions
- measured latency is aligned with PRD target on representative local data

---

# Follow-On Dependencies

This feature should leave the codebase ready for:

- richer discovery combinations on the same listing endpoint
- future ranking improvements without API contract changes
- optional evolution toward advanced search (kept out of MVP scope)
