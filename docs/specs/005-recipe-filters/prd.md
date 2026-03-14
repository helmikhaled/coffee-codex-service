# PRD - 005 Recipe Filters API

# A) Problem Statement

As Coffee Codex grows, the backend must support retrieving recipes filtered by category or tags.

Without filtering support, users would need to scroll through the entire recipe library to find relevant drinks.

This feature extends the recipe listing API to support **filtering parameters**.

---

# B) Goals

- Support category filtering
- Support tag filtering
- Maintain pagination compatibility
- Ensure efficient database queries

---

# C) Non-Goals

- advanced search ranking
- recommendation systems
- full-text search

Search functionality will be implemented in Spec 006.

---

# D) Target Consumers

Primary

- Angular frontend

Future

- mobile clients

---

# E) Assumptions

- categories stored in recipe table
- tags stored in tag tables
- pagination implemented in recipe listing

---

# F) User Journey

1. Frontend sends request with filter parameters
2. Backend constructs query
3. Filter conditions applied
4. Results sorted by display_order
5. Paginated results returned

---

# G) Functional Requirements

## Endpoint

Filtering extends existing endpoint:

```

GET /recipes

```

---

## Category Filtering

Example request:

```

GET /recipes?category=Modern

```

Query must return only recipes in selected category.

---

## Tag Filtering

Example request:

```

GET /recipes?tag=matcha

```

Query must return recipes associated with the tag.

---

## Combined Filtering

Example request:

```

GET /recipes?category=Modern&tag=matcha

```

Filtering logic must support combining parameters.

---

## Ordering

Filtered results must still respect curated ordering.

```

ORDER BY display_order ASC

```

---

## Pagination

Pagination parameters must continue working:

```

page
pageSize

```

---

# H) Non-Functional Requirements

Performance

- filtered query < 100ms

Scalability

- support growing dataset

Maintainability

- modular query construction

---

# I) User Stories

## Story: Filter Recipes by Category

As a frontend client  
I want to retrieve recipes by category  
So that users can browse specific drink types.

Acceptance Criteria

Given category filter  
When API called  
Then only matching recipes returned.

---

## Story: Filter Recipes by Tag

As a frontend client  
I want to retrieve recipes by tag  
So that users can find drinks with specific ingredients.

Acceptance Criteria

Given tag filter  
When API called  
Then only matching recipes returned.

---

# J) Out of Scope

- full-text search
- ranking algorithms
- personalization

---

# K) Milestones

MVP

- category filter implemented
- tag filter implemented
- integrated with listing endpoint

---

# L) Success Metrics

Performance

- filtered query execution < 100ms

Reliability

- error rate < 1%

---

# M) Risks & Mitigations

Risk  
Complex join queries when filtering by tag.

Mitigation  
Ensure proper indexing on tag tables.

---

# N) Open Questions

- Should filtering support multiple tags? Yes.
- Should categories be case-insensitive? No.
