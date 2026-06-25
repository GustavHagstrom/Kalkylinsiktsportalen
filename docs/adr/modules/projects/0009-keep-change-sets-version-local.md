# Keep change sets version-local

`SnapshotChangeSet` applies only to the snapshot state or project version where the change was made. Replacement selections are validated against that snapshot's imported tree and `Remark` tags, and are not automatically carried forward to later Bidcon imports with different rows, quantities, tags or tree structure.

Refined by ADR-0011: change sets are also scoped to a `View`, and the active term is `ViewScopedChangeSet`.

A later nice-to-have use case may suggest replacement selections for a new version based on the previous version and same view, but the user must confirm them and the system must create new view-scoped change sets for the new version.
