# Treat estimation snapshots as immutable source of truth

`EstimationSnapshot` should be modeled as an immutable source-of-truth object rather than a rich aggregate with mutable behavior. Projects use cases may read and interpret the complete imported snapshot to build renderable state, while user-facing changes are stored separately in `ViewScopedChangeSet` and never by mutating the snapshot.
