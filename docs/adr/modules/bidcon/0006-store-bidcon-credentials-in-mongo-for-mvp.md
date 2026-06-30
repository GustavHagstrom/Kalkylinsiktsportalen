# Store Bidcon credentials in Mongo for MVP

Bidcon credentials may be stored in the application MongoDB for MVP, even though the stored values are not yet encrypted by the application. This is an explicit temporary trade-off to keep the MVP small while the credentials surface is still isolated behind the Bidcon module's store contract, so later encrypted storage or an external secret store can replace Mongo persistence without changing use cases or UI.

## Consequences

The UI must not expose full saved secrets, credentials administration must be access-controlled and audit-logged, and changing or removing credentials must invalidate the organization's cached Bidcon access token. Before production hardening, revisit this decision and choose application-side encryption or an external secret store.
