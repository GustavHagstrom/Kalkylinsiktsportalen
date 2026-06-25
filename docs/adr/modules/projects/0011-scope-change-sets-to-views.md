# Scope change sets to views

Change sets for controlled replacements are scoped to the combination of `View`, `ProjectVersion` and `EstimationSnapshot`, rather than applying globally to every render of the same snapshot. This makes a view an intentional workspace: a view with changes enabled can have its own replacement selections while another view can render the same snapshot/version differently. This refines the earlier snapshot-local decision by adding view and version scope; snapshots remain immutable and replacement validation still belongs in Projects.
