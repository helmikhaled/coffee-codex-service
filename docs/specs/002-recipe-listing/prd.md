# PRD - 002 Recipe Listing API

# A) Problem Statement

The Coffee Codex frontend requires a backend API that returns recipe summaries for display on the landing page.

The API must return lightweight recipe data suitable for grid rendering while maintaining the curated ordering defined by the admin.

Recipes must be returned in deterministic order using the `display_order` column.

---

# B) Goals

- Provide `/recipes` endpoint
- Return paginated recipe summaries
- Maintain curated order
- Include thumbnail images
- Include author metadata

---

# C) Non-Goals

- recipe filtering
- search
- random recipe
- admin operations

---

# D) Target Consumers

Primary

- Angular frontend

Future

- mobile apps
- integrations

---

# E) Assumptions

- PostgreSQL database
- EF Core used for persistence
- DTO contracts defined in `architecture.md`
- recipe images stored in blob storage

---

# F) User Journey

1. Frontend requests `/recipes`
2. Backend queries database
3. Recipes sorted by `display_order`
4. Backend selects thumbnail image
5. API returns paginated results
6. Frontend renders grid

---

# G) Functional Requirements

## Endpoint

```

GET /recipes

```

---

## Query Parameters

```

page
pageSize

```

Example:

```

GET /recipes?page=1&pageSize=12

```

---

## Ordering

Recipes must be ordered by:

```

display_order ASC

```

---

## Thumbnail Selection

Thumbnail must be:

```

recipe_images.position = 1

```

---

## Response Model

```

PagedResponse<RecipeSummaryDto>

```

RecipeSummaryDto:

```

Id
Slug
Title
Category
ThumbnailUrl
BrewCount
AuthorName
Difficulty

```

---

# H) Non-Functional Requirements

Performance

- query execution < 100ms

Scalability

- pagination required

Observability

- OpenTelemetry tracing

Maintainability

- clean architecture boundaries

---

# I) User Stories

## Story: Fetch Recipe List

As a frontend client  
I want to retrieve recipe summaries  
So that I can render the homepage.

Acceptance Criteria

Given recipes exist  
When API called  
Then paginated results returned.

---

## Story: Maintain Curated Order

As an admin  
I want recipes displayed in curated order  
So that the homepage reflects editorial intent.

Acceptance Criteria

Given recipes have display_order  
When API returns results  
Then they appear sorted by display_order.

---

# J) Out of Scope

- filtering
- search
- admin endpoints
- analytics

---

# K) Milestones

MVP

- API endpoint implemented
- database query implemented
- DTO mapping implemented

---

# L) Success Metrics

Performance

- API response time < 100ms

Reliability

- error rate < 1%

---

# M) Risks & Mitigations

Risk

N+1 queries when retrieving images.

Mitigation

Use optimized join query.

---

# N) Open Questions

- Should API return total count for pagination? Yes.
- Should featured recipes be supported? Yes.
