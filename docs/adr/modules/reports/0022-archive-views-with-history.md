# Archive views with history

Views that have history, exports or `ViewScopedChangeSet` state should be archived rather than hard-deleted. This is a deliberate product and retention decision: historical reports and view-scoped decisions must remain explainable even after a view leaves normal active use.

The detailed `View` lifecycle belongs in [../../../domain/reports.md](../../../domain/reports.md).
