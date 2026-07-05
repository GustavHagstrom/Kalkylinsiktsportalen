# Use IModule and UI composition registration

Status: superseded by ADR-0040

This ADR is kept only as historical context. It is not a source of current implementation guidance.

Current module registration guidance lives in:

- [0040 - Use static module registration functions](0040-use-static-module-registration-functions.md)
- [../../development.md](../../development.md)

Current architecture-level ownership for app shell composition lives in:

- [../../architecture/target-architecture.md](../../architecture/target-architecture.md)

## Rationale

This ADR originally explored `IModule` and `ModuleRegistration` as the target module registration model. ADR-0040 later replaced that with explicit static `IServiceCollection` extension methods that receive `AppRegistrationContext`.

## Consequences

- Do not use this ADR to decide how new modules register.
- Do not introduce `IModule`, `ModuleRegistration` or `ModuleRegistrator` from this historical decision.
- Use ADR-0040 and `docs/development.md` for current module registration practice.
