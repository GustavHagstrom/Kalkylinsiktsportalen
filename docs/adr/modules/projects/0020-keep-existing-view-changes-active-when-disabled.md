# Keep existing view changes active when changes are disabled

`View.EnableChanges` controls whether users may make new view-scoped changes, not whether existing changes are applied. If changes are disabled on a view that already has active `ViewScopedChangeSet` records, those changes remain stored and continue to affect rendering and PDF export until explicitly cleared or replaced.
