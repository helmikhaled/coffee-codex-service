# Tasks - 004 Recipe Images

Tasks must be executed sequentially.

---

# Task 1 - Finalize Image Contract Semantics [x]

Lock the image contract decisions before code changes.

Define:

- `images` payload shape in recipe detail as `{ url, caption, position }`
- ordering rule: `position ASC`
- tie-break rule: `position ASC`, then `id ASC`
- empty-state rule: `images: []` (never `null`)

Verify:

- PRD and plan requirements are fully represented
- `position` is used consistently (not `order`) in backend contracts

---

# Task 2 - Define Image Constraints For Future Writes [x]

Set reusable image rules now so later upload work (Spec 011) does not duplicate logic.

Define:

- minimum position rule (`position >= 1`)
- maximum image count per recipe constant
- caption optionality rule (`caption` may be `null`)

Verify:

- constraints are placed in a shared location reusable by Application and Infrastructure

---

# Task 3 - Align Domain Model For Recipe Images [x]

Ensure domain entities reflect the finalized image contract and constraints.

Update/verify:

- `RecipeImage` fields and nullability
- `Recipe.Images` relationship
- naming consistency between persistence (`blob_url`) and API output (`url`)

Verify:

- domain model can represent all required image metadata

---

# Task 4 - Align Recipe Detail DTO Image Shape [x]

Ensure application-level detail DTOs expose the finalized image contract.

Update/verify:

- `ImageDto` fields: `url`, `caption`, `position`
- no lingering `order` naming in recipe detail DTOs

Verify:

- DTO contract matches PRD and frontend expectations

---

# Task 5 - Enforce Recipe Image Schema Constraints [x]

Apply persistence rules that protect image ordering and data integrity.

Update `RecipeImage` entity configuration to include:

- required `recipe_id`
- required `blob_url`
- optional `caption`
- required `position`
- required `created_at`
- index on `(recipe_id, position)`
- check constraint for `position > 0`
- uniqueness strategy for `(recipe_id, position)` (or explicit non-unique decision)

Verify:

- EF Core model builds with all constraints

---

# Task 6 - Add Or Update EF Core Migration [x]

Persist schema changes introduced by Task 5.

Implement:

- migration for recipe image constraints/index updates
- snapshot update

Verify:

- migration applies cleanly to a local database

---

# Task 7 - Expand Seed Data For Image Scenarios [x]

Ensure local and test data covers required image behaviors.

Add/verify seed cases:

- recipe with multiple images
- recipe with one image
- recipe with no images
- image with `null` caption

Verify:

- seeded data supports ordering and empty-state verification

---

# Task 8 - Implement Deterministic Image Projection In Detail Reader [x]

Make recipe detail retrieval return correctly ordered image metadata.

Update/verify detail query behavior:

- `AsNoTracking()`
- projection to `ImageDto`
- ordering by `position`, then `id`
- mapping `blob_url -> url`

Verify:

- no N+1 behavior is introduced
- recipes without images return an empty list

---

# Task 9 - Keep API Detail Surface Aligned [x]

Ensure the public endpoint continues to expose the correct image payload.

Update/verify:

- `GET /recipes/{id}` response includes ordered `images`
- OpenAPI annotations/documentation reflect image fields
- not-found behavior remains `404`

Verify:

- endpoint remains publicly accessible

---

# Task 10 - Add Infrastructure Tests For Image Retrieval [x]

Protect image query behavior at the reader level.

Add tests for:

- ordered images by `position`
- tie-break behavior for equal positions (if non-unique positions are allowed)
- `null` caption preservation
- empty image list for recipes with no images

Verify:

- all infrastructure tests pass

---

# Task 11 - Add Persistence Constraint Tests [x]

Protect schema-level guarantees introduced for image metadata.

Add tests for:

- rejection of invalid `position` values (`<= 0`)
- duplicate `(recipe_id, position)` behavior according to chosen uniqueness strategy

Verify:

- constraint behavior is deterministic across test runs

---

# Task 12 - Add API Integration Tests For Image Contract [x]

Protect end-to-end HTTP contract behavior for recipe images.

Add tests for:

- `GET /recipes/{id}` returns `images` array in correct order
- image objects include `url`, `caption`, `position`
- recipes without images return `images: []`

Verify:

- all API integration tests pass

---

# Task 13 - Run Focused Automated Test Suite [x]

Run and validate all tests impacted by this feature.

Execute:

- Application/Infrastructure/API test projects related to recipe detail and images

Verify:

- all targeted tests are green
- no regressions in existing listing/detail behavior

---

# Task 14 - Run Manual API Smoke Checks [x]

Confirm runtime behavior with seeded data.

Validate manually:

- existing recipe with multiple images
- recipe with no images
- stable ordering behavior in response JSON

Verify:

- payload matches the finalized image contract

---

# Task 15 - Validate Performance And Observability [x]

Confirm non-functional requirements from PRD and plan.

Validate:

- image metadata retrieval path is within `< 50ms` target on representative local data
- request is captured by OpenTelemetry instrumentation
- query shape remains efficient and avoids unnecessary joins

Verify:

- performance and observability checks are documented in implementation notes

---

# Completion

Recipe images are complete for this spec when:

- recipe detail returns ordered image metadata using `position`
- schema constraints protect image ordering integrity
- empty and nullable image scenarios are handled correctly
- automated tests protect image retrieval behavior end-to-end
- performance and telemetry checks are completed
