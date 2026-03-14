# Coffee Codex

Coffee Codex is a curated digital catalog of modern coffee drinks.

This repository contains the **backend API** powering the Coffee Codex application.

The project is also a practical exploration of **human-led, AI-driven development**.

---

## Project Purpose

Modern AI tools can generate large amounts of code quickly.

However, without clear intent, this often leads to:

- inconsistent architecture
- unclear system boundaries
- unnecessary complexity

Coffee Codex is built using a different approach:

> Humans define intent.  
> AI accelerates execution.

Before writing code, the system is defined through structured documentation.

---

## Technology Stack

Backend is built with:

- .NET 10 Web API
- Clean Architecture
- PostgreSQL
- Entity Framework Core
- Blob Storage for images

---

## Repository Structure

```

docs/
vision.md
architecture.md
specs/

```

Each feature is defined as a **Product Requirement Document (PRD)**.

Example:

```

002-recipe-listing
003-recipe-detail
004-recipe-images

```

Every feature follows the same lifecycle:

```

PRD
↓
Plan
↓
Tasks
↓
Implementation

```

---

## Why the First Commit Has No Code

The initial commit intentionally contains only:

- product vision
- architecture decisions
- feature specifications

This reflects the philosophy behind the project:

Architecture and intent should exist **before implementation**.

---

## Related Repository

Frontend client: [coffee-codex-client](https://github.com/helmikhaled/coffee-codex-client)

---

## Development Workflow

Each feature is implemented as a vertical slice.

Example progression:

```

001 app shell
002 recipe listing
003 recipe detail

```

AI tools are used to accelerate implementation **after the intent is clearly defined**.

---

## Follow the Journey

The development process and reasoning behind this project are documented in a series of technical posts exploring:

- intent-driven development
- AI-assisted engineering
- structured feature delivery

The repositories will evolve feature by feature.
