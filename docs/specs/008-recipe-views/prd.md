# PRD - 008 Recipe View Tracking API

# A) Problem Statement

Coffee Codex displays a brew count for each recipe to indicate how often a recipe has been viewed.

The backend must provide an API endpoint that increments the brew count whenever a recipe is opened.

This feature introduces lightweight engagement tracking without requiring user authentication or analytics systems.

---

# B) Goals

- increment recipe brew count
- expose brew count in recipe APIs
- ensure fast write operations

---

# C) Non-Goals

- detailed analytics tracking
- user-specific view tracking
- popularity ranking

---

# D) Target Consumers

Primary

- Angular frontend

Future

- analytics systems

---

# E) Assumptions

- brew_count stored in recipe table
- write operations are lightweight
- view endpoint called by frontend

---

# F) User Journey

1. Frontend calls view endpoint
2. Backend increments brew_count
3. Updated value stored in database

---

# G) Functional Requirements

## Endpoint

`POST /recipes/{id}/view`

`id="yn9v4x"`

---

## Operation

Endpoint increments brew_count:

`brew_count = brew_count + 1`

`id="ns0o66"`

---

## Validation

If recipe does not exist:

Return 404.

---

## Idempotency

Endpoint does not require strict idempotency.

Multiple calls may increase count.

---

## Integration

Updated brew count must appear in:

- recipe listing endpoint
- recipe detail endpoint

---

# H) Non-Functional Requirements

Performance

- update query < 50ms

Scalability

- must support frequent writes

Maintainability

- logic isolated in service layer

---

# I) User Stories

## Story: Increment Brew Count

As a frontend client
I want to increment brew count
So that the system reflects recipe popularity.

Acceptance Criteria

Given recipe exists
When endpoint called
Then brew_count increments.

---

# J) Out of Scope

- per-user view tracking
- analytics dashboards
- popularity ranking algorithms

---

# K) Milestones

MVP

- endpoint implemented
- database update implemented
- integration with recipe APIs

---

# L) Success Metrics

Performance

- endpoint latency < 50ms

Reliability

- error rate < 1%

---

# M) Risks & Mitigations

Risk
High write frequency could affect performance.

Mitigation
Use efficient indexed queries.

---

# N) Open Questions

- Should view increments be batched? Yes.
