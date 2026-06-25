# Model project versions as snapshot selections

`Projektversion` is a named saved project state that references a selected set of `EstimationSnapshot` records, rather than owning copied snapshot data or change sets. This keeps project history useful for comparison while preserving snapshots and snapshot-level change sets as the source data used for rendering.
