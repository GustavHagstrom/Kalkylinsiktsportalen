# Reuse code under the right module ownership

During migration to the target architecture, existing behavior and useful code should be reused when it still fits the intended model, but reused code should be moved, copied or adapted into the module that owns it rather than kept in an old location by inertia. The goal is not to rewrite everything, but to avoid preserving unclear ownership, UI orchestration or persistence-shaped design inside the new module boundaries.
