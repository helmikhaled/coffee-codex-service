# PRD - 003 Recipe Detail API

# A) Problem Statement

The Coffee Codex frontend requires a backend endpoint that returns complete recipe information including brewing parameters, ingredients, steps, images, and metadata.

This endpoint must provide all information required for rendering the recipe detail page in a single API response.

---

# B) Goals

- Provide `/recipes/{id}` endpoint
- Return complete recipe information
- Include brew specifications
- Include ingredients and steps
- Include recipe images
- Include author metadata

---

# C) Non-Goals

- recipe editing
- recipe submission
- analytics dashboards
- recommendation engines

---

# D) Target Consumers

Primary

- Angular frontend

Future

- mobile clients
- integrations

---

# E) Assumptions

- database schema defined in `architecture.md`
- recipes stored in PostgreSQL
- images stored in blob storage
- EF Core used for persistence

---

# F) User Journey

1. Frontend requests `/recipes/{id}`
2. Backend retrieves recipe
3. Backend retrieves related entities
4. Backend maps entities to DTO
5. Response returned to frontend

---

# G) Functional Requirements

## Endpoint

```

GET /recipes/{id}

```

---

## Response Model

```

RecipeDetailDto

```

Fields:

```

Id
Slug
Title
Description
Category
BrewCount
Author
BrewSpecs
Ingredients
Steps
Images
Tags

```

---

## Brew Specifications

Return:

```

coffeeDoseInGrams
coffeeYieldInGrams
milkInMl
cupSizeInMl
difficulty
timeInMinutes

```

Fields may be null for non-espresso drinks.

---

## Ingredients

Return ordered list.

Fields:

```

name
quantityValue
unit
position

```

---

## Steps

Return ordered list.

Fields:

```

stepNumber
instruction

```

---

## Images

Return ordered list.

Fields:

```

url
caption
position

```

---

## Database Query

Backend must retrieve:

- recipe
- brew specs
- ingredients
- steps
- images
- tags
- author

Queries should avoid N+1 issues.

---

# H) Non-Functional Requirements

Performance

- query < 100ms

Reliability

- return 404 if recipe does not exist

Maintainability

- clean architecture layering

---

# I) User Stories

## Story: Fetch Recipe Details

As a frontend client  
I want complete recipe information  
So that I can render the recipe page.

Acceptance Criteria

Given recipe exists  
When API called  
Then full recipe data returned.

---

## Story: Handle Missing Recipes

As a frontend client  
I want meaningful error responses  
So that UI can show error state.

Acceptance Criteria

Given recipe does not exist  
When API called  
Then response is 404.

---

# J) Out of Scope

- recipe editing
- admin operations
- analytics

---

# K) Milestones

MVP

- endpoint implemented
- DTO mapping implemented
- optimized query implemented

---

# L) Success Metrics

Performance

- response time < 100ms

Reliability

- API error rate < 1%

---

# M) Risks & Mitigations

Risk  
Large join queries may degrade performance.

Mitigation  
Use efficient EF queries and indexing.

---

# N) Open Questions

- Should recipe view tracking occur here or separate endpoint? Should track here.
- Should caching be implemented? No.
