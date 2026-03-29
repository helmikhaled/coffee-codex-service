# Tasks - 002 Recipe Listing API

Tasks must be executed sequentially.

---

# Task 1 - Define Listing Contracts [x]

Finalize the recipe listing contract in code.

Define:

- `RecipeSummaryDto`
- `PagedResponse<T>`
- pagination defaults and maximum page size
- stable ordering rule: `display_order ASC`, then `id ASC`

Verify:

- all fields from `architecture.md` are represented
- total count is included in the paged response

---

# Task 2 - Add Recipe Domain Types [x]

Create the minimum domain model required for recipe listing.

Add:

- `Recipe`
- `Author`
- `RecipeImage`
- `RecipeBrewSpecs`
- `RecipeCategory`
- `DifficultyLevel`

Include:

- `display_order` on `Recipe`
- relationships needed for author, brew specs, and images

Verify:

- the domain model can represent every field needed by `RecipeSummaryDto`

---

# Task 3 - Configure EF Core Entities [x]

Expand persistence to support recipe listing.

Update:

- `CoffeeCodexDbContext`

Add entity configuration for:

- `recipes`
- `authors`
- `recipe_brew_specs`
- `recipe_images`

Include:

- keys and relationships
- `display_order` column mapping
- image `position` mapping

Verify:

- the EF model builds successfully

---

# Task 4 - Add Listing Indexes And Schema Support [x]

Add database support required for performant listing queries.

Create schema changes for:

- `recipes.display_order`
- index on `recipes.display_order`
- index on `recipe_images(recipe_id, position)`

If migrations are used in the repository:

- add the migration for the listing schema

Verify:

- schema changes can be applied cleanly

---

# Task 5 - Prepare Representative Recipe Data [x]

Add local data support for exercising the listing endpoint.

Prepare data that covers:

- multiple recipes
- multiple authors
- thumbnail image at `position = 1`
- recipe with no image
- overlapping `display_order`
- recipe with brew specs difficulty

Verify:

- seeded or prepared data is sufficient for pagination and ordering tests

---

# Task 6 - Add Application Query Models And Validation [x]

Create the application-layer recipe listing use case.

Add:

- `GetRecipesQuery`
- query result model using `PagedResponse<RecipeSummaryDto>`
- validator for `page` and `pageSize`

Rules:

- `page >= 1`
- `pageSize >= 1`
- `pageSize <= configured max`

Verify:

- invalid pagination inputs fail validation

---

# Task 7 - Add Listing Read Abstraction And Registration [x]

Introduce the application-to-infrastructure boundary for recipe listing reads.

Add:

- repository or query service interface for recipe summaries
- handler contract and implementation registration
- dependency injection wiring in Application and Infrastructure

Verify:

- the API layer can request recipe listings without referencing EF Core directly

---

# Task 8 - Implement The Listing Query [x]

Implement the optimized EF Core query for recipe summaries.

Requirements:

- use `AsNoTracking()`
- project directly to `RecipeSummaryDto`
- order by `display_order ASC`, then `id ASC`
- select thumbnail from `recipe_images.position = 1`
- include author name
- include difficulty from brew specs
- return total count
- apply `Skip` and `Take` for pagination

Verify:

- the query avoids N+1 behavior
- recipes without thumbnails return `null` thumbnail URLs

---

# Task 9 - Expose GET /recipes [x]

Add the public API endpoint for recipe listing.

Implement:

- query string binding for `page` and `pageSize`
- endpoint-to-application delegation
- `200 OK` response with paged recipe summaries
- `400 Bad Request` response for invalid pagination inputs

Verify:

- the endpoint is publicly accessible
- no Auth0 protection is applied to this route

---

# Task 10 - Register Endpoint And Finalize API Wiring [x]

Integrate the new recipe endpoint into the API startup path.

Update:

- endpoint registration in `Program.cs` or a dedicated endpoint module
- any required service registrations for validation and handlers

Verify:

- the application starts with the recipe listing route available

---

# Task 11 - Add Automated Tests [x]

Create coverage for the recipe listing slice.

Add tests for:

- pagination validation
- paged response includes total count
- ordering by `display_order`
- stable secondary ordering by `id`
- thumbnail selection from image `position = 1`
- null thumbnail when no image exists
- empty result set
- `GET /recipes` returns `200`
- invalid query parameters return `400`

Verify:

- all new tests pass

---

# Task 12 - Run Manual Verification [x]

Perform a final smoke check against the running API.

Validate:

- `GET /recipes?page=1&pageSize=12` returns the expected payload
- curated order is preserved
- pagination metadata is correct
- query behavior is consistent with the PRD

Confirm:

- the endpoint remains lightweight and aligned with MVP performance expectations

---

# Completion

Recipe listing is ready for the next specs when:

- the public listing endpoint is working
- pagination and total count are correct
- curated ordering is deterministic
- thumbnail selection is correct
- automated coverage protects the core behavior
