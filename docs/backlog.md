# Backlog Och Öppna Frågor

Det här dokumentet samlar förbättringsområden, öppna frågor och föreslagen arbetsordning. Saker som redan besvaras av [architecture/target-architecture.md](architecture/target-architecture.md) eller [architecture/migration-plan.md](architecture/migration-plan.md) ska inte dupliceras här.

## Prioriterad Arbetsordning

1. Flytta bort secrets från versionshanterad konfiguration.
2. Bygg ut tester för MVP-kritisk logik.
3. Centralisera och städa routes där de blockerar eller förvirrar MVP-arbetet.
4. Tunna ut komponenter där det behövs för att leverera MVP säkert.
5. Flytta produktnära kod från gamla `Shared` stegvis när den ändå berörs.
6. Rensa naming/stavning och DI-mönster efter MVP i små steg.

## Före MVP

### Secrets Och Konfiguration

Problem: `appsettings.json` får inte bära riktiga hemligheter.

Åtgärder:
- flytta secrets till User Secrets, miljövariabler eller annan dokumenterad lösning
- dokumentera lokal konfiguration
- rotera/städa känslig historik om det behövs

Klart när inga hemligheter ligger i versionshanterad `appsettings.json` och lokal utveckling fungerar med dokumenterad setup.

### MVP-Kritiska Tester

Nuvarande testyta är smal och fokuserad kring `DataCellFormulaEvaluator`.

Första kandidater:
- view-logik som idag ligger i editor-/helper-klasser
- access- och rättighetslogik
- Mongo-mappers
- authorization policies
- kommande use cases för snapshots och rendering

## Under MVP Vid Behov

### Routes Och Feature-Kontrakt

Problem: routes verkar finnas både i feature-konstanter och hårdkodade komponenter.

Åtgärder:
- definiera feature-routes centralt
- använd samma route-definitioner i komponenter och navigation
- dokumentera vilka features som äger vilka routes när det behövs

### Tunnare Blazor-Komponenter

Problem: vissa komponenter innehåller orchestration, dataladdning och integrationslogik.

Åtgärder:
- flytta orchestration till feature services, use cases eller state-objekt
- låt komponenter fokusera på rendering och interaktion
- bryt ut mappning och laddningsflöden där det förenklar testning

## Efter MVP

### Krymp Gamla `Shared`

Problem: produktnära kod ligger i `Shared`, särskilt `Shared/Components/Views`.

Riktning:
- view-/rapportlogik mot `Reports`
- Bidcon-DTO:er och klientlogik mot `Bidcon`
- generella UI-primitiver mot `Shared/UI`
- små stabila begrepp mot `Shared/Kernel`
- teknisk implementation mot `Shared/Infrastructure`

### DI Och Feature-Registrering

Problem: `FeatureRegistrator` bygger en temporär service provider under registrering.

Åtgärd: se över hur features kan registreras utan att blanda registrering och upplösning.

### Naming Och Stavning

Kända exempel:
- `IFreature`
- `Enteties`
- `MongoRoleAccssMapper`
- `Pageination`
- `AppNavMenue`
- `HasThoasandsSeparator`

Gör små, säkra namnförbättringar stegvis.

## Öppna Frågor

**Produkt och domän**
- Vilka användartyper är officiella?
- Vilket namn är officiellt för produkt, repo och solution?
- Vilka delar av [product-and-domain.md](product-and-domain.md) ska bli beslutad produktbeskrivning?

**Arkitektur**
- När ska `Modules/*` skapas fysiskt i `src/BlazorServer`?
- Hur mycket av `IFeature`-mönstret ska behållas?
- Ska WYSIWYG/PDF vara intern del av `Reports` eller egen renderingmodul?
- Hur ska `SnapshotChangeSet` modelleras?

**Säkerhet**
- Vilken policy gäller för User Secrets, miljövariabler och annan hemlighetshantering?
- Hur ska Bidcon credentials lagras och roteras?

**Auth och access**
- Vilka claims och roller är officiellt definierade?
- Hur ska development-auth förhålla sig till production-auth långsiktigt?
- Ska alla features ha dokumenterade policies och accessregler?

**Dokumentation**
- Vem äger dokumentationen?
- När ska docs uppdateras som del av vanligt utvecklingsarbete?
- Vilka ADR:er ska skrivas först?

