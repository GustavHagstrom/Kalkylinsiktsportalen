# Keep persistence APIs behind store contracts

Status: accepted

Handlers in pragmatic CQRS feature slices should depend on small module- or slice-owned store contracts, not directly on `DbContext`, `IMongoCollection<T>` or other persistence APIs. Direct database API usage belongs in infrastructure implementations of those store contracts, where persistence-specific filters, mappings, transactions and optimistic concurrency can be kept out of application handlers. AppPlatform uses the term store for these contracts; repository is not the default name for new persistence contracts.

## Considered Options

- Let handlers use `DbContext` or `IMongoCollection<T>` directly. This keeps simple handlers transparent and avoids extra interfaces, but spreads persistence idioms, query details, tenant filters and test setup across application code.
- Require store contracts for all handler persistence. This adds small contracts per module or slice, but matches the existing codebase, keeps handlers testable with fakes, and gives EF/SQL and Mongo implementations the same application-facing shape.
- Use a hybrid rule where simple reads and writes may go directly to the database. This is flexible, but it makes the boundary subjective and risks recreating mixed patterns as the codebase grows.

## Consequences

- New handlers use store contracts for reads and writes.
- `DbContext`, `IMongoCollection<T>` and provider-specific query builders stay in infrastructure, store implementations, migrations, setup code or technical adapters.
- Store contracts should be small and use-case-oriented rather than generic CRUD repositories.
- Name new persistence contracts as stores unless there is a specific reason to use another term.
- Reusable queries, tenant-scoped reads, persistence mapping, transaction handling and concurrency behavior belong behind stores instead of being repeated in handlers.
