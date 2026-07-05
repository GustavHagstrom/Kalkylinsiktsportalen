# Target-Based Functional Access Grants

We will model functional access grants as a single target-based concept instead of separate user-access and role-access concepts. A `FunctionalAccessGrant` assigns access claims to an explicit target, where the target type is `User` or `Role`; direct user grants and role grants remain separate sources of access, and effective access is the union calculated at decision time.

This keeps the domain and persistence model aligned with the shared `Access grant` concept while avoiding an implicit dependency on Entra ID GUID uniqueness to distinguish users from roles. The application layer may still expose user-specific and role-specific commands when those match the UI and use cases, but those commands should map into the same target-based functional access grant model internally.

The app is not in production, so we do not need to preserve or migrate the existing separate Mongo data models for user access and role access. The refactor may replace those collections with the new target-based collection directly.
