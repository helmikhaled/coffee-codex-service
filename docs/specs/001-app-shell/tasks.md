# Tasks - 001 Backend Application Shell

Tasks must be executed sequentially.

---

# Task 1 - Create Solution [x]

Create .NET solution.

Add projects

- CoffeeCodex.API
- CoffeeCodex.Application
- CoffeeCodex.Domain
- CoffeeCodex.Infrastructure
- CoffeeCodex.Shared

---

# Task 2 - Configure Clean Architecture [x]

Setup project references.

Application depends on Domain.

Infrastructure depends on Application.

API depends on Application.

---

# Task 3 - Install Dependencies [x]

Install packages

- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- FluentValidation
- OpenTelemetry
- Auth0.AspNetCore.Authentication

---

# Task 4 - Configure Dependency Injection [x]

Create extension method for service registration.

Register:

- Application services
- Infrastructure services

---

# Task 5 - Configure Database Context [x]

Create DbContext.

Add placeholder configuration.

---

# Task 6 - Configure OpenTelemetry [x]

Setup tracing for API requests.

---

# Task 7 - Configure Authentication Placeholder [x]

Add Auth0 configuration placeholders.

---

# Task 8 - Run Application [x]

Start API server.

Verify:

- server starts
- health endpoint works

---

# Completion

Backend foundation ready for next spec:

002-recipe-listing
