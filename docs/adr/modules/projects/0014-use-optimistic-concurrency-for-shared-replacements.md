# Use optimistic concurrency for shared replacements

Because `ViewScopedChangeSet` is shared between users of the same view and project version/snapshot state, replacement edits should use optimistic concurrency. If two users edit the same original row concurrently, the later save should detect the stale version and ask the client to reload rather than silently overwriting the active selection.
