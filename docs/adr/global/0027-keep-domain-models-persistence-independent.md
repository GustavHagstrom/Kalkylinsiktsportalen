# Keep domain models persistence-independent

Domain models in the target modules should be shaped by business invariants and aggregate behavior, not by MongoDB serialization convenience. Infrastructure may use separate document models and mappers when persistence needs a simpler mutable shape, so domain tests and use cases can work against the domain model without leaking database concerns into it.
