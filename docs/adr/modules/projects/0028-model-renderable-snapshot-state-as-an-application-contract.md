# Model renderable snapshot state as an application contract

`RenderableSnapshotState` should be modeled as an application read model or use case contract produced by Projects, not as a persisted aggregate or source-of-truth domain model. It represents the interpreted active snapshot state that Reports can consume for rendering, while persisted truth remains in `EstimationSnapshot`, `ProjectVersion` and `ViewScopedChangeSet`.
