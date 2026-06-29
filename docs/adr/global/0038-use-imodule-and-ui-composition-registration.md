# Use IModule and UI composition registration

New product modules should register through `IModule` and `ModuleRegistration`, while the older `IFeature` registration remains as a separate compatibility path during migration.

## Decision

Use `IModule` as the registration interface for target modules:

```csharp
public interface IModule
{
    void Register(ModuleRegistration module);
}
```

`ModuleRegistration` exposes intention-based registration surfaces:

- `Services` for dependency injection
- `Ui` for app shell and settings composition
- `Access` for declared access claims
- `Authorization` for policies

UI navigation should be registered as data, not arbitrary component types. Top navigation and settings navigation use navigation items containing text, route, order, optional icon, optional policy, and match behavior. Module-owned settings content can still be registered as a component because it is real feature content rather than app shell navigation.

## Rationale

The old `IFeature` contract mixed application startup concerns with feature naming and exposed low-level collections such as `RenderCollection`, `ClaimInfoCollection`, and `PolicyCollection`.

The target module contract should be a deeper interface: callers learn one registration method, while the implementation can evolve behind named surfaces. This keeps module registration explicit without turning the application into a plugin platform.

Navigation is structured app shell data. Registering it as data lets the shell own consistent rendering, ordering, authorization wrapping, and future frontend adaptation. Settings sections remain component-based because modules often own their own forms, state, and layout inside those sections.

## Consequences

- New `Modules/*` code should use `IModule`.
- Old `Features/*` code may keep using `IFeature` until migrated.
- `IFeature` and `IModule` are not inheritance-related.
- `ConfigureFeatures(...)` and `ConfigureModules(...)` can run side by side and contribute to the same runtime registration context.
- `RenderCollection` is compatibility plumbing, not the preferred interface for new modules.
- New navigation should be registered through `module.Ui` or the navigation-data methods, not by contributing navigation components directly.
