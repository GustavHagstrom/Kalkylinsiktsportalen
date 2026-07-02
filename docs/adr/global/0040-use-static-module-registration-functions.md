# Use static module registration functions

Status: accepted

Supersedes ADR-0038.

Product modules should register through explicit static `IServiceCollection` extension methods, not through `IModule` instances. `AppRegistrationContext` is now the shared contribution point for UI composition, access claims and authorization policies, so a separate module object contract adds ceremony without buying dynamic discovery, lifetime management or a plugin boundary.

## Consequences

- New modules should expose an explicit `services.AddXModule(registrationContext)` registration function.
- Module registration functions should depend on `IServiceCollection` and `AppRegistrationContext`; if a module needs configuration or another startup dependency, pass that dependency explicitly instead of passing the whole `WebApplicationBuilder`.
- Startup should create one `AppRegistrationContext`, pass it to feature and module registration, then apply the completed context to dependency injection once. The lifecycle should be visible in application startup instead of hidden inside each registration method.
- The context lifecycle helpers should live with composition infrastructure, for example as `IServiceCollection` extensions under `Shared/Composition`, not under feature or module registration.
- `IModule`, `ModuleRegistration` and `ModuleRegistrator` should be removed rather than kept as a deprecated parallel registration path.
- Module registration should use `AppRegistrationContext` directly by default.
- Keep helper APIs only where they protect a real application rule, such as one settings area per module. Thin wrappers around collection methods should be removed.
- Settings area registration should keep an explicit module key so the Settings Hub can enforce at most one settings area per module after `IModule` is removed.
- Existing `Feature registration` remains a compatibility path for old `IFeature` code during migration.
- ADR-0038 remains useful for the UI composition and settings-area decisions, but its `IModule` target is superseded.
- App shell code is a top-level application boundary, not part of `Shared`.
- Modules must not depend on app shell code. Modules contribute shell metadata through neutral composition contracts such as `Shared/Composition`.
- App shell code consumes composition data and owns concrete shell surfaces such as app navigation and the Settings Hub.
- Reusable primitives needed by both app shell and modules belong in `Shared`; concrete shell routes and bootstrap code do not.
