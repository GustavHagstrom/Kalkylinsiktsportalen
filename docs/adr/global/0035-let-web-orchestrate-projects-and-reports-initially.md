# Let Web orchestrate Projects and Reports initially

The initial Projects and Reports integration should let the Blazor/Web layer orchestrate between module use cases without owning domain logic. Web can combine the selected view, project version and snapshot, ask Projects for a `RenderableSnapshotState`, and pass that state to Reports for view rendering, while Projects keeps replacement and snapshot rules and Reports keeps view definition, layout and PDF responsibilities.
