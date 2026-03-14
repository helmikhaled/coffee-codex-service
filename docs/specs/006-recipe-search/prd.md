# PRD - 006 Recipe Search API

# A) Problem Statement

As the Coffee Codex recipe library grows, users need the ability to search for specific drinks or ingredients.

The backend must support keyword search against recipe data so the frontend can retrieve matching recipes.

Search functionality must remain efficient while supporting pagination and curated ordering.

---

# B) Goals

- Extend recipe listing endpoint with search capability
- Support keyword matching on recipe title
- Support keyword matching on tags
- Maintain pagination compatibility

---

# C) Non-Goals

- advanced ranking algorithms
- semantic search
- AI search
- typo correction

---

# D) Target Consumers

Primary

- Angular frontend

Future

- mobile clients
- integrations

---

# E) Assumptions

- PostgreSQL used as database
- EF Core used for persistence
- recipes contain searchable text fields

---

# F) User Journey

1. Frontend sends request with search query
2. Backend constructs search query
3. Backend filters matching recipes
4. Results returned in paginated format

---

# G) Functional Requirements

## Endpoint

Search extends existing endpoint:

```

GET /recipes

```

---

## Query Parameter

Search parameter:

```

search

```

Example:

```

GET /recipes?search=matcha

```

---

## Search Fields

Search must match:

- recipe title
- tags

Future versions may include ingredient search.

---

## Ordering

Search results should still respect curated ordering when possible.

```

ORDER BY display_order ASC

```

If ranking becomes necessary in future versions, ordering logic may change.

---

## Pagination

Search results must support pagination parameters:

```

page
pageSize

```

---

# H) Non-Functional Requirements

Performance

- search query < 150ms

Scalability

- support growing recipe dataset

Maintainability

- modular query logic

---

# I) User Stories

## Story: Search Recipes

As a frontend client  
I want to retrieve recipes matching a query  
So that users can find drinks quickly.

Acceptance Criteria

Given search parameter  
When API called  
Then matching recipes returned.

---

# J) Out of Scope

- fuzzy search
- synonyms
- semantic search

---

# K) Milestones

MVP

- search parameter implemented
- database query implemented
- integrated with recipe listing endpoint

---

# L) Success Metrics

Performance

- search latency < 150ms

Reliability

- error rate < 1%

---

# M) Risks & Mitigations

Risk  
Full table scans may degrade performance.

Mitigation

- add database index on searchable fields

---

# N) Open Questions

- Should ingredient names be searchable? Yes.
- Should search ranking prioritize title matches? Yes.
