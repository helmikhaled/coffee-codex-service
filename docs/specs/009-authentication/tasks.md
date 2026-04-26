# Tasks - 009 Authentication

Tasks must be executed sequentially.

---

# Task 1 - Finalize The Authentication Contract [ ]

Lock the MVP behavior before implementation starts.

Define:

- identity provider: Auth0
- token type: JWT bearer access token
- protected surface for this spec: `GET /admin/auth-check`
- success response for the protected probe: `204 No Content`
- failure response for missing, malformed, expired, wrong-issuer, and wrong-audience tokens: `401 Unauthorized`
- public endpoints that must remain anonymous:
  - `GET /recipes`
  - `GET /recipes/{id}`
  - `GET /recipes/random`
  - `POST /recipes/{id}/view`
- authorization scope for MVP: authenticated admin access only, no RBAC

Verify:

- contract matches `docs/specs/009-authentication/prd.md`
- contract stays aligned with `docs/vision.md` and `docs/architecture.md`
- task boundary does not absorb `010-admin-recipes` or `011-image-upload`

---

# Task 2 - Tighten The Auth0 Settings Model [ ]

Turn the existing placeholder settings object into a clear API-auth configuration contract.

Update:

- `src/CoffeeCodex.API/Authentication/Auth0Settings.cs`

Implementation requirements:

- keep `Domain` and `Audience` as the required inputs for request authentication
- retain `ClientId` and `ClientSecret` only if they are intentionally preserved for future Auth0-related flows
- add any derived or helper values needed to normalize the expected issuer/authority shape

Verify:

- the settings type clearly expresses what the API needs to validate bearer tokens
- no unnecessary authentication details leak into Application or Infrastructure layers

---

# Task 3 - Add Startup Validation For Authentication Settings [ ]

Fail fast when authentication configuration is incomplete or invalid.

Add:

- startup options-validation helper(s) under `src/CoffeeCodex.API/Authentication/`

Update:

- `src/CoffeeCodex.API/Program.cs`

Validation requirements:

- reject blank or missing Auth0 domain values
- reject blank or missing audience values
- normalize the configured domain into a deterministic authority/issuer form
- keep secrets out of exception messages beyond what is needed to identify the missing setting

Verify:

- the API fails during boot when required auth settings are not configured
- configuration errors are centralized instead of surfacing as late request-time failures

---

# Task 4 - Align Appsettings With The Validated Auth Contract [ ]

Keep repository configuration files consistent with the runtime auth model.

Update:

- `src/CoffeeCodex.API/appsettings.json`
- `src/CoffeeCodex.API/appsettings.Development.json`

Implementation requirements:

- keep placeholder development and default values aligned with the validated Auth0 settings contract
- avoid documenting or exposing secret values beyond placeholders
- make it obvious which values must be supplied for JWT validation to work

Verify:

- local and default configuration shape matches the startup validation rules

---

# Task 5 - Register JWT Bearer Authentication [ ]

Wire the API to validate Auth0-issued bearer tokens.

Update:

- `src/CoffeeCodex.API/Program.cs`
- `src/CoffeeCodex.API/Authentication/` (new auth registration helper if needed)

Implementation requirements:

- use the official Auth0-compatible ASP.NET integration path
- enforce:
  - token signature validation
  - expiration validation
  - issuer validation
  - audience validation
- keep the registration isolated to the API layer

Verify:

- protected requests can be authenticated without introducing custom token-parsing logic in controllers

---

# Task 6 - Register The Shared Admin Authorization Policy [ ]

Create one reusable authorization policy for current and future admin endpoints.

Add:

- policy-name helper or equivalent under `src/CoffeeCodex.API/Authentication/`

Update:

- `src/CoffeeCodex.API/Program.cs`

Policy requirements:

- require an authenticated user
- do not introduce role, scope, or claim requirements in this MVP slice

Verify:

- the policy can be reused directly by `010-admin-recipes` and `011-image-upload`

---

# Task 7 - Add Authentication And Authorization Middleware [ ]

Insert the new security pipeline into request processing.

Update:

- `src/CoffeeCodex.API/Program.cs`

Implementation requirements:

- add `UseAuthentication()`
- add `UseAuthorization()`
- keep middleware ordering compatible with controllers, CORS, health checks, and current routing

Verify:

- protected endpoints are intercepted before controller execution
- public endpoints do not start requiring tokens accidentally

---

# Task 8 - Add The Protected Admin Probe Endpoint [ ]

Expose a minimal protected API surface that proves the auth pipeline end-to-end.

Add:

- `src/CoffeeCodex.API/Authentication/AuthCheckController.cs` or equivalent

Implementation requirements:

- route: `GET /admin/auth-check`
- apply the shared admin authorization policy
- return `204 No Content` on success
- avoid database access and recipe-domain behavior

Verify:

- the endpoint is narrow, reusable for smoke checks, and independent of future CRUD work

---

# Task 9 - Add Protected Endpoint Metadata And OpenAPI Security Wiring [ ]

Make the protected surface discoverable in development tooling.

Update:

- `src/CoffeeCodex.API/Authentication/AuthCheckController.cs` or equivalent
- `src/CoffeeCodex.API/Program.cs`

Implementation requirements:

- add endpoint metadata for:
  - `204 No Content`
  - `401 Unauthorized`
- add bearer-auth OpenAPI metadata for the protected endpoint
- do not expose secrets or raw token examples in generated documentation

Verify:

- development API documentation shows that the admin probe requires a bearer token

---

# Task 10 - Preserve Anonymous Public Endpoint Behavior [ ]

Ensure the new auth setup does not change the recipe discovery contract.

Update:

- `src/CoffeeCodex.API/Recipes/RecipesController.cs` only if explicit anonymous metadata is required
- `src/CoffeeCodex.API/Program.cs` only if authorization defaults need adjustment

Implementation requirements:

- keep recipe listing, detail, random, and brew-tracking endpoints public
- keep health-check reachability aligned with current operational expectations
- avoid broad fallback authorization rules unless public routes are explicitly protected from regression

Verify:

- existing public API semantics stay unchanged apart from the new protected admin surface being added

---

# Task 11 - Create JWT Test Utilities And Auth-Capable Test Host Support [ ]

Make the auth slice testable without depending on live Auth0 infrastructure.

Add:

- authentication-focused test helpers under `tests/CoffeeCodex.RecipeListing.Tests/Api/`

Update:

- existing or new API test factory in `tests/CoffeeCodex.RecipeListing.Tests/Api/`

Implementation requirements:

- generate signed JWTs locally for tests
- configure the test host so JWT validation still checks signature, issuer, audience, and expiration
- keep the test approach close to production behavior rather than bypassing the middleware with a fake success handler

Verify:

- integration tests can issue valid and intentionally invalid bearer tokens deterministically

---

# Task 12 - Add Authentication Configuration Tests [ ]

Protect the fail-fast startup behavior for auth settings.

Add:

- authentication configuration tests under `tests/CoffeeCodex.RecipeListing.Tests/`

Cover:

- missing domain fails configuration validation
- missing audience fails configuration validation
- valid configuration can boot successfully

Verify:

- startup validation is covered independently from endpoint behavior

---

# Task 13 - Add Unauthorized Integration Tests For The Protected Endpoint [ ]

Protect the core rejection paths required by the PRD.

Add or update:

- authentication endpoint tests under `tests/CoffeeCodex.RecipeListing.Tests/Api/`

Cover:

- `GET /admin/auth-check` returns `401` with no token
- `GET /admin/auth-check` returns `401` with a malformed bearer token
- `GET /admin/auth-check` returns `401` with the wrong issuer
- `GET /admin/auth-check` returns `401` with the wrong audience
- `GET /admin/auth-check` returns `401` with an expired token

Verify:

- unauthorized requests are rejected before controller business logic runs

---

# Task 14 - Add Valid-Token Integration Test For The Protected Endpoint [ ]

Protect the success path for authenticated admin access.

Add or update:

- authentication endpoint tests under `tests/CoffeeCodex.RecipeListing.Tests/Api/`

Cover:

- `GET /admin/auth-check` returns `204` when a valid bearer token is supplied

Verify:

- the middleware accepts correctly signed and correctly targeted tokens

---

# Task 15 - Add Anonymous Public-Endpoint Regression Tests [ ]

Prove that authentication only applies to admin APIs.

Add or update:

- `tests/CoffeeCodex.RecipeListing.Tests/Api/RecipesEndpointTest.cs`

Cover:

- `GET /recipes` still succeeds without a token
- `GET /recipes/{id}` still succeeds without a token
- `GET /recipes/random` still succeeds without a token
- `POST /recipes/{id}/view` still succeeds without a token

Verify:

- public recipe behavior remains aligned with `docs/vision.md`

---

# Task 16 - Run Focused Automated Verification [ ]

Run the repository tests that cover the authentication slice and the affected public API regressions.

Execute:

- `dotnet test .\tests\CoffeeCodex.RecipeListing.Tests\CoffeeCodex.RecipeListing.Tests.csproj`

Verify:

- new authentication tests pass
- existing public recipe tests remain green

---

# Task 17 - Run Runtime Authentication Smoke Checks [ ]

Perform manual end-to-end checks against the running API.

Validate:

- `GET /admin/auth-check` without a token returns `401`
- `GET /admin/auth-check` with an invalid token returns `401`
- `GET /admin/auth-check` with a valid token returns `204`
- representative public recipe endpoints still return successful anonymous responses

Verify:

- the implemented auth contract matches the documented plan and PRD

---

# Task 18 - Validate Observability And Handoff Readiness [ ]

Confirm the feature is ready for dependent specs.

Validate:

- authorized and unauthorized requests are captured by existing OpenTelemetry ASP.NET tracing
- no secrets or raw tokens appear in logs, errors, or docs
- the shared admin policy can be applied directly by the recipe-management and image-upload specs

Verify:

- the auth slice is ready for handoff to `010-admin-recipes` and `011-image-upload`

---

# Completion

Authentication is complete when:

- JWT bearer validation is active for protected endpoints
- missing or invalid tokens return `401 Unauthorized`
- `GET /admin/auth-check` succeeds only with a valid bearer token
- public recipe endpoints remain anonymous
- configuration fails fast when required Auth0 settings are missing
- automated tests cover both rejection and success paths
