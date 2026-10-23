# Model SheetSection invariants explicitly

`SheetSection` is a report-template section bound to one immutable `SheetKind` under a stable `SectionId`; changing the sheet kind creates a new section. We model its columns as an effectively complete ordered catalog of immutable `SheetColumnConfiguration` values, and its cells as an effectively complete matrix of immutable `SheetCellConfiguration` values keyed by `SheetRowKind` and `SheetColumnKind`. Persistence may omit newly introduced default columns or cells as a schema-evolution fallback, but reconstitution normalizes the domain object to effective completeness and rejects unknown keys instead of silently dropping them.

## Consequences

`SheetSection` owns mutation of column and cell collections through intention-revealing methods or validated bulk paths, while UI and rendering read immutable configurations. `GetCell` returns the effective immutable cell configuration without mutating state; lazy/defaulting exists for schema evolution, not as an active sparse/pruning model. Replacements remain section-scoped in eligibility, Projects-owned in lifecycle and storage, and rendered through the system `ReplacementPolicy` rather than per-section policy.
