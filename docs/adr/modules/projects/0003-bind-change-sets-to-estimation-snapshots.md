# Bind change sets to estimation snapshots

`SnapshotChangeSet` belongs to a single `EstimationSnapshot`, rather than to a whole project version or a separate global revision stream. This keeps the first render model explicit as `Snapshot + ChangeSet + View`; a future `Projektversion` can compose multiple snapshots and their change sets without owning the changes itself.

Refined by ADR-0011: controlled replacement change sets are also scoped to a `View`, and the active term is `ViewScopedChangeSet`.
