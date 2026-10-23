# Edit views through intent commands

Status: accepted

Reports editing should support both whole-view template saves and intention-revealing feature slices. `View` owns report-structure changes, while `DataSection` and `SheetSection` own their own internal changes. The report editor may save a complete edited view definition as one explicit bulk use case when the user works with the full template as a single document. More specific workflows may still use intention-revealing commands, such as renaming a view, moving a section, adding a section, changing a data cell or configuring a sheet section.

Whole-view update is not a generic shortcut around the domain model. It must be an explicit feature boundary that validates the complete edited definition, rejects changes that the domain model cannot express, protects stable section identities and preserves section order. Future narrow edit flows should still prefer specific intention-revealing commands when they need clearer audit, authorization, concurrency or user intent.

## Consequences

- Whole-view saves are part of the target command model for full-template editing.
- Structural and section-internal changes may be submitted together through the whole-view update use case when the editor saves a full report definition.
- Narrow workflows should still go through `View` behavior or the relevant section behavior when that better expresses the user's intent.
- Whole-view update is not a second place to define Reports write rules; it validates an edited definition against the same domain invariants.
- Edited section payloads identify existing sections by `SectionId`. Duplicate or blank section identities are rejected at the edit boundary instead of being silently repaired.
- An edited payload may introduce a section whose `SectionId` is not already in the stored `View`; this is treated as an add only when the identity is non-blank, unique, the section type is known and the resulting order is valid.
- A missing existing section is treated as a delete only when the caller has explicitly confirmed deletion.
- An existing `SectionId` may not change section type.
- Whole-view update preserves the submitted section order.
- `DataSection` and `SheetSection` remain responsible for validating their own internal invariants.
- Persistence may still store the whole `View`; this decision is about the application and domain boundary for edits, not necessarily document-level storage granularity.
- Optimistic concurrency for view editing is intentionally outside this decision and should be handled as a separate persistence and workflow decision.
- The first refactor slice validates view-structure concerns only. Stronger `DataSection` and `SheetSection` internal edit boundaries belong in later section-specific slices.
