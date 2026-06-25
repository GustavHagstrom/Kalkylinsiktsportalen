# Use factories for aggregate creation

Aggregate roots and entities in the target domain model should be created through explicit factory methods or behavior methods when invariants, metadata or initial state must be enforced. Public constructors are acceptable for trivial value objects, but aggregates such as `Project`, `ViewScopedChangeSet` and `View` should not rely on object initializers or freely settable properties to become valid.
