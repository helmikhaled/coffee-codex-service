# Plan - 005 Recipe Filters API

This spec extends the existing public recipe listing slice with filter support.

The implementation should keep `GET /recipes` as the single listing endpoint, add category and tag filtering, preserve deterministic ordering and pagination behavior, and remain aligned with MVP performance goals.

---

# Scope

In scope

- extend `GET /recipes` with optional filter parameters
- category filter support
- tag filter support (including multiple tags)
- combined filtering support (`category` + `tag`)
- preserve pagination and curated ordering behavior
- filtered query performance and regression coverage

Out of scope

- full-text search (Spec 006)
- ranking/recommendation logic
- personalization
- admin-side filter management

---

# Planning Notes

- The listing endpoint (`GET /recipes`) is already implemented in the current codebase and should be extended, not replaced.
- Existing listing behavior from Spec 002 must remain intact:
  - deterministic order by `display_order`, then `id`
  - pagination via `page` and `pageSize`
  - thumbnail selection from image `position = 1`
- `architecture.md` defines category as an enum and tag data in `tags`/`recipe_tags`; filtering should use these existing models without introducing new domain entities.
- PRD requires category matching to be case-sensitive. This should be explicitly enforced in request validation/parsing (default enum model binding behavior should not be relied on).
- PRD requires multiple tag support; query contract and filter semantics must be explicitly defined before implementation.

---

# Phase 1 - Filter Contract And Semantics

Lock filter behavior before code changes.

Work

- Extend `GET /recipes` query contract with optional filters:
  - `category`
  - `tag`
- Define multi-tag request shape.
  Recommendation: repeated query parameter format:
  - `GET /recipes?tag=matcha&tag=iced`
- Define multi-tag matching semantics.
  Recommendation: OR semantics (recipe matches if it has at least one requested tag) for MVP browsing usability.
- Define combined filtering behavior:
  - `category` filter AND `tag` filter applied together.
- Define category parsing behavior:
  - exact enum match only (case-sensitive)
  - invalid category value or casing returns `400 Bad Request`
- Preserve existing ordering and paging semantics after filtering.

Outcome

- Filter API contract is explicit, stable, and testable.

---

# Phase 2 - API And Application Contract Updates

Extend request/query models while keeping clean architecture boundaries.

Work

- Update API request model for listing (`GetRecipesRequest`) to accept optional `category` and tag collection input.
- Update application query model (`GetRecipesQuery`) to carry filter criteria.
- Keep query model focused on use-case inputs only (no EF/persistence details).
- Update listing query validation:
  - keep existing paging validation rules
  - validate category against `RecipeCategory` with case-sensitive parsing
  - validate tag inputs for empty/whitespace values
  - normalize tag collection for predictable processing (trim, deduplicate)
- Keep handler abstraction unchanged in shape where possible; only extend request payload.

Outcome

- Application layer owns filter contract and validation logic without persistence leakage.

---

# Phase 3 - Filter Query Implementation

Implement efficient filtered reads in Infrastructure.

Work

- Extend `RecipeSummaryReader` query composition:
  - start from base `Recipes.AsNoTracking()` query
  - conditionally apply `category` filter
  - conditionally apply `tag` filter via `RecipeTags`/`Tag`
  - apply combined filters when both are present
- Ensure filtered `totalCount` reflects the same filter set as paged items query.
- Preserve existing ordering:
  - `display_order ASC`
  - `id ASC`
- Preserve existing paging and projection behavior:
  - `Skip/Take`
  - projection-first `RecipeSummaryDto`
  - thumbnail selection from image `position = 1`
- Keep query shape N+1-safe.

Outcome

- Listing endpoint returns filtered, paginated recipe summaries with stable ordering.

---

# Phase 4 - Persistence Performance Support

Ensure schema/indexes support tag-filtered reads efficiently.

Work

- Review existing indexes involved in filter path:
  - `recipes.category` (if filtering frequency warrants index)
  - `tags.name`
  - `recipe_tags` join columns
- Add or adjust indexes where necessary for expected query shapes.
  Common targets:
  - index on `tags.name`
  - composite index on `recipe_tags(tag_id, recipe_id)` if not already present
- Add EF Core migration only if schema/index changes are introduced.
- Ensure migration keeps backward compatibility with existing data model.

Outcome

- Filtered queries remain performant as recipe/tag data grows.

---

# Phase 5 - API Surface Finalization

Expose filter behavior through the existing public endpoint contract.

Work

- Keep `GET /recipes` as the single listing endpoint.
- Ensure query-string binding supports:
  - paging parameters
  - category
  - repeated `tag` parameters
- Keep response type unchanged (`PagedResponse<RecipeSummaryDto>`).
- Keep validation failure behavior consistent (`400 Bad Request` with validation details).
- Update endpoint metadata/OpenAPI annotations as needed for filter parameters.

Outcome

- Frontend can call filtered listing requests without contract fragmentation.

---

# Phase 6 - Testing And Verification

Add focused coverage for filter behavior and regressions.

Work

- Application validation tests:
  - invalid category casing/value fails
  - valid category passes
  - empty tag entry fails (if validation rule enforces non-empty)
- Infrastructure reader tests:
  - category-only filter
  - tag-only filter
  - multi-tag filter semantics
  - combined category + tag filter
  - filtered ordering and stable tie-break
  - filtered pagination and total count correctness
  - no-match filter returns empty items with `totalCount = 0`
- API integration tests:
  - `GET /recipes?category=Modern` returns filtered results
  - `GET /recipes?category=modern` returns `400` (case-sensitive rule)
  - `GET /recipes?tag=matcha`
  - `GET /recipes?tag=matcha&tag=iced`
  - `GET /recipes?category=Modern&tag=matcha`
  - existing unfiltered listing behavior remains unchanged
- Manual smoke checks with seeded data for representative filter combinations.

Outcome

- Filter behavior is protected end-to-end and listing regressions are prevented.

---

# Phase 7 - Performance And Observability Checks

Confirm filtered listing meets MVP non-functional requirements.

Work

- Validate filtered query response time against PRD target (`< 100ms`) on representative local dataset.
- Review generated SQL/query plans for category-only, tag-only, and combined filters.
- Confirm no N+1 behavior in filtered listing path.
- Verify requests remain visible in existing OpenTelemetry instrumentation.

Outcome

- Filtered listing path is performant, observable, and production-ready for MVP scale.

---

# Suggested Implementation Order

1. Finalize filter contract semantics (category case-sensitivity, multi-tag shape, multi-tag matching semantics).
2. Extend API request/query models and validation rules.
3. Implement filtered query composition in `RecipeSummaryReader`.
4. Add/adjust indexes and migrations if query analysis indicates need.
5. Finalize endpoint binding and OpenAPI metadata.
6. Add automated tests across Application, Infrastructure, and API layers.
7. Run manual smoke checks and filtered-query performance verification.

---

# Completion Criteria

The feature is complete when:

- `GET /recipes` supports category and tag filters
- multi-tag requests are supported with documented semantics
- combined filters work predictably
- filtered results preserve curated ordering and pagination
- invalid category casing/value returns consistent validation errors
- filtered queries remain within performance targets
- automated tests cover filter logic and protect unfiltered listing behavior

---

# Follow-On Dependencies

This feature should leave the codebase ready for:

- Spec 006 recipe search integration on the same listing surface
- richer discovery combinations (filter + search)
- future admin/filter UX enhancements without endpoint redesign
