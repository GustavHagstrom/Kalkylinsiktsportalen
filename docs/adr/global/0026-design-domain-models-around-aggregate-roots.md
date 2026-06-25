# Design domain models around aggregate roots

New or rewritten domain models in the target modules should be designed around aggregate roots with explicit invariant-protecting behavior, rather than making every entity independently mutable through richer setters. Use cases should call behavior on the aggregate root, such as replacing a row through a `ViewScopedChangeSet`, adding an estimation through a `Project`, or archiving a `View`, while child entities remain protected behind the aggregate boundary.
