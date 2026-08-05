# Add domain events only for concrete follow-up behavior

Domain events are not a default part of new domain models. Aggregate methods protect invariants, and use cases orchestrate stores, policies, access checks and persistence directly until concrete cross-module follow-up behavior justifies introducing domain events deliberately.
