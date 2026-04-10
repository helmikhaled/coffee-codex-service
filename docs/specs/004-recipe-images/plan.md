# Plan - 004 Recipe Images

This spec delivers the recipe image metadata slice needed by recipe detail responses.

The implementation should ensure recipe images are modeled and stored correctly, returned in deterministic order, and exposed through `GET /recipes/{id}` without introducing upload/edit behavior.

---

# Scope

In scope

- recipe image metadata model (`id`, `recipe_id`, `blob_url`, `caption`, `position`, `created_at`)
- ordered image retrieval for recipe detail
- recipe detail contract integration (`images: [{ url, caption, position }]`)
- persistence constraints and indexes that protect ordering and query performance
- tests for ordering, nullability, and missing-image scenarios

Out of scope

- image upload endpoint (Spec 011)
- image deletion/editing endpoints
- image transformations and CDN behavior
- moderation workflows

---

# Planning Notes

- `docs/architecture.md` currently names `ImageDto.order`, while this PRD uses `position`. This spec should treat the PRD as source of truth and standardize on `position` for backend contracts.
- The codebase already includes recipe detail and image-related types from earlier slices; this spec should still formalize and verify the image-specific guarantees (ordering, constraints, and payload behavior).
- Image files stay in blob storage; only metadata is persisted and queried from PostgreSQL.
- The PRD open question says max image count should be enforced. This spec should define a backend limit now (constant/config + persistence guard where feasible), even though upload is implemented later.

---

# Phase 1 - Contract And Behavior Alignment

Define exact image semantics before schema or query changes.

Work

- Confirm the recipe detail image contract:
  - `url`
  - `caption`
  - `position`
- Define ordering rule as `position ASC`.
- Define tie-break behavior if duplicate positions exist in existing data.
  Recommendation: `position ASC`, then `id ASC`.
- Define empty-state behavior: recipes with no images return `images: []` (not `null`).
- Confirm caption is optional and may be `null`.
- Set image-position rules for future writes:
  - `position >= 1`
  - optional upper bound tied to max image count decision

Outcome

- Image payload and ordering behavior are unambiguous and testable.

---

# Phase 2 - Domain And Data Model Hardening

Align the domain model with PRD requirements and upcoming write-path needs.

Work

- Add or verify `RecipeImage` entity fields and nullability against the PRD.
- Add or verify `Recipe -> Images` relationship in the domain model.
- Define image count and position constraints in a shared location (for later reuse by upload workflows).
- Ensure naming consistency:
  - persistence field: `blob_url`
  - API field: `url`

Outcome

- Domain representation is explicit, stable, and compatible with future image-management specs.

---

# Phase 3 - EF Core Mapping, Schema, And Seed Coverage

Ensure persistence enforces image integrity and supports fast ordered lookups.

Work

- Add or verify `recipe_images` mapping with required fields:
  - required `recipe_id`
  - required `blob_url`
  - optional `caption`
  - required `position`
  - required `created_at`
- Add or verify relational behavior:
  - foreign key to `recipes`
  - expected delete behavior for orphan prevention
- Add or verify database support for ordered retrieval:
  - index on `(recipe_id, position)`
- Add guardrails for data quality:
  - check constraint for `position > 0`
  - uniqueness strategy for `(recipe_id, position)` or documented tie-break fallback if uniqueness is intentionally not enforced
- Add or verify migration updates when schema changes are required.
- Update seed/test data to include:
  - recipe with multiple images
  - recipe with one image
  - recipe with no images
  - image with `null` caption

Outcome

- Image metadata integrity is protected at schema level and seeded data covers critical scenarios.

---

# Phase 4 - Recipe Detail Query Integration

Integrate image retrieval into the detail read path with deterministic projection.

Work

- Add or verify image projection in recipe detail reader/query.
- Order images by `position` (plus tie-break if needed).
- Map persistence fields to API DTO fields:
  - `blob_url -> url`
  - `caption -> caption`
  - `position -> position`
- Keep query optimized:
  - `AsNoTracking()`
  - avoid N+1 behavior
  - fetch metadata only (no binary payload concerns)
- Preserve stable behavior when no images exist.

Outcome

- `GET /recipes/{id}` returns deterministic image metadata suitable for carousel rendering.

---

# Phase 5 - API Surface And Contract Validation

Confirm API behavior and OpenAPI metadata remain aligned with the feature contract.

Work

- Add or verify `RecipeDetailDto` image shape.
- Ensure `GET /recipes/{id}` response includes image list for existing recipes.
- Ensure missing recipe behavior remains `404 Not Found`.
- Add or verify OpenAPI response metadata/examples for image fields.
- Confirm endpoint remains public (no additional auth requirements for this read path).

Outcome

- Frontend clients can rely on a stable image contract in recipe detail responses.

---

# Phase 6 - Testing And Verification

Protect image behavior with focused automated coverage.

Work

- Add or verify infrastructure/query tests:
  - images returned in ascending `position`
  - `null` caption is preserved
  - empty image list returns `[]`
  - non-existent recipe returns `null` at reader layer
- Add or verify API tests:
  - detail response includes `images` array
  - ordering is preserved end-to-end
  - `404` behavior unaffected
- Add persistence tests where constraints are introduced:
  - invalid `position` rejected
  - duplicate `(recipe_id, position)` rejected if uniqueness is enforced
- Run manual smoke checks with seeded data.

Outcome

- Core image behavior is covered against regressions and contract drift.

---

# Phase 7 - Performance And Observability Validation

Validate non-functional requirements for image retrieval.

Work

- Verify detail query SQL shape remains efficient and avoids N+1 patterns.
- Measure metadata retrieval path against PRD target (`< 50ms`) on representative local data.
- Confirm request tracing is visible through existing OpenTelemetry instrumentation.
- Spot check payload size impact from multi-image recipes.

Outcome

- Image retrieval is observable and aligned with MVP performance expectations.

---

# Suggested Implementation Order

1. Finalize contract decisions (`position`, ordering, empty-state behavior).
2. Harden domain rules and schema constraints for image metadata.
3. Apply EF mapping/migration and refresh seed coverage.
4. Implement or refine detail query projection for ordered images.
5. Confirm API contract exposure and OpenAPI metadata.
6. Add automated tests and run manual smoke verification.
7. Validate query performance and observability signals.

---

# Completion Criteria

The feature is complete when:

- image metadata exists in `recipe_images` with required fields and integrity constraints
- recipe detail responses include `images` with `url`, `caption`, `position`
- image order is deterministic and stable
- recipes without images return an empty list
- query behavior avoids N+1 access patterns
- tests cover ordering, nullability, and missing-data behavior
- metadata retrieval performance is validated against the PRD target

---

# Follow-On Dependencies

This spec should leave the codebase ready for:

- Spec 011 image upload workflow (write path and URL validation)
- admin image management capabilities in future specs
- continued reuse of `position` semantics across listing thumbnails and detail carousels
