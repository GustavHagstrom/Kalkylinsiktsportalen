# Use encapsulated domain models in target modules

When domain models are introduced or rewritten in the target modules, they should protect their own invariants through constructors, factories and behavior methods rather than exposing freely mutable state. This applies to domain models in modules such as Projects, Reports, Bidcon, Access and Organizations, but not to transport DTOs, request/response models, view models or persistence-only models, which may remain simple data carriers when that is their purpose.
