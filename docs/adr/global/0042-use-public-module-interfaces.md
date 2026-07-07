# Use public module interfaces

Status: accepted

Product modules should expose cross-module contracts only through a `Public` folder. Code outside `Public` is a module implementation detail and should not be referenced by other modules, even when the current project structure technically allows it.

The public module interface may include:

- `Public/Contracts` for types and abstractions that other modules may reference
- `Public/Components` for entry components that other modules may embed

Modules should not call each other's feature use cases directly. When a module needs behaviour from another module, the owning module should expose a narrow contract under `Public/Contracts` and keep the feature implementation behind that contract.

Public components are allowed, but only as intentional entry components. They should expose a small interface in terms of the caller's need and the owning module's public contracts. Internal helper components should stay outside `Public`.

## Consequences

- Other modules may reference only code under `Modules/{Module}/Public`.
- `Modules/{Module}/Public/Contracts` is the preferred seam for cross-module application behaviour.
- `Modules/{Module}/Public/Components` is reserved for stable entry components, not shared internal UI parts.
- `Features`, `Domain`, `Infrastructure`, `Settings` and non-public `Components` folders are implementation details of the owning module.
- If another module needs something from a feature use case, store, external DTO, API client or helper component, the owning module should publish a smaller interface under `Public` instead.
- `Shared` remains reserved for stable cross-cutting building blocks and should not be used to bypass module ownership.
