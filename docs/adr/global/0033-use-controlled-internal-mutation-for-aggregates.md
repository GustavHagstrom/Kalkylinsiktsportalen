# Use controlled internal mutation for aggregates

Aggregate roots in the target domain model may mutate their internal state through behavior methods that enforce invariants, using private setters or private collections as needed. Immutable modeling should be reserved for snapshots and value objects, such as `EstimationSnapshot`, IDs and small domain values, rather than requiring every aggregate operation to return a copied object graph.
