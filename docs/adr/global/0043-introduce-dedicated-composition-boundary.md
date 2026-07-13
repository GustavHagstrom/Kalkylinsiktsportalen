# Introduce dedicated Composition boundary

Status: accepted

Application-wide module contribution contracts currently live under `AppShell/Composition`, which forces product modules to reference `AppShell` even though modules must remain independent of the concrete portal shell. Introduce a top-level `Composition` boundary that owns shared bootstrap state, contribution models and registries used by both modules and the host.

`Modules/*` must not reference `AppShell/*`. Both modules and AppShell may depend on `Composition`, while `Composition` must not depend on AppShell, product modules or future module contract projects. This direction keeps the boundary extractable if modules later become separate projects and avoids circular project references.

`AppRegistrationContext` remains the single registration facade and keeps its name. It continues to collect UI, access-claim and authorization-policy contributions. The registration models are generic bootstrap types; individual modules own the meaning of their contributions, while Access owns runtime authorization vocabulary, interpretation and access decisions.

The shared boundary is introduced as a top-level folder and namespace in the existing BlazorServer project, not as a separate project yet. Project extraction is deferred until it provides concrete value.

Login presentation is not a module contribution and remains owned by AppShell. `LoginDisplayComponent` and `SetLoginDisplay<T>()` must therefore be removed from the shared `UiRegistry` and handled by the host/AppShell authentication setup.

Composition owns creation and collection of registration state, but not application of that state to ASP.NET Core. The host composition root in `ServiceAndFeatureRegistrations` creates the context, registers modules, selects host-specific authentication and login presentation, and applies the completed context to dependency injection and authorization. No separate Bootstrap boundary is introduced.

`PolicyCollection` should be renamed to `AuthorizationPolicyRegistry` to describe its architectural role consistently with the other registries.

## Consequences

- Move shared contribution types and registries from `AppShell/Composition` to the top-level `Composition` namespace.
- Keep `AppRegistrationContext` as the facade containing UI, access-claim and authorization-policy registries.
- Keep concrete shell surfaces, login presentation and host wiring outside `Composition`.
- Protect the dependency rules with architecture tests: `Modules !-> AppShell`, `Composition !-> Modules`, `Composition !-> AppShell`, and `Kernel !-> Composition`.
- ADR-0040 remains the decision for static module registration functions, but its earlier placement examples under `Shared/Composition` or AppShell are superseded by this ADR.