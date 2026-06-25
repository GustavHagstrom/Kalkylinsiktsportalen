# Use strong ID types in domain code

Domain models, use cases and intentional module contracts should use small strong ID types for central concepts such as `OrganizationId`, `UserId`, `ProjectId`, `ViewId`, `EstimationSnapshotId` and `ProjectVersionId` when that reduces accidental ID mix-ups. DTOs, persistence documents and external integration models may still serialize these values as strings or GUIDs, with mapping at the boundary.
