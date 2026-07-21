# Introduce dedicated Composition boundary

Status: accepted

Product modules must not depend on the concrete `AppShell`. Generic module contribution contracts therefore move to a top-level `Composition` boundary used by both modules and the host.

`Composition` owns generic bootstrap state and registries, including `AppRegistrationContext`, but no product semantics or shell presentation. It may depend on small, intentional module-owned registration contracts under `Modules/{Module}/Public`, but not on module internals or `AppShell`; login presentation remains in AppShell, and the host composition root applies the completed registration state.

## Consequences

- `Modules -> Composition <- AppShell`; `Composition -> Modules/{Module}/Public`; `Modules !-> AppShell`; `Composition !-> AppShell`.
- `Composition` remains a folder and namespace in the current assembly until project extraction provides concrete value.
- Architecture tests protect the dependency direction.
