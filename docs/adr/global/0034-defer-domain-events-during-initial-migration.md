# Defer domain events during initial migration

The initial migration to richer domain models should not introduce domain events by default. Aggregate methods should protect invariants and use cases should orchestrate repositories, policies, access checks and persistence directly until real cross-module follow-up behavior justifies adding domain events deliberately.
