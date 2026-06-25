# Defer undo and redo for view-scoped changes

MVP will not support undo/redo for `ViewScopedChangeSet`; `ClearViewScopedChanges` is a confirmed permanent action. If undo/redo becomes important later, it should be introduced as an operation log for view-scoped changes rather than as special-case metadata on individual actions.
