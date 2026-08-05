# Use static module registration functions

Status: accepted

Product modules register through explicit static `IServiceCollection` extension methods that receive `AppRegistrationContext`. `AppRegistrationContext` is the central contribution point for UI composition, access claims and authorization policies.

## Consequences

- New modules should expose an explicit `services.AddXModule(registrationContext)` registration function.
- Module registration functions should depend on `IServiceCollection` and `AppRegistrationContext`; if a module needs configuration or another startup dependency, pass that dependency explicitly instead of passing the whole `WebApplicationBuilder`.
- Startup should create one `AppRegistrationContext`, pass it to feature and module registration, then apply the completed context to dependency injection once. The lifecycle should be visible in application startup instead of hidden inside each registration method.
- The context lifecycle helpers should live with `Composition` infrastructure, for example as `IServiceCollection` extensions, not under feature or module registration.
- Module registration should use `AppRegistrationContext` directly by default.
- Keep helper APIs only where they protect a real application rule, such as one settings area per module. Thin wrappers around collection methods should be removed.
- Settings area registration should keep an explicit module key so the Settings Hub can enforce at most one settings area per module.
- App shell code is a top-level application boundary.
- Modules must not depend on app shell code. Modules contribute shell metadata through neutral `Composition` contracts.
- App shell code consumes composition data and owns concrete shell surfaces such as app navigation and the Settings Hub.
- Reusable primitives needed by both app shell and modules belong in `Composition`, `Kernel`, `Identity` or `UiKit` depending on ownership; concrete shell routes and bootstrap code do not.
