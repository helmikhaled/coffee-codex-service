# Plan - 009 Authentication

This spec adds the backend authentication foundation for Coffee Codex admin APIs without changing the existing public recipe discovery surface.

The implementation should turn the current Auth0 placeholder configuration into real JWT bearer validation, centralize admin authorization behind a reusable policy, keep public endpoints anonymous, and provide an independently verifiable protected API surface that future admin recipe and image-upload features can reuse.

---

# Scope

In scope

- Auth0-backed JWT bearer validation for backend API requests
- validation of token signature, expiration, issuer, and audience
- centralized authentication and authorization registration in the API startup path
- a reusable admin-only authorization policy for future protected endpoints
- a minimal protected API surface that proves the auth pipeline end-to-end without pulling recipe CRUD into this spec
- OpenAPI/security metadata for bearer-token usage
- automated coverage for valid-token, missing-token, invalid-token, and public-endpoint regression scenarios

Out of scope

- user registration
- user profile management
- role-based access control or per-permission policies
- frontend sign-in UX
- session storage or cookie authentication
- recipe create/update/delete business logic from `010-admin-recipes`
- image upload behavior from `011-image-upload`
- direct refresh-token handling inside the backend API

---

# Planning Notes

- `src/CoffeeCodex.API/Authentication/Auth0Settings.cs` already exists, and `Auth0.AspNetCore.Authentication` is already referenced by the API project. The missing work is wiring and validating the runtime authentication pipeline, not introducing a brand-new identity-provider integration from scratch.
- `Program.cs` currently binds `Auth0Settings` only as plain configuration. There is no `AddAuthentication(...)`, no `AddAuthorization(...)`, and no `UseAuthentication()` / `UseAuthorization()` middleware in the request pipeline.
- The current production API surface is entirely public:
  - `GET /recipes`
  - `GET /recipes/{id}`
  - `GET /recipes/random`
  - `POST /recipes/{id}/view`
- `010-admin-recipes` and `011-image-upload` both assume authentication already exists. This spec should therefore deliver shared auth infrastructure and a reusable admin policy rather than absorbing recipe-mutation or upload business logic early.
- Because no real admin endpoint exists yet, this spec should include one minimal protected probe endpoint so the auth middleware can be verified independently. Keep it narrow, response-only, and free of recipe-domain behavior.
- The API acts as a resource server. For MVP token validation, the backend needs `Domain` and `Audience`; `ClientId` and `ClientSecret` can remain in configuration for future use but should not be required for request authentication unless implementation proves the SDK path needs them.
- The PRD open question says refresh tokens should be supported. For this backend slice, that means refreshed access tokens must continue to validate correctly; refresh-token issuance and rotation remain between the frontend and Auth0, not inside this API.
- `401 Unauthorized` should be the primary failure contract for missing, malformed, expired, wrong-issuer, or wrong-audience bearer tokens. `403 Forbidden` is not a primary MVP concern because this spec does not introduce role or scope checks beyond "is this request authenticated for admin access?"
- Existing public recipe endpoints must stay open and must not start requiring tokens as a side effect of introducing admin auth.
- The current test suite lives in `tests/CoffeeCodex.RecipeListing.Tests`. Even though the project name is listing-oriented, it already hosts API integration infrastructure and can cover the authentication slice without forcing a test-project rename in this spec.

---

# Planned File Touchpoints

API

- `src/CoffeeCodex.API/Program.cs`
- `src/CoffeeCodex.API/Authentication/Auth0Settings.cs`
- `src/CoffeeCodex.API/Authentication/` (new auth registration, policy, and options-validation helpers)
- `src/CoffeeCodex.API/Authentication/` (minimal protected probe controller or equivalent protected endpoint surface)
- `src/CoffeeCodex.API/appsettings.json`
- `src/CoffeeCodex.API/appsettings.Development.json`

Existing public endpoints

- `src/CoffeeCodex.API/Recipes/RecipesController.cs` (only if explicit anonymous metadata is needed to preserve public behavior)

Tests

- `tests/CoffeeCodex.RecipeListing.Tests/Api/` (new authentication-focused integration tests)
- `tests/CoffeeCodex.RecipeListing.Tests/Api/` (auth-capable test factory or token helpers)
- `tests/CoffeeCodex.RecipeListing.Tests/` (startup/configuration tests if needed)

Project files

- no package changes are expected unless the chosen test-token strategy requires an additional first-party package already aligned with ASP.NET JWT testing

---

# Phase 1 - Lock Authentication Contract And Feature Boundary

Define exactly what this spec protects and what it intentionally leaves for follow-on specs.

Work

- confirm the authentication model:
  - Auth0 is the external identity provider
  - the backend validates bearer access tokens on each protected request
- confirm public vs protected behavior:
  - existing recipe discovery/view endpoints remain anonymous
  - admin-only endpoints require authentication
- define the MVP failure contract:
  - missing token -> `401 Unauthorized`
  - malformed/invalid token -> `401 Unauthorized`
  - expired token -> `401 Unauthorized`
  - wrong issuer/audience -> `401 Unauthorized`
- define the verification surface for this spec:
  - add one narrow protected admin probe endpoint
  - do not add recipe create/update/delete behavior yet
- document that role/scope authorization is deferred until a future spec requires finer-grained authorization

Outcome

- the auth slice has a clear delivery boundary and will not absorb adjacent admin feature scope

---

# Phase 2 - Harden Auth0 Configuration And Startup Validation

Turn placeholder settings into a validated runtime contract.

Work

- review `Auth0Settings` and keep only the settings the API truly needs for bearer validation as required inputs
- add startup validation so missing or blank authentication settings fail fast during boot instead of surfacing later as request-time misconfiguration
- normalize the Auth0 authority/issuer shape from the configured domain so token validation is deterministic
- preserve secure defaults:
  - never log secrets or raw tokens
  - do not expose `ClientSecret` through diagnostics or OpenAPI
- keep development and default appsettings aligned with the validated options contract

Outcome

- authentication configuration becomes explicit, centralized, and safe to consume across environments

---

# Phase 3 - Wire JWT Bearer Authentication And Admin Authorization

Add the actual authentication and authorization pipeline to the API.

Work

- register JWT bearer authentication using the official Auth0-compatible ASP.NET integration path
- configure token validation to enforce:
  - signature validation
  - expiration validation
  - issuer validation
  - audience validation
- register a named admin authorization policy that future admin endpoints can reuse
- keep the policy intentionally simple for MVP:
  - authenticated user required
  - no RBAC, scopes, or claims matrix yet
- insert middleware into the correct order in `Program.cs`:
  - routing/controller setup remains unchanged
  - `UseAuthentication()`
  - `UseAuthorization()`
  - endpoint mapping continues afterward

Outcome

- the API can authenticate bearer tokens and consistently gate protected endpoints behind one shared admin policy

---

# Phase 4 - Add A Minimal Protected Admin Probe Surface

Provide a real protected endpoint so the auth pipeline can be delivered and tested independently of future CRUD work.

Work

- add a minimal controller/action or equivalent endpoint under an admin-oriented route
- protect it with the named admin authorization policy
- keep the endpoint intentionally narrow:
  - no recipe writes
  - no database dependency
  - no extra business rules
- choose a lightweight success response such as `204 No Content` or a minimal authenticated status payload
- add endpoint metadata for:
  - success response
  - `401 Unauthorized`
- keep the endpoint reusable as a smoke-check surface until real admin endpoints from later specs are available

Outcome

- the feature becomes independently shippable and verifiable before `010-admin-recipes` lands

---

# Phase 5 - Preserve Public APIs And Improve API Discoverability

Make the new auth behavior visible where it helps consumers without changing the public recipe contract.

Work

- verify public recipe endpoints stay anonymous after auth middleware is introduced
- only add explicit anonymous metadata if the chosen authorization setup would otherwise risk accidental protection
- add bearer-auth OpenAPI metadata so development documentation shows how protected endpoints are called
- document `Authorization: Bearer <token>` usage on the protected surface
- ensure health checks remain reachable according to current operational expectations unless a deliberate protection choice is made elsewhere

Outcome

- the backend exposes clear auth expectations for admin consumers while preserving current public behavior

---

# Phase 6 - Add Automated Coverage And Test Utilities

Protect the auth pipeline with focused tests across startup and endpoint behavior.

Work

- add or extend API test infrastructure so tests can generate locally trusted JWTs without depending on live Auth0 infrastructure
- keep the test strategy close to production semantics:
  - signed token
  - expected issuer
  - expected audience
  - expiration handling
- add integration coverage for:
  - protected endpoint returns `401` with no token
  - protected endpoint returns `401` with malformed token
  - protected endpoint returns `401` with wrong issuer or audience
  - protected endpoint returns `401` with expired token
  - protected endpoint succeeds with a valid bearer token
  - public recipe endpoints still succeed anonymously
- add configuration/startup coverage if options validation is implemented as separate units

Outcome

- the authentication slice is protected against regression without relying on external identity-provider availability

---

# Phase 7 - Verification, Observability, And Handoff Readiness

Confirm the implementation is production-ready for follow-on admin features.

Work

- run the focused automated test suite covering the auth slice and existing public recipe regressions
- perform local runtime smoke checks against the protected endpoint using:
  - no token
  - invalid token
  - valid token
- confirm the existing OpenTelemetry ASP.NET request tracing still captures both authorized and unauthorized requests
- verify the failure contract remains security-safe:
  - no token leakage in responses
  - no secret values surfaced in logs or docs
- confirm the resulting policy and wiring are ready to be reused directly by `010-admin-recipes` and `011-image-upload`

Outcome

- authentication is ready to support real admin business endpoints without rework

---

# Suggested Implementation Order

1. Lock the auth boundary, response contract, and probe-endpoint approach.
2. Tighten `Auth0Settings` and add startup validation.
3. Register JWT bearer authentication and the shared admin authorization policy.
4. Insert authentication and authorization middleware into the API pipeline.
5. Add the minimal protected admin probe endpoint and endpoint metadata.
6. Add OpenAPI/security metadata while preserving anonymous public routes.
7. Add token-generation utilities and integration tests for success and failure paths.
8. Run automated and runtime verification, then hand the shared policy off to specs `010` and `011`.

---

# Completion Criteria

The feature is complete when:

- the backend validates Auth0 bearer tokens for protected requests
- token signature, expiration, issuer, and audience are all enforced
- missing or invalid bearer tokens receive `401 Unauthorized`
- existing public recipe endpoints remain accessible without authentication
- a minimal protected admin endpoint exists to prove the auth pipeline end-to-end
- authentication configuration fails fast when required values are missing
- OpenAPI or equivalent development metadata documents bearer-token usage for protected endpoints
- automated tests cover valid-token success, missing-token failure, invalid-token failure, expired-token failure, and anonymous access to public routes
- the shared admin policy is ready to be applied directly by `010-admin-recipes` and `011-image-upload`

---

# Follow-On Dependencies

This feature should leave the codebase ready for:

- `010-admin-recipes` to protect create/update/delete recipe endpoints with the shared admin policy
- `011-image-upload` to protect upload endpoints with the same auth configuration
- future RBAC or scope-based authorization if admin access later needs finer-grained control
- frontend admin clients to attach Auth0 access tokens via the `Authorization` header
