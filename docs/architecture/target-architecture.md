# Målarkitektur

## Syfte

Det här dokumentet beskriver den övergripande målarkitekturen för Kalkylinsiktsportalen.

Det fokuserar på arkitekturriktning, modulgränser, beroenderiktningar och ansvarsfördelning mellan större delar av systemet.

Det här dokumentet äger inte den detaljerade domänmodellen. Aktuella domänbegrepp, aggregates, relationer, invariants och modulnära policyer ägs av `docs/domain/*.md`. Långlivade och svårbackade trade-offs dokumenteras i ADR:er.

Det här dokumentet äger inte heller lokal setup eller dagliga arbetsregler. Praktiska kodriktlinjer och lokalt arbetssätt ägs av [../development.md](../development.md).

Den sammanfattande riktningen är:

- modulär monolit som systemform
- vertical slices som huvudsaklig arbetsform
- pragmatisk CQRS för nya use cases
- clean architecture som beroendeprincip
- DDD-light för domänmodellering där det ger verkligt värde

Målbilden utgår från produktinriktningen:

1. rapportportal
2. projekt- och versionshantering
3. kalkylutforskare

Blazor Server används fortsatt som första UI-host, men backend, kontrakt och affärslogik ska kunna bära framtida klienter som Blazor WASM eller React.

## Arkitekturprinciper

### 1. Modulär Monolit Först

Systemet utvecklas som en modulär monolit innan det eventuellt delas upp i fler deploybara delar.

Det innebär att:

- lösningen kan köras som en applikation
- koden organiseras i tydliga moduler
- modulgränser respekteras även innan de är separata projekt
- externa integrationer och persistence ligger bakom avsiktliga kontrakt

### 2. Produktområden Och Use Cases Före Global Lagerstruktur

Lösningen ska i första hand organiseras runt produktområden och användningsfall, inte runt en global teknisk lagerindelning.

Moduler får ha interna lager när det hjälper, till exempel `Domain`, `Application`, `Infrastructure` och `Components`, men tomma eller symmetriska lager ska inte skapas utan behov.

Vertical slices är den praktiska arbetsformen för ny produktlogik. Clean architecture används som riktning för beroenden, inte som krav på en global mappstruktur.

Pragmatisk CQRS är den föredragna formen för nya feature slices: kommandon uttrycker förändringsintention, queries uttrycker läsintention, och handlers utgör en tydlig testyta utan att lösningen för den skull behöver ett tungt mediator- eller pipeline-ramverk.

### 3. UI Är Utbytbart

Blazor-komponenter ska inte vara platsen där central produktlogik bor.

UI ska främst:

- rendera data
- hantera användarinteraktion
- anropa use cases
- visa laddning, fel och resultat

Produktlogik ska kunna återanvändas av flera klienter, tester och andra gränssnitt.

### 4. Tydliga Ägargränser

Varje viktig del av produktlogiken ska ha en tydlig modulägare.

Tvärgående kod ska hållas liten och stabil. `Shared` är inte en defaultplats för otydligt ägarskap.

Modulernas detaljerade modeller och policyer ska beskrivas i domändokumenten, medan detta dokument bara anger ansvarsfördelningen på hög nivå.

### 5. Organisation Är Hård Systemgräns

Organisationen är den grundläggande tenant-gränsen i systemet.

Data, rättigheter, integrationer och konfiguration ska utformas så att organisationsgränsen förblir tydlig och genomgående.

## Målstruktur

Den långsiktiga riktningen är tydliga produktmoduler med avsiktliga kontrakt mellan sig.

Målmodulerna är:

- `Projects`
- `Reports`
- `Bidcon`
- `Access`
- `Organizations`

Detta dokument äger modulernas ansvar på översiktsnivå:

- `Projects` äger projekt, versioner, snapshots och aktiv renderbar kalkylstate
- `Reports` äger views, rapportstruktur, rendering och export
- `Bidcon` äger extern integration, credentials och import
- `Access` äger funktions- och view-rättigheter
- `Organizations` äger tenant-kontext och organisationsinställningar

Den detaljerade modellen inom varje modul ska beskrivas i `docs/domain/*`.

## Shared Och Gemensamma Byggblock

Gemensamma byggblock ska hållas små, stabila och avsiktliga.

### SharedKernel

Får innehålla små och stabila begrepp som används över modulgränser, till exempel identiteter och enkla kontraktstyper.

SharedKernel ska vara litet och får inte bli en plats för produktflöden.

### Shared UI

Får innehålla generella UI-primitiver utan produktspecifik affärslogik.

### Gemensam Infrastruktur

Får innehålla teknisk implementation som delas av flera moduler, till exempel registrering, konfiguration och adapters.

Produktnära regler eller flöden ska inte placeras här bara för att de används från flera skärmar.

### Modulregistrering Och UI Composition

Nya målmoduler registreras via statiska `IServiceCollection`-extensions som får portalens `AppRegistrationContext`.

`AppRegistrationContext` är den gemensamma samlingspunkten för modulens bidrag till:

- dependency injection
- app shell och settings-UI composition
- access claims
- authorization policies

Äldre `IFeature`-registrering får finnas kvar under migrationen, men är en separat kompatibilitetsväg och ska inte vara modellen för ny modulär kod.

Navigation i app shell och settings areas ska registreras som data, till exempel text, route, order, ikon och policy. Settings area-registrering ska ange en explicit module key så Settings Hub kan hålla en synlig settings-ingång per modul.

## Beroenderiktningar

Rekommenderad riktning:

```text
Web
  -> Modules
  -> Shared UI

Modules
  -> SharedKernel
  -> avsiktliga modulkontrakt

Infrastructure
  -> modulkontrakt
  -> externa paket
```

Moduler ska undvika att nå in i varandras interna implementationer.

När en modul behöver något från en annan modul ska det ske via ett avsiktligt kontrakt eller ett use case, inte via intern persistens- eller UI-struktur.

## Frontendstrategi

### Nu: Blazor Server

Blazor Server behålls som snabbaste utvecklingsmiljö under den nuvarande fasen.

Komponenterna kan ligga kvar i Blazor-projektet under övergången, men de ska stegvis göras tunnare.

### Senare: API Och Utbytbar Frontend

När produktmodellen stabiliserats bör use cases kunna exponeras via HTTP API eller motsvarande tydliga backend-gränssnitt.

Det ska möjliggöra framtida klienter som:

- Blazor WASM
- React
- andra klienter

De praktiska kodreglerna för hur vi stödjer detta redan nu hör hemma i [../development.md](../development.md).

## Öppna Arkitekturfrågor

- när lösningen, om någonsin, bör delas i fler deploybara delar
- hur långt modulernas interna projektuppdelning ska drivas under migreringen
- vilka backend-gränssnitt som först bör exponeras för framtida alternativa klienter

## Slutsats

Bygg vidare som Blazor Server nu, men strukturera koden som en modulär monolit med tydliga moduler, use cases och kontrakt.

Det ger snabb utveckling idag och en tydligare väg mot en mer utbytbar frontend och renare ansvarsfördelning över tid.
