# Målarkitektur

## Syfte

Det här dokumentet beskriver den övergripande målarkitekturen för Kalkylinsiktsportalen.

Det fokuserar på arkitekturriktning, modulgränser, beroenderiktningar och ansvarsfördelning mellan större delar av systemet. Detaljerad domänmodell ägs av `docs/domain/*.md`, praktiska kodregler av [../development.md](../development.md) och långlivade trade-offs av ADR:er.

Den sammanfattande riktningen är:

- modulär monolit som systemform
- vertical slices som huvudsaklig arbetsform
- pragmatisk CQRS för nya feature slices
- clean architecture som beroendeprincip
- DDD-light där domänmodellering ger verkligt värde

Blazor Server används fortsatt som första UI-host, men backend, kontrakt och affärslogik ska kunna bära framtida klienter.

## Arkitekturprinciper

### 1. Modulär Monolit Först

Systemet utvecklas som en applikation med tydliga interna moduler. Modulgränser ska respekteras även innan modulerna är separata projekt. Externa integrationer och persistence ligger bakom avsiktliga kontrakt.

### 2. Produktområden Och Use Cases Före Global Lagerstruktur

Lösningen organiseras runt produktområden och användningsfall, inte runt en global teknisk lagerindelning. Moduler får interna lager som `Domain`, `Infrastructure`, `Features` och `Components` när de löser ett konkret behov.

Vertical slices är den praktiska arbetsformen. Pragmatisk CQRS används för att skilja förändrings- och läsintention utan krav på ett tungt mediator- eller pipeline-ramverk.

### 3. UI Är Utbytbart

Blazor-komponenter ska främst rendera data, hantera interaktion, anropa use cases och visa resultat. Central produktlogik ska kunna återanvändas av tester, endpoints och framtida klienter.

### 4. Tydliga Ägargränser

Varje production-typ ska ha en explicit ägare. `Shared` är avvecklat och får inte återintroduceras som generell samlingsplats.

Tvärgående kod ska ägas av en av de avsiktliga top-level-gränserna: `Kernel`, `AppShell`, `UiKit` eller `Infrastructure`. Produktnära kod ägs av en modul.

### 5. Organisation Är Hård Systemgräns

Organisationen är den grundläggande tenant-gränsen. Data, rättigheter, integrationer och konfiguration ska utformas så att organisationsgränsen förblir tydlig.

## Målstruktur

```text
AppShell/
  Äger host- och portalansvar: composition-kontrakt, layout, navigation,
  shell-pages, loginpresentation, aktuell användarsession och portalpreferenser.

Infrastructure/
  Äger neutral teknisk infrastruktur, till exempel Mongo-klient,
  databas-provider, serializers och conventions.

Kernel/
  Äger små, stabila och teknikneutrala systemprimitiver.

Modules/
  Äger produktförmågor, use cases, persistence, integrationer,
  feature-UI och avsiktliga publika kontrakt.

UiKit/
  Äger neutrala UI-primitiver utan modul-, domän-, route- eller shellkunskap.
```

`Kernel` får inte bero på `Modules`, `AppShell`, `UiKit` eller `Infrastructure`.

Moduler ska inte bero på implementation i `AppShell`. När en modul behöver synas i shell bidrar den metadata via AppShells composition-kontrakt. AppShell äger registreringsmekanismen; respektive modul äger betydelsen i sitt bidrag.

En UI-komponent hör bara hemma i `UiKit` när den är neutral och antingen har flera verkliga konsumenter eller är en avsiktlig design-systemprimitiv. Feature-specifika komponenter stannar i ägande modul.

## Målmoduler

- `Projects` äger projekt, versioner, snapshots och aktiv renderbar kalkylstate.
- `Reports` äger views, rapportstruktur, rendering och export.
- `Bidcon` äger extern Bidcon-integration, credentials, import och integrations-DTO:er.
- `Access` äger portalclaims, authorization policies, application-admin-semantik, grants och accessbeslut.
- `Organizations` äger tenant-kontext och organisationsinställningar.

Den detaljerade modellen inom varje modul beskrivs i `docs/domain/*`.

## Övergång Från Äldre Feature-Struktur

Äldre produktkod kan fortfarande ligga i `Features/*`. För små ändringar kan det vara rätt att arbeta nära befintlig kod, men ny eller flyttad production-kod ska placeras hos en explicit ägare.

Ingen production-kod får ligga i en `Shared`-katalog eller använda namespace `BlazorServer.Shared`.

| Äldre feature | Målmodul/riktning |
|---|---|
| `Settings` | `AppShell` / neutral `UiKit` |
| `Access` | `Access` |
| `BidconAccess` | `Bidcon` + `Organizations` |
| `BidconBrowser` | `Bidcon`, senare delvis `Projects` |
| `ViewSettings` | `Reports` |

## Gemensamma Byggblock

### Kernel

Får innehålla små och stabila systemövergripande typer, till exempel starka identiteter och minimala CQRS-kontrakt. Att en typ används av två moduler är inte i sig skäl att lägga den i Kernel.

### UiKit

Får innehålla generella UI-primitiver utan produktspecifik logik, modulkunskap, routeägarskap eller shellbeteende.

### Top-level Infrastructure

Får innehålla neutral teknisk implementation som flera ägare kan använda. Den får inte känna till modulernas entities, stores, collection names eller externa integrationsmodeller.

Varje modul äger sin egen persistence-implementation, mappings, collections och registrering. Modulintern `Infrastructure` får bero på modulens egna kontrakt och på neutral top-level-infrastruktur.

## Modulregistrering Och UI Composition

Målmoduler registreras explicit och bidrar metadata till AppShell genom composition-kontrakt. Den praktiska kodformen ägs av [../development.md](../development.md), och det historiska beslutet finns i [../adr/global/0040-use-static-module-registration-functions.md](../adr/global/0040-use-static-module-registration-functions.md).

`AppRegistrationContext` samlar composition-bidrag för exempelvis navigation, settings-ytor samt registrering av claims och policies. AppShell äger composition-mekanismen. Access äger authorization-vokabulären och tolkningen av claims, policies och adminrättigheter.

Navigation och settings areas registreras som data, inte som godtyckliga komponenttyper.

## Authentication Och Authorization

Ansvar delas vid en tydlig gräns:

- AppShell mappar generell extern identitet till en smal aktuell användarsession och äger loginpresentation.
- Access tolkar portalclaims, policies, application-admin och accessbeslut.
- Featurekod konsumerar sessions- och authorization-kontrakt i stället för att tolka `ClaimsPrincipal` direkt.
- Råa Entra- och Microsoft Graph-modeller hålls privata i integrationsadaptern.

## Beroenderiktningar

```text
Web
  -> Modules
  -> AppShell
  -> UiKit
  -> Infrastructure

Modules
  -> UiKit
  -> Kernel
  -> neutral Infrastructure
  -> avsiktliga publika modulkontrakt

AppShell
  -> UiKit
  -> Kernel
  -> avsiktliga publika modulkontrakt

Infrastructure
  -> Kernel, vid behov
  -> externa paket
```

Moduler får inte nå in i varandras interna use cases, persistence, integrationer eller UI. Cross-module-interaktion sker genom den ägande modulens avsiktliga yta under `Modules/{Module}/Public`.

## Frontendstrategi

Blazor Server behålls som snabbaste utvecklingsmiljö i nuvarande fas. Komponenterna ska stegvis göras tunnare och use cases ska kunna exponeras genom HTTP API eller andra adapters när produktmodellen stabiliserats.

## Öppna Arkitekturfrågor

- när lösningen, om någonsin, bör delas i fler deploybara delar
- hur långt modulernas interna projektuppdelning ska drivas
- vilka backend-gränssnitt som först bör exponeras för alternativa klienter

## Slutsats

Bygg vidare som Blazor Server, men håll varje typ hos en explicit ägare och skydda modulgränser med små publika kontrakt. Det ger snabb utveckling idag och en tydlig väg mot utbytbara klienter och renare ansvarsfördelning.