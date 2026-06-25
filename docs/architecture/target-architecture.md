# Målarkitektur

## Syfte

Det här dokumentet beskriver den övergripande målarkitekturen för Kalkylinsiktsportalen.

Det fokuserar på arkitekturriktning, modulgränser, beroenderiktningar och ansvarsfördelning mellan större delar av systemet.

Det här dokumentet äger inte den detaljerade domänmodellen. Aktuella domänbegrepp, aggregates, relationer, invariants och modulnära policyer ägs av `docs/domain/*.md`. Om detta dokument och `docs/domain/*.md` pekar i olika riktning ska domändokumenten betraktas som den mer aktuella källan. Långlivade och svårbackade trade-offs dokumenteras i ADR:er.

Den sammanfattande riktningen är: modulär monolit som systemform, vertical slices som huvudsaklig arbetsform, pragmatisk CQRS för nya use cases, clean architecture som beroendeprincip och DDD-light för domänmodellering där det ger verkligt värde.

Målbilden utgår från produktinriktningen:

1. rapportportal
2. projekt- och versionshantering
3. kalkylutforskare

Blazor Server används fortsatt som första UI-host, men backend, kontrakt och affärslogik ska kunna bära framtida klienter som Blazor WASM eller React.

## Arkitekturprinciper

### 1. Modulär monolit först

Systemet utvecklas som en modulär monolit innan det eventuellt delas upp i fler deploybara delar.

Det innebär att:

- lösningen kan köras som en applikation
- koden organiseras i tydliga moduler
- modulgränser respekteras även innan de är separata projekt
- externa integrationer och persistence ligger bakom avsiktliga kontrakt

### 2. Produktområden och use cases före global lagerstruktur

Lösningen ska i första hand organiseras runt produktområden och användningsfall, inte runt en global teknisk lagerindelning.

Moduler får ha interna lager när det hjälper, till exempel `Domain`, `Application`, `Infrastructure` och `Components`, men tomma eller symmetriska lager ska inte skapas utan behov.

Vertical slices är den praktiska arbetsformen för ny produktlogik. Clean architecture används som riktning för beroenden, inte som krav på en global mappstruktur.

Pragmatisk CQRS är den föredragna formen för nya feature slices: kommandon uttrycker förändringsintention, queries uttrycker läsintention, och handlers utgör en tydlig testyta utan att lösningen för den skull behöver ett tungt mediator- eller pipeline-ramverk.

### 3. UI är utbytbart

Blazor-komponenter ska inte vara platsen där central produktlogik bor.

UI ska främst:

- rendera data
- hantera användarinteraktion
- anropa use cases
- visa laddning, fel och resultat

Produktlogik ska kunna återanvändas av flera klienter, tester och andra gränssnitt.

### 4. Tydliga ägargränser

Varje viktig del av produktlogiken ska ha en tydlig modulägare.

Tvärgående kod ska hållas liten och stabil. `Shared` är inte en defaultplats för otydligt ägarskap.

### 5. Organisation är hård systemgräns

Organisationen är den grundläggande tenant-gränsen i systemet.

Data, rättigheter, integrationer och konfiguration ska utformas så att organisationsgränsen förblir tydlig och genomgående.

## Huvudmoduler

### Projects

Projects äger projektarbetsytan och dess förändringsbara arbetsflöden.

Ansvar:

- projekt och versionsflöden
- snapshots och projektbundna arbetslägen
- aktiv kalkylstate som andra moduler kan konsumera
- regler och use cases för projektbundna förändringar ovanpå importerad data

### Reports

Reports äger rapportvyer, presentation och export.

Ansvar:

- view-definitioner och rapportstruktur
- rendering av rapporter från modulkontrakt
- export och layoutrelaterade beslut
- livscykel för rapportvyer

### Bidcon

Bidcon äger integrationen mot Bidcon som extern källa.

Ansvar:

- API-klient och autentisering
- import av extern estimationsdata
- översättning från externa kontrakt till interna importkontrakt

### Access

Access äger portalens behörighetsmodell.

Ansvar:

- funktionella rättigheter
- view-rättigheter
- roll- och användarbaserad access
- översättning från identity till portalens accessmodell

### Organizations

Organizations äger tenant-kontexten.

Ansvar:

- aktuell organisation
- organisationsbundna inställningar
- tenant-isolering
- organisationsbundna integrationsförutsättningar

## Shared och gemensamma byggblock

Gemensamma byggblock ska hållas små, stabila och avsiktliga.

### SharedKernel

Får innehålla små och stabila begrepp som används över modulgränser, till exempel identiteter och enkla kontraktstyper.

SharedKernel ska vara litet och får inte bli en plats för produktflöden.

### Shared UI

Får innehålla generella UI-primitiver utan produktspecifik affärslogik.

### Gemensam infrastruktur

Får innehålla teknisk implementation som delas av flera moduler, till exempel registrering, konfiguration och adapters.

Produktnära regler eller flöden ska inte placeras här bara för att de används från flera skärmar.

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

### Senare: API och utbytbar frontend

När produktmodellen stabiliserats bör use cases kunna exponeras via HTTP API eller motsvarande tydliga backend-gränssnitt.

Det ska möjliggöra framtida klienter som:

- Blazor WASM
- React
- andra klienter

För att stödja det redan nu:

- använd request/response-modeller för use cases när det är rimligt
- undvik att affärslogik kräver Blazor-specifika typer
- undvik att UI-komponenter pratar direkt med databaser eller externa API-klienter
- låt frontend anropa applikationstjänster och use cases

## Öppna arkitekturfrågor

- när lösningen, om någonsin, bör delas i fler deploybara delar
- hur långt modulernas interna projektuppdelning ska drivas under migreringen
- vilka backend-gränssnitt som först bör exponeras för framtida alternativa klienter

## Slutsats

Bygg vidare som Blazor Server nu, men strukturera koden som en modulär monolit med tydliga moduler, use cases och kontrakt.

Det ger snabb utveckling idag och en tydligare väg mot en mer utbytbar frontend och renare ansvarsfördelning över tid.
