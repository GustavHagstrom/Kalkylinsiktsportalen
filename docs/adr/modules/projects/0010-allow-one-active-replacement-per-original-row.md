# Allow one active replacement per original row

Within a `ViewScopedChangeSet`, each original estimation row or item may have at most one active replacement selection. If a user changes from one replacement to another, the active selection changes and stores a stable user ID, display name/email snapshot and timestamp; any detailed audit trail of previous selections can be introduced separately without making the renderable view/snapshot state ambiguous.
