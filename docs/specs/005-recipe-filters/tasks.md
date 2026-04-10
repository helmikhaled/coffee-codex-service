# Tasks - 005 Recipe Filters API

Tasks must be executed sequentially.

---

# Task 1 - Finalize Filter Semantics [ ]

Lock filtering behavior before implementation.

Define:

- endpoint remains `GET /recipes`
- optional filters: `category`, `tag`
- multi-tag request shape: repeated query params (`?tag=a&tag=b`)
- multi-tag semantics: OR match (recipe has at least one requested tag)
- combined behavior: `category` AND tag-filter result set
- category match rule: case-sensitive enum match only

Verify:

- filter semantics are documented and unambiguous for API and tests

---

# Task 2 - Extend Listing Request Contract [ ]

Update API request model for filter inputs.

Update `GetRecipesRequest` to include:

- optional `category`
- optional tag collection from query string
- existing `page` and `pageSize` defaults unchanged

Verify:

- request model can map filtered and unfiltered listing calls

---

# Task 3 - Extend Application Query Contract [ ]

Carry filter inputs into the application use case.

Update `GetRecipesQuery` to include:

- optional category filter
- optional normalized tag filter collection
- existing pagination fields unchanged

Verify:

- query object can represent all supported filter combinations

---

# Task 4 - Add Filter Validation Rules [ ]

Extend listing query validation while preserving existing pagination validation.

Add rules for:

- category must map to `RecipeCategory` using case-sensitive parsing
- invalid category (including casing mismatch) fails validation
- tag entries cannot be null/empty/whitespace
- duplicate tags are normalized or ignored deterministically

Verify:

- invalid filter inputs return validation failures suitable for `400` responses

---

# Task 5 - Wire Request-To-Query Mapping [ ]

Ensure API mapping produces the correct application query payload.

Update mapping logic to:

- pass `category` and tag list from `GetRecipesRequest` to `GetRecipesQuery`
- preserve existing pagination mapping behavior

Verify:

- query object contents match incoming query string parameters

---

# Task 6 - Implement Category Filter In Reader [ ]

Apply category filter in infrastructure listing query.

Update `RecipeSummaryReader` to:

- conditionally apply `category` predicate when provided
- skip category filtering when not provided

Verify:

- category-only requests return only matching categories

---

# Task 7 - Implement Tag Filter In Reader [ ]

Apply tag filtering using recipe-tag relationships.

Update `RecipeSummaryReader` to:

- conditionally apply tag filter when one or more tags are provided
- use `recipe_tags`/`tags` relationship for matching
- implement OR semantics across requested tags

Verify:

- tag-only requests return only recipes with at least one matching tag

---

# Task 8 - Preserve Listing Behavior With Filters [ ]

Ensure filtered and unfiltered listing retain existing behavior.

Verify query path still:

- calculates `totalCount` from the filtered base query
- orders by `display_order ASC`, then `id ASC`
- applies `Skip/Take` pagination after filtering
- projects to `RecipeSummaryDto` with thumbnail from image `position = 1`

Verify:

- filtered paging metadata and item ordering are deterministic

---

# Task 9 - Add/Adjust Filter Indexes [ ]

Harden persistence for filter performance.

Update EF configurations as needed to include:

- index on `tags.name`
- index supporting tag->recipe traversal (`recipe_tags(tag_id, recipe_id)`) if missing
- optional index on `recipes.category` if query analysis indicates value

Verify:

- EF model reflects required indexes for filter query patterns

---

# Task 10 - Add Migration For Schema/Index Changes [ ]

Persist any index/schema updates introduced by Task 9.

Implement:

- EF Core migration
- updated model snapshot

Verify:

- migration applies successfully and cleanly on local database

---

# Task 11 - Finalize API Endpoint Contract Metadata [ ]

Keep API surface consistent for filtered listing.

Update endpoint metadata/documentation to reflect:

- optional `category` and repeated `tag` query parameters
- unchanged response contract `PagedResponse<RecipeSummaryDto>`
- unchanged validation error behavior (`400 Bad Request`)

Verify:

- API contract remains backward compatible for unfiltered callers

---

# Task 12 - Add Application Validation Tests [ ]

Protect filter validation behavior.

Add tests for:

- invalid category value/casing fails
- valid category passes
- empty/whitespace tag input fails
- pagination validation rules still enforced

Verify:

- validation tests pass and cover new filter rules

---

# Task 13 - Add Infrastructure Reader Tests [ ]

Protect filtered query behavior at read-model level.

Add tests for:

- category-only filter
- tag-only filter
- multi-tag OR filter behavior
- combined category + tag filters
- filtered `totalCount` correctness
- filtered ordering and pagination
- no-match filters return empty items with `totalCount = 0`

Verify:

- infrastructure tests pass and confirm deterministic results

---

# Task 14 - Add API Integration Tests [ ]

Protect end-to-end filter behavior.

Add tests for:

- `GET /recipes?category=Modern`
- `GET /recipes?category=modern` returns `400`
- `GET /recipes?tag=matcha`
- `GET /recipes?tag=matcha&tag=iced`
- `GET /recipes?category=Modern&tag=matcha`
- existing unfiltered listing behavior remains valid

Verify:

- API integration tests pass for filtered and unfiltered scenarios

---

# Task 15 - Run Automated Test Suite [ ]

Run target test projects after implementation.

Execute:

- application tests
- infrastructure tests
- API integration tests

Verify:

- all relevant tests pass with no regressions

---

# Task 16 - Run Manual And Performance Verification [ ]

Perform final runtime verification.

Validate manually:

- representative filter combinations return expected payloads
- ordering and pagination remain correct under filters
- filtered query path remains under `< 100ms` target on representative local data
- request tracing is visible via existing OpenTelemetry instrumentation

Verify:

- implementation is ready for handoff to Spec 006 integration

---

# Completion

Recipe filters are complete when:

- `GET /recipes` supports category and tag filters
- multi-tag OR filtering works with repeated `tag` parameters
- combined filtering preserves curated ordering and pagination
- invalid category casing/value is rejected consistently
- filtered listing performance meets MVP targets
- automated and manual verification are complete
