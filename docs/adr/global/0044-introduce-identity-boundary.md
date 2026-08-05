# Introduce Identity boundary

Status: accepted

Current-user contracts and shared identity values must be usable by product modules without making them depend on AppShell or Blazor authentication APIs. Introduce a top-level `Identity` boundary that owns `UserId`, `OrganizationId`, `CurrentUser` and the read-only `ICurrentUserContext`.

AppShell continues to own authentication adapters, session refresh and login presentation, while Access owns access-claim interpretation and authorization decisions. `Identity` remains technology-neutral and does not expose authentication lifecycle behavior. It may own neutral identity claim names and helpers for projecting `ClaimsPrincipal` into shared identity values, because those helpers are part of the identity boundary rather than AppShell or Access policy.

## Consequences

- Modules may depend on `Identity` instead of AppShell for current-user information.
- `CurrentUser` always includes an active `OrganizationId`.
