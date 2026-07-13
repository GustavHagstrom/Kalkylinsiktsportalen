# Introduce dedicated Composition boundary

Status: accepted

Product modules must not depend on the concrete `AppShell`. Shared module contribution contracts therefore move to a top-level `Composition` boundary used by both modules and the host.

`Composition` owns generic bootstrap state and registries, including `AppRegistrationContext`, but no product semantics or shell presentation. It may not depend on `Modules` or `AppShell`; login presentation remains in AppShell, and the host composition root applies the completed registration state.

## Consequences

- `Modules -> Composition <- AppShell`; `Modules !-> AppShell`.
- `Composition` remains a folder and namespace in the current assembly until project extraction provides concrete value.
- Architecture tests protect the dependency direction.