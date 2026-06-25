# Do not copy change sets when duplicating views

Duplicating a view copies the view definition and `EnableChanges` setting, but not existing `ViewScopedChangeSet` state. View-scoped replacements are working state for a particular view and project version/snapshot state, so copying them into a new view would risk carrying old report decisions into a new workspace invisibly.
