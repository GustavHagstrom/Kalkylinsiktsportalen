# Kalkylinsiktsportalen

Kalkylinsiktsportalen är en .NET 10 och Blazor Server-applikation för att hämta, spara och presentera kalkylinformation från Bidcon i anpassningsbara rapportvyer.

Produkten är främst en rapportportal. På sikt ska den även stödja projekt där en organisation kan samla ett eller flera Bidcon-estimat, skapa oföränderliga snapshots vid olika tillfällen, granska versioner genom egna rapportvyer och styra åtkomst per funktion och view.

Blazor Server används som första UI-host för snabb utveckling, men arkitekturen ska stegvis röra sig mot en modulär monolit där produktlogik, use cases och kontrakt kan återanvändas av en framtida Blazor WASM-, React- eller annan frontend.

## Repoöversikt

### `src/BlazorServer`

Huvudapplikationen.

Nuvarande teknik och mönster:
- Blazor Server och .NET 10
- MudBlazor för UI
- MongoDB för bland annat användarinställningar, åtkomst, credentials och views
- Microsoft Identity / OpenID Connect i production
- mockad auth i development
- Bidcon API via `HttpClient`
- feature-baserad struktur via `IFeature`

### `src/Tests/BlazorServer.Tests`

Testprojekt för huvudapplikationen.

Testytan är fortfarande smal och bör byggas ut i takt med att kärnflödena för snapshots, rapportvyer och access blir tydligare.

### `docs`

Repoets långlivade kontextlager för domän, arkitektur, utvecklingssätt, backlog och agentarbete.

## Läs Detta Först

Rekommenderad läsordning:

1. [docs/product-and-domain.md](docs/product-and-domain.md)
2. [docs/architecture/current-state.md](docs/architecture/current-state.md)
3. [docs/architecture/target-architecture.md](docs/architecture/target-architecture.md)
4. [docs/architecture/migration-plan.md](docs/architecture/migration-plan.md)
5. [docs/development.md](docs/development.md)
6. [docs/backlog.md](docs/backlog.md)

För en komplett dokumentationskarta, se [docs/README.md](docs/README.md).

## Arkitekturriktning

Målbilden är en modulär monolit med tydliga produktmoduler:

- `Projects` för projekt, estimatkopplingar och snapshotversioner
- `Reports` för views, rapportmallar, rendering och PDF/WYSIWYG
- `Bidcon` för extern integration, credentials och import
- `Access` för funktions- och view-rättigheter
- `Organizations` för tenant-kontext och organisationsisolering

Nuvarande `Shared` ska på sikt krympas och bara innehålla verkligt delade byggblock, exempelvis små kernel-typer, generiska UI-primitiver och teknisk infrastruktur.

## Dokumentationsprincip

`README.md` ska vara kort och fungera som ingång till repoet.

Detaljer hör hemma i `docs/`:

- produkt och domän i `docs/product-and-domain.md`
- nuläge och målarkitektur i `docs/architecture/`
- lokalt arbete och kodriktlinjer i `docs/development.md`
- förbättringsområden och öppna frågor i `docs/backlog.md`
- återanvändbar agentkontext i `docs/agent-guide.md`

## Status

Repoet är under aktiv utveckling. Dokumentation och kodstruktur utvecklas löpande mot målarkitekturen, med fokus på att hitta rätt plats för ny kod och undvika att `Shared` växer som defaultyta.
