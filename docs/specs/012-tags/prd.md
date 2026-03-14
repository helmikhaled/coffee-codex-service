# PRD - 012 Recipe Tags API

# A) Problem Statement

Recipes in Coffee Codex require flexible labeling beyond high-level categories. Tags allow recipes to be associated with characteristics such as ingredients or styles.

Tags enable improved discovery through filtering and allow recipes to share common attributes across categories.

This feature introduces backend support for storing and retrieving recipe tags.

---

# B) Goals

- support tagging recipes
- retrieve tags in recipe APIs
- support filtering recipes by tag
- maintain efficient database queries

---

# C) Non-Goals

- user-created tags
- tag moderation workflows
- tag popularity analytics

---

# D) Target Consumers

Primary

- Angular frontend

Future

- search systems
- recommendation engines

---

# E) Assumptions

- tags stored in database
- recipes linked to tags through join table
- recipe listing endpoint supports tag filtering

---

# F) User Journey

1. Admin assigns tags when creating recipe
2. Backend stores tag associations
3. Recipe APIs return tag metadata
4. Frontend uses tags for filtering

---

# G) Functional Requirements

## Tag Entity

Tags must contain:

```

id
name
slug
created_at

```

---

## Recipe Tag Association

Recipes must support many-to-many relationship with tags.

Join table:

```

recipe_tags

```

Fields:

```

recipe_id
tag_id

```

---

## Retrieve Tags

Recipe detail endpoint must include tags.

Example:

```

tags: [
{ name: "matcha" },
{ name: "citrus" }
]

```

---

## Filter Recipes by Tag

Recipe listing endpoint must support tag filtering.

Example:

```

GET /recipes?tag=matcha

```

---

## Tag Management

Admin recipe creation endpoint must accept tag IDs.

Example request:

```

tags: ["matcha", "citrus"]

```

---

# H) Non-Functional Requirements

Performance

- tag filtering query < 100ms

Scalability

- support many recipes per tag

Maintainability

- tag logic isolated in domain layer

---

# I) User Stories

## Story: Associate Tags with Recipes

As an admin  
I want to assign tags to recipes  
So that drinks are categorized.

Acceptance Criteria

Tags stored successfully.

---

## Story: Retrieve Recipe Tags

As a frontend client  
I want recipe tags returned  
So that the UI can display them.

Acceptance Criteria

Tags appear in recipe detail response.

---

## Story: Filter by Tag

As a frontend client  
I want to retrieve recipes by tag  
So that users can discover related drinks.

Acceptance Criteria

Filtered results returned.

---

# J) Out of Scope

- tag analytics
- tag editing endpoints

---

# K) Milestones

MVP

- tag entity
- recipe-tag association
- tag filtering

---

# L) Success Metrics

Performance

- tag filtering latency

Reliability

- tag association accuracy

---

# M) Risks & Mitigations

Risk  
Uncontrolled tag growth may cause inconsistent taxonomy.

Mitigation  
Restrict tag creation to admin.

---

# N) Open Questions

- Should tag synonyms be supported? Yes.
