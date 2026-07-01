# Use IModule and UI composition registration

Status: superseded by ADR-0040

New product modules should register through `IModule` and `ModuleRegistration`, while the older `IFeature` registration remains as a separate compatibility path during migration. UI composition is structured app-shell data, not arbitrary component injection.

## Decision

Use `IModule` as the registration interface for target modules:

```csharp
public interface IModule
{
    void Register(ModuleRegistration module);
}
```

Keep the registration paths separate in code. `Feature registration` is compatibility for old `IFeature` code, and `Module registration` is the target model for new `IModule` code. Both paths may contribute to shared runtime app-shell data, but that shared data should live behind neutral `Composition` types rather than in either registration path.

`ModuleRegistration` exposes intention-based registration surfaces:

- `Services` for dependency injection
- `Ui` for app shell and settings composition
- `Access` for declared access claims
- `Authorization` for policies

UI navigation should be registered as data, not arbitrary component types. Top navigation uses navigation items containing text, href, order, optional icon, optional policy, and match behavior.

Settings use a more specific model. `/settings` is a shell-owned `Settings Hub` that lists module-owned `Settings Area` entries. A module may declare at most one settings area through `module.Ui.Settings.SetArea(...)`. The settings area registration is structured metadata: stable id, title, description, href, order, optional icon, and optional policy. The href must point under `/settings/`; `/settings` itself is reserved for the hub. The actual settings page is still owned by the module through normal Blazor routing, not by registering a component type in UI composition.

The older `SettingsNavigation` registration remains compatibility plumbing for old `IFeature` code only. New settings hub code should not read it as a fallback, and new module code should not use it.

## Rationale

The old `IFeature` contract mixed application startup concerns with feature naming and exposed low-level collections such as `RenderCollection`, `ClaimInfoCollection`, and `PolicyCollection`.

The target module contract should be a deeper interface: callers learn one registration method, while the implementation can evolve behind named surfaces. This keeps module registration explicit without turning the application into a plugin platform.

Navigation is structured app shell data. Registering it as data lets the shell own consistent rendering, ordering, authorization wrapping, and future frontend adaptation.

Settings areas are also shell-discovered data, but not generic settings navigation. A settings area is the single visible settings entry for a module. If a module needs multiple settings pages, it owns the internal navigation below its area route. This keeps the hub clean and prevents modules from contributing arbitrary fragments to a shared settings page.

## Consequences

- New `Modules/*` code should use `IModule`.
- Old `Features/*` code may keep using `IFeature` until migrated.
- `IFeature` and `IModule` are not inheritance-related.
- Shared runtime data such as UI composition, claims, and policies should live in neutral composition types, not in the feature or module registration APIs.
- `ClaimInfoCollection` and `PolicyCollection` are composition data; feature and module registration are only contribution paths.
- The old and new registration paths should be physically separated in folders and namespaces.
- `ConfigureFeatures(...)` and `ConfigureModules(...)` can run side by side and contribute to the same runtime registration context.
- `RenderCollection` is compatibility plumbing, not the preferred interface for new modules.
- New navigation should be registered through `module.Ui` or the navigation-data methods, not by contributing navigation components directly.
- New module settings should be registered through `module.Ui.Settings.SetArea(...)`.
- `ModuleRegistration` must not expose legacy settings navigation or settings section registration.
- `SettingsSections` and component-based shared settings-page composition are not part of the target model.
- `SettingsNavigation` can remain only as old feature compatibility and must not be rendered by the new `Settings Hub`.
