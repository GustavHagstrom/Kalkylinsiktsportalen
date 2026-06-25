# Migrate code directly into clean target shape

Because the product is still far from production, migration work should move or copy useful existing code into the clean target module shape instead of introducing temporary legacy adapter layers as a default strategy. A migrated slice should expose the intended domain model, use case contract and module ownership when it lands, even if the slice is small.
