# Edit views through intent commands

Status: accepted

Reports editing should move toward intention-revealing feature slices for view changes. `View` owns report-structure changes, while `DataSection` and `SheetSection` own their own internal changes. Editing commands should therefore describe the user intent, such as renaming a view, moving a section, adding a section, changing a data cell or configuring a sheet section, and handlers should call the owning domain object instead of replacing the whole view shape directly.

The existing whole-view update flow may remain as a compatibility bridge for the current editor while the module is refactored. New behavior should not deepen that pattern. While it exists, the bridge should translate the edited view payload into domain operations and reject changes that the domain model cannot express, rather than accepting arbitrary section lists and relying on normalization as validation.

## Consequences

- Structural changes go through `View` behavior.
- Section-internal changes go through the relevant section behavior.
- Whole-view saves are treated as legacy compatibility, not the target command model.
- The compatibility bridge adapts editor-shaped payloads into domain operations; it is not a second place to define Reports write rules.
- Edited section payloads identify existing sections by `SectionId`. Duplicate or blank section identities are rejected at the edit boundary instead of being silently repaired.
- During the bridge period, an edited payload may introduce a section whose `SectionId` is not already in the stored `View`; this is treated as an add only when the identity is non-blank, unique, the section type is known and the resulting order is valid.
- Persistence may still store the whole `View`; this decision is about the application and domain boundary for edits, not necessarily document-level storage granularity.
- Optimistic concurrency for view editing is intentionally outside this decision and should be handled as a separate persistence and workflow decision.
- The first refactor slice validates view-structure concerns only. Stronger `DataSection` and `SheetSection` internal edit boundaries belong in later section-specific slices.
