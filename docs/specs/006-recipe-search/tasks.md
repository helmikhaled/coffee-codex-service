# Tasks - 006 Recipe Search API

Tasks must be executed sequentially.

---

# Task 1 - Finalize Search Semantics [x]

Lock search behavior before code changes.

Define:

- endpoint remains `GET /recipes`
- optional query parameter `search`
- search normalization rule: trim; whitespace-only acts as no filter
- searchable fields: `recipes.title`, `tags.name`, `recipe_ingredients.name`
- search combination rule: searchable fields use OR semantics
- filter combination rule: search result is AND-ed with `category` and `tag` filters
- ranking rule: title matches first, then tag/ingredient-only matches
- tie-break rule within each ranking tier: `display_order ASC`, then `id ASC`

Verify:

- semantics are unambiguous for API, query logic, and tests

---

# Task 2 - Extend API Listing Request Contract [x]

Add search input to the HTTP request model.

Update `GetRecipesRequest` to include:

- optional `search`
- existing `page`, `pageSize`, `category`, and `tag` behavior unchanged

Verify:

- query-string binding supports `GET /recipes?search=matcha`

---

# Task 3 - Add Request-To-Query Search Mapping [x]

Pass and normalize `search` from API to Application query object.

Update `GetRecipesRequest.ToQuery()` to:

- trim `search` input
- map normalized `search` into `GetRecipesQuery`
- keep existing tag normalization behavior intact

Verify:

- mapped query payload is correct for null, whitespace, and non-empty search inputs

---

# Task 4 - Extend Application Query Contract [x]

Carry search criteria through the use-case contract.

Update `GetRecipesQuery` to include:

- optional `search` field
- existing pagination/filter fields unchanged

Verify:

- query model can represent unfiltered, filtered, and search+filter combinations

---

# Task 5 - Add Search Validation Limits [x]

Define reusable search input limits in listing defaults.

Update `RecipeListingDefaults` with:

- max search length constant (for example `100`)

Verify:

- search validation can reference a single shared constant

---

# Task 6 - Add Search Validation Rules [x]

Extend `GetRecipesQueryValidator` for search input.

Add rules for:

- max length enforcement for non-null `search`
- null or whitespace-only `search` accepted
- existing category/tag/page/pageSize validation preserved

Verify:

- invalid search input produces validation failures suitable for `400 Bad Request`

---

# Task 7 - Implement Title Search Predicate [x]

Add title keyword matching in `RecipeSummaryReader`.

Implement:

- case-insensitive contains search on `recipes.title` using SQL-translatable EF expression
- no search predicate applied when normalized search is empty

Verify:

- title matches are returned when `search` is provided

---

# Task 8 - Implement Tag Search Predicate [x]

Extend search to tag names in `RecipeSummaryReader`.

Implement:

- case-insensitive contains search against related `tags.name`
- keep predicate composition server-side in SQL

Verify:

- recipes can be returned from tag-only search matches

---

# Task 9 - Implement Ingredient Search Predicate [x]

Extend search to ingredient names in `RecipeSummaryReader`.

Implement:

- case-insensitive contains search against related `recipe_ingredients.name`
- keep predicate composition SQL-translatable

Verify:

- recipes can be returned from ingredient-only search matches

---

# Task 10 - Compose Search With Existing Filters [x]

Integrate search with current category and tag filters.

Implement query composition order to ensure:

- search predicate is OR across title/tag/ingredient fields
- category and explicit tag filters still apply as AND constraints

Verify:

- combined requests behave correctly, e.g. `search + category`, `search + tag`

---

# Task 11 - Add Title-Priority Ordering Tier [x]

Apply lightweight ranking while preserving curated ordering.

Update ordering logic when `search` is present:

- tier 1: title matches
- tier 2: non-title matches (tag/ingredient)
- within tiers keep `display_order ASC`, then `id ASC`

Verify:

- title matches are prioritized without breaking deterministic ordering

---

# Task 12 - Preserve Paging And Count Semantics [x]

Ensure search path keeps listing contract guarantees.

Verify query behavior:

- `totalCount` uses the same filtered/searched base query
- pagination still uses `Skip/Take` after filters and ordering
- response contract remains `PagedResponse<RecipeSummaryDto>`

Verify:

- paged results and metadata remain correct with and without search

---

# Task 13 - Add PostgreSQL Search Index Configuration [x]

Add DB support to reduce risk of search-related full table scans.

Update persistence model/migration inputs for:

- `pg_trgm` extension enablement
- trigram index support for `recipes.title`
- trigram index support for `tags.name`
- trigram index support for `recipe_ingredients.name`

Verify:

- EF model changes reflect required search index strategy

---

# Task 14 - Add EF Core Migration For Search Indexes [x]

Persist schema/index changes introduced in Task 13.

Implement:

- new migration file for search indexes/extensions
- model snapshot update

Verify:

- migration applies and rolls back cleanly on local database

---

# Task 15 - Add Application Validation Tests [x]

Protect query validation behavior for the new search contract.

Add tests for:

- over-max search length fails validation
- null search passes
- whitespace-only search passes
- existing category/tag/page/pageSize validation remains valid

Verify:

- validator test suite passes with new search rules

---

# Task 16 - Add Infrastructure Search Query Tests [x]

Protect reader-level search behavior and ordering.

Add tests for:

- title match
- tag match
- ingredient match
- case-insensitive matching
- search + category AND semantics
- search + tag AND semantics
- title-priority ordering
- pagination and `totalCount` correctness under search
- no-match search returns empty items and `totalCount = 0`

Verify:

- infrastructure tests pass with deterministic results

---

# Task 17 - Add API Integration Tests For Search [x]

Protect end-to-end HTTP behavior for search.

Add tests for:

- `GET /recipes?search=matcha`
- `GET /recipes?search=MaTcHa`
- `GET /recipes?search=tonic` (title/tag/ingredient coverage)
- `GET /recipes?search=<over-max>` returns `400`
- combined search + category/tag query scenarios
- unfiltered listing behavior remains unchanged

Verify:

- API integration tests pass for search and regression cases

---

# Task 18 - Finalize API Metadata For Search [x]

Ensure public API contract documentation includes search.

Update/verify endpoint metadata:

- `search` is documented as optional query parameter on `GET /recipes`
- success and validation responses remain unchanged

Verify:

- OpenAPI surface reflects the implemented search contract

---

# Task 19 - Run Automated Test Verification [x]

Run relevant test suites after implementation.

Execute:

- application tests
- infrastructure tests
- API integration tests

Verify:

- all targeted tests pass with no regressions

---

# Task 20 - Run Manual Performance And Observability Checks [x]

Perform final runtime validation for search readiness.

Validate manually:

- representative search-only and search+filter requests
- deterministic ordering and pagination in responses
- search latency target `< 150ms` on representative local dataset
- request visibility in existing OpenTelemetry traces

Verify:

- feature is ready for handoff to downstream specs

---

# Completion

Recipe search is complete when:

- `GET /recipes` supports optional `search`
- search matches title, tags, and ingredients per agreed semantics
- title-priority ordering is applied while preserving curated ordering
- search integrates correctly with existing category/tag filters and pagination
- PostgreSQL index support is in place for search query performance
- automated and manual verification are complete
