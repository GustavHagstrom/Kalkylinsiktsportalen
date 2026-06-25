# Store replacements as selections

Snapshot replacements should be stored as user selections, not as materialized calculation results. A replacement selection records the original row, replacement row, matching replacement tag, original quantity at selection time and policy version; Projects then builds a renderable view/snapshot state by applying the selection to the immutable snapshot and selected view.
