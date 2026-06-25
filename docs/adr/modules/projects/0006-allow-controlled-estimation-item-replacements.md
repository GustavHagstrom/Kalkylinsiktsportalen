# Allow controlled estimation item replacements

`SnapshotChangeSet` may store controlled replacements of imported Bidcon estimation rows or items, not only comments. Replacements are only allowed where the imported Bidcon data marks an item as replaceable through defined tags in the `Remarks`/`Remark` field, so users without Bidcon calculation permissions can make approved substitutions without mutating the immutable snapshot or turning the portal into a free-form calculation system.

Refined by ADR-0011: controlled replacements are stored in a view-scoped change set.
