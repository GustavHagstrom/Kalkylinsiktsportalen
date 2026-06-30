# Bidcon

## Syfte Och Modulgräns

Bidcon äger integrationen mot Bidcon och portalens importerade representation av Bidcon-data.

Modulen äger:

- Bidcon API-integration och importmekanik
- `EstimationSnapshot`
- normaliserat snapshot-payloadformat
- snapshot metadata, inklusive `SnapshotSchemaVersion` och `SourceEditDate`
- readers/adapters från lagrat snapshot till aktuell intern readmodell

Bidcon äger inte:

- projekt, projektkopplade estimations eller aktiv versionskedja
- `ViewScopedChangeSet` eller användarens ersättningsval
- rapportdefinitioner, layout eller PDF-rendering

## Huvudmodeller

### `EstimationSnapshot`

Oföränderlig komplett import av en Bidcon-estimation vid ett importtillfälle.

Ansvar:

- vara källsanning för importerad Bidcon-data
- bevara all Bidcon-data som behövs för att återskapa den importerade estimationen utan nytt Bidcon-anrop
- lagra portalens normaliserade JSON-payload, inte Bidcons råa response-shape
- bevara Bidcon source identities där Bidcon tillhandahåller dem
- exponera metadata som behövs för identifiering, schema-tolkning och importjämförelse

`EstimationSnapshot` är immutable imported data, inte ett beteendetungt aggregate.

## Relationer

- Projects refererar snapshot via `EstimationSnapshotId` från `EstimationVersion`.
- Projects äger inte snapshot-payloaden och allokerar inte `EstimationSnapshotId`.
- Bidcon/import skapar `EstimationSnapshotId` när snapshotten skapas.
- Snapshot rows delas inte mellan olika `ProjectEstimation` i initial modell.

## Invariants

- `EstimationSnapshot` ändras inte efter import.
- Snapshot-payloaden sparas som ett komplett dokument i applikationsdatabasen. I dokumentdatabaser som MongoDB används native nested document storage; i andra databaser kan samma payload representeras som okomprimerad JSON-text.
- Snapshot-payloaden representeras i kod med strikta C# payload-typer per `SnapshotSchemaVersion`.
- Snapshot readers är versionsmedvetna och mappar till aktuell intern readmodell.
- Portalberäknade view- eller rapportvärden sparas inte i snapshotten om de inte krävs för att återskapa importerad källstate.
- Snapshot metadata är tunn och innehåller bara fält som behövs för att identifiera, versionera och ladda snapshotten.

## Nuvarande Policyer

### Import Och Dubbletter

`SourceEditDate` används för att avgöra om en Bidcon-redigering redan importerats för en viss `ProjectEstimation`, förutsatt att Bidcon garanterar att `EditDate` ändras vid relevanta estimation-ändringar.

Om en befintlig version för samma `ProjectEstimation` redan har matchande `SourceEditDate` skapas ingen ny snapshot. UI:t ska tydligt säga att ingen ny version skapades eftersom en version från senaste Bidcon-redigeringen redan finns, och erbjuda aktivering om den versionen inte är aktiv.

### Transaktionell Import

Skapande av snapshot i Bidcon och skapande eller aktivering av motsvarande `EstimationVersion` i Projects ska vara atomärt i importoperationen när modulerna delar databas.

### Bidcon Credentials

Bidcon credentials är organisationsägda och det finns högst ett aktuellt credential-par per organisation. Ändring av credentials ersätter hela paret efter att det nya paret har verifierats mot Bidcon API; om verifieringen misslyckas behålls befintliga credentials.

Administratörer får se maskerade sparade värden, inte hemligheten i klartext. Administration av Bidcon credentials kräver en separat access claim från vanlig användning av Bidcon-data.

När credentials ändras eller tas bort invalidas organisationens cachade Bidcon access token. Borttagna credentials är ett explicit konfigurationsläge och ska ge tydliga missing-credentials-fel vid Bidcon-anrop.

Administration av Bidcon credentials ska audit-loggas med aktör, organisation, tidpunkt, åtgärd och verifieringsresultat. Audit-loggen får inte innehålla `ApiSecret` eller fullständiga credential-värden.

## MVP-Begränsningar Och Tillfälliga Val

- Ingen separat rå Bidcon-response eller source envelope sparas bredvid normaliserad payload.
- Gamla snapshots migreras inte i förväg; schema-migrering införs först när ett konkret behov finns.
- Ingen djup payload-deduplicering görs när Bidcon `EditDate` har ändrats.
- Bidcon credentials får lagras i applikationens MongoDB i MVP även om värdena ännu inte är krypterade applikationssidigt. Detta är ett medvetet temporärt val; åtkomst ska gå via Bidcon-modulens store-kontrakt så lagringen senare kan bytas till krypterad lagring eller extern secret store utan att ändra use cases och UI.

## Sannolika Framtida Variationer

- indexerade metadatafält om specifika sök- eller filtreringsfall kräver det
- snapshot-komprimering om uppmätt lagringskostnad motiverar det
- starkare importjämförelse om Bidcon `EditDate` visar sig vara för svag

## Relaterade ADR:er

- [../adr/modules/bidcon/0005-store-estimation-snapshots-as-complete-imports.md](../adr/modules/bidcon/0005-store-estimation-snapshots-as-complete-imports.md)
- [../adr/modules/bidcon/0006-store-bidcon-credentials-in-mongo-for-mvp.md](../adr/modules/bidcon/0006-store-bidcon-credentials-in-mongo-for-mvp.md)
