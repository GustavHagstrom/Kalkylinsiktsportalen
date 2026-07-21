# Use access claim registry

Status: accepted

Access claim metadata is registration data with a system-wide identity rule, not just a list of display items. Use `AccessClaimRegistry` through `AppRegistrationContext.AccessClaims` so feature and module registration can contribute access claims while the registry enforces that each access claim value has exactly one meaning.

Access owns `AccessClaimDefinition` as the public access-claim registration contract; Composition owns the registry and collected registration state through `AppRegistrationContext`.

## Consequences

- `AccessClaimDefinition.Value` is the stable global identity for an access claim.
- `AccessClaimDefinition` is immutable registered metadata.
- Registering the same access claim metadata more than once is idempotent.
- Registering the same access claim value with different metadata fails during registration.
- The registry preserves registration order; UI code may apply its own presentation sorting when needed.
