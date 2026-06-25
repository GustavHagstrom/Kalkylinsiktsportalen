# Keep view-scoped change sets in Projects

Although replacement changes are scoped to a `View`, `ViewScopedChangeSet` is owned by Projects. Reports owns the view definition and `EnableChanges` flag, while Projects owns validation, storage and rendering input for controlled replacements against snapshots and project versions.
