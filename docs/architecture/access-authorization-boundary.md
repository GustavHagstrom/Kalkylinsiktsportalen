# Access authorization boundary

Access owns the portal's authorization vocabulary and decisions.

## Canonical authorization vocabulary

`Modules.Access.Public.Authorization.AccessAuthorizationClaims` is the single source for:

- the portal access-claim type
- the external directory role claim type
- the application-admin role value

AppShell and other modules may consume this vocabulary through the public Access namespace. They must not reference `Modules.Access.Authentication`.

## Authentication adapter

Raw `ClaimsPrincipal` data is interpreted at the Access authentication boundary. The adapter maps external user, organization, group and admin claims into an Access-owned `AccessSubject`.

Feature requests such as view-access decisions receive `AccessSubject`, not `ClaimsPrincipal`. Feature handlers therefore depend on normalized authorization data rather than Microsoft Identity claim details.

## Razor and bootstrap dependencies

The root `_Imports.razor` may expose only module public contracts. Module-internal Razor imports belong in the owning module's `_Imports.razor` or in the individual component.

The application bootstrap may reference a module's registration entrypoint at `Modules.<Module>` and explicit `Modules.<Module>.Public` contracts. It must not reference module implementation namespaces.
