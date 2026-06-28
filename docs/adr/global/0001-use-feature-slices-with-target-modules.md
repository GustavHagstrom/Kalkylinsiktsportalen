# Use feature slices with target modules

New product code should move toward the target modules `Projects`, `Reports`, `Bidcon`, `Access` and `Organizations`, while small changes can stay close to the existing feature that owns the behavior today. This lets the application migrate from the current feature-based Blazor Server structure toward a modular monolith without forcing a large rewrite before the module boundaries have been proven.
