# Keep current-user context as host boundary

Status: accepted

`ICurrentUserContext` is intentionally kept as a thin boundary between product code and the current host's authentication mechanism.

The contract exists to let modules read the neutral `CurrentUser` projection without depending on Blazor's `AuthenticationStateProvider`, `ClaimsPrincipal` or raw claims. It is not a session object, authentication service, profile source or cache.

## Decision

`ICurrentUserContext` may expose an async read of the current `CurrentUser`.

The host adapter may project the authenticated principal to `CurrentUser`, but must not own login/logout, profile loading, authorization decisions, access-claim interpretation, refresh workflows or external IO.

Feature use cases should prefer explicit `UserId` and `OrganizationId` inputs. UI and host-adjacent module components may use `ICurrentUserContext` when they need to cross from the host authentication state into module calls.

## Consequences

- Modules stay independent of Blazor authentication APIs.
- Current-user reads remain IO-free except for reading the host authentication state.
- Full user profiles, avatars and directory data must be fetched explicitly by the consumer that needs them.
- If the context starts caching, refreshing, navigating or calling Graph/database services, it has outgrown this decision.
