# Use pragmatic CQRS feature slices

Status: accepted

New module behavior should be organized as feature slices instead of broad horizontal `Application` folders. Each slice may expose commands, queries, validators, handlers and result contracts through small `Kernel` interfaces, but the repository should not introduce a mediator framework or pipeline until there is real repeated orchestration to hide.

Commands represent state-changing intent and queries represent read intent. Handlers are the test surface for module behavior, while domain models still protect invariants with domain exceptions and request problems still return validation results.

## Consequences

- Module folders should prefer `Features/<FeatureName>` for new slices.
- Common contracts such as `ICommand`, `ICommandHandler`, `IQuery`, `IQueryHandler`, `IValidator` and validation result types may live in `Kernel` when they stay product-agnostic and have no clearer owner.
- Slice-specific commands, queries, results, validators, stores and handlers stay inside the owning module feature.
- We avoid naming new handlers as `UseCase` unless a future decision intentionally changes this convention.
