# Introduce Identity boundary

Status: accepted

Current-user contracts and shared identity values must be usable by product modules without making them depend on AppShell or Blazor authentication APIs. Introduce a top-level `Identity` boundary that owns `UserId`, `OrganizationId`, `CurrentUser` and the read-only `ICurrentUserContext`.

AppShell continues to own authentication adapters, session refresh and login presentation, while Access owns claims interpretation and authorization decisions. `Identity` remains technology-neutral and does not expose claims or authentication lifecycle behavior.

## Consequences

- Modules may depend on `Identity` instead of AppShell for current-user information.
- `CurrentUser` always includes an active `OrganizationId`.
- Existing identity types and contracts move to the new boundary when the code migration is implemented.