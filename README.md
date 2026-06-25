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

Repoets långlivade kontextlager för domän, arkitektur, utvecklingssätt, planering och agentarbete.

## Läs Detta Först

Rekommenderad läsordning:

1. [CONTEXT.md](CONTEXT.md)
2. [docs/product-and-domain.md](docs/product-and-domain.md)
3. [docs/domain/README.md](docs/domain/README.md)
4. [docs/architecture/current-state.md](docs/architecture/current-state.md)
5. [docs/architecture/target-architecture.md](docs/architecture/target-architecture.md)
6. [docs/architecture/migration-plan.md](docs/architecture/migration-plan.md)
7. [docs/development.md](docs/development.md)
8. [docs/planning.md](docs/planning.md)

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

- kanoniskt domänspråk i `CONTEXT.md`
- produkt och domän i `docs/product-and-domain.md`
- levande modulägd domänmodell i `docs/domain/`
- nuläge och målarkitektur i `docs/architecture/`
- lokalt arbete och kodriktlinjer i `docs/development.md`
- planeringsprinciper och öppna frågor i `docs/planning.md`
- återanvändbar agentkontext i `docs/agent-guide.md`

Nya eller ändrade kanoniska domäntermer ska uppdateras i `CONTEXT.md`. Långlivade beslut som är svåra att backa ur och bygger på en verklig trade-off ska dokumenteras som korta ADR:er enligt `docs/adr/README.md`. `docs/planning.md` ska bara innehålla öppna frågor och planeringsprinciper.

## Status

Repoet är under aktiv utveckling. Dokumentation och kodstruktur utvecklas löpande mot målarkitekturen, med fokus på att hitta rätt plats för ny kod och undvika att `Shared` växer som defaultyta.
