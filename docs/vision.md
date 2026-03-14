# Coffee Codex — Backend Vision

Repository: coffee-codex-service

Coffee Codex is a curated digital library of modern coffee drink recipes designed for home baristas.

The backend provides the API and data foundation that powers recipe discovery, recipe viewing, and administrative management of recipes.

The backend prioritizes:

- simplicity
- maintainability
- predictable APIs
- clean architecture

---

# Backend Responsibilities

The backend is responsible for:

- storing recipes
- serving recipe data
- managing authors
- handling recipe images
- tracking recipe views ("brews")
- supporting discovery features

The backend does not contain presentation logic.

Frontend applications consume the backend through REST APIs.

---

# Product Scope (MVP)

The MVP focuses on **recipe discovery and viewing**.

Supported features:

- list recipes
- view recipe details
- filter recipes
- search recipes
- retrieve random recipe
- track recipe views

Recipes are **admin-created only**.

There is:

- no public recipe submission
- no comments
- no likes

Authentication is required only for **admin APIs**.

---

# Backend Technology Stack

The backend uses modern .NET architecture.

Core technologies:

- .NET 10 Web API
- Clean Architecture
- Entity Framework Core
- PostgreSQL
- Blob Storage
- FluentValidation
- OpenTelemetry
- Auth0

---

# Architecture

The backend follows **Clean Architecture with Domain-Driven Design principles**.

Project layers:

Domain

- core entities
- business rules

Application

- use cases
- commands and queries
- validation

API

- controllers
- request and response models

Infrastructure

- EF Core persistence
- blob storage integration

Shared

- cross-cutting utilities

---

# Core Capabilities

The backend provides APIs for:

Recipe discovery

```

GET /recipes
GET /recipes/{id}
GET /recipes/random

```

Recipe filtering and search

```

GET /recipes?category=
GET /recipes?search=
GET /recipes?tag=

```

Admin management

```

POST /recipes
PUT /recipes/{id}
DELETE /recipes/{id}

```

Image uploads

```

POST /recipes/{id}/images

```

---

# Data Storage

The primary database is PostgreSQL.

Data is accessed through Entity Framework Core.

Images are stored in blob storage.

Only image metadata is stored in the database.

---

# Observability

The backend integrates OpenTelemetry for:

- request tracing
- metrics
- structured logging

This enables monitoring and diagnostics.

---

# Security

Authentication uses Auth0.

For MVP:

- public APIs are open
- admin APIs require authentication

Authorization will be minimal initially.

---

# Performance Goals

The backend should provide fast responses for browsing.

Targets:

- recipe list queries < 100ms
- recipe detail queries < 100ms
- efficient pagination
- lightweight payloads

---

# Future Direction

Future backend capabilities may include:

- contributor accounts
- recipe submissions
- moderation workflows
- curated collections
- advanced search

These are outside MVP scope but supported by the architecture.

---
