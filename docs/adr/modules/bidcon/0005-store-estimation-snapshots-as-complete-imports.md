# Store estimation snapshots as complete imports

`EstimationSnapshot` stores a complete imported Bidcon estimation state. It is the portal's historical source data for future views, comparisons and report renders, so those features do not depend on re-fetching old data from Bidcon.

## Decision

- Store snapshots in the Bidcon/import module as immutable imported data, not as behavior-heavy aggregates.
- Projects records only `EstimationSnapshotId` on `EstimationVersion`; it does not own snapshot JSON or allocate snapshot ids.
- Store the normalized snapshot payload as uncompressed JSON text in the application database, loaded only when needed.
- Use a portal-owned normalized payload format, not Bidcon's raw response shape.
- Preserve all Bidcon data needed to reconstruct the imported estimation state, including metadata, structure, rows and Bidcon source identities.
- Keep portal-computed view/report values out of the snapshot unless required to reproduce the imported source state.
- Represent payloads with strict C# types per `SnapshotSchemaVersion`; readers map stored versions to the current internal read model.
- Do not eagerly migrate old snapshots; add migrations only when a concrete need justifies rewriting historical data.

Snapshot metadata stays thin: id, import time, `SnapshotSchemaVersion` and `SourceEditDate`. The snapshot does not store a separate raw Bidcon response or source envelope.

## Import Rules

- Duplicate detection is scoped to one `ProjectEstimation`.
- If any existing version for that `ProjectEstimation` has `SourceEditDate` equal to Bidcon's current `EditDate`, no new snapshot or version is created.
- If that existing version is not active, the UI should offer to activate it, but must not activate it automatically.
- If Bidcon's `EditDate` changed, create a new snapshot and `EstimationVersion` without deep payload deduplication.
- A newly imported version becomes active immediately.
- `VersionNumber` increases only when a new version is created; skipped imports and manual reactivation do not consume version numbers.
- Manual reactivation only changes `ActiveEstimationVersionId`.

These rules assume Bidcon guarantees that `EditDate` changes whenever relevant estimation content changes. If that guarantee is too weak, import must use a stronger comparison before suppressing version creation.

## Consistency

Snapshot rows are not shared across `ProjectEstimation` instances in the initial model. Creating a snapshot and creating or activating the corresponding `EstimationVersion` must be atomic in the import operation, using one database transaction when both modules persist to the same database.

## Consequences

This increases storage and import cost, but keeps historical rendering independent from Bidcon availability and API changes. Snapshot internals are not part of the relational query model unless a later use case proves that specific fields need indexing or filtering.
