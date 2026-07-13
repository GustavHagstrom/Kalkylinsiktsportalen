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

Tvärgående kod ska ägas av en av de avsiktliga top-level-gränserna: `Composition`, `Kernel`, `AppShell`, `UiKit` eller `Infrastructure`. Produktnära kod ägs av en modul.

### 5. Organisation Är Hård Systemgräns

Organisationen är den grundläggande tenant-gränsen. Data, rättigheter, integrationer och konfiguration ska utformas så att organisationsgränsen förblir tydlig.

## Målstruktur

```text
Composition/
  Äger gemensam bootstrap-state, bidragsmodeller och registries som används
  när hosten, AppShell och produktmoduler sätts samman.

AppShell/
  Äger konkreta portalytor: layout, navigation rendering, shell-pages,
  loginpresentation, aktuell användarsession och portalpreferenser.

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

`Composition` införs tills vidare som top-level-mapp och namespace i samma assembly. Den ska kunna extraheras till ett eget projekt senare utan att dess ansvar eller beroenderiktning behöver ändras.

`Kernel` får inte bero på `Composition`, `Modules`, `AppShell`, `UiKit` eller `Infrastructure`.

Moduler får inte bero på `AppShell`, inte heller på en publik del av AppShell. När en modul behöver synas i portalen bidrar den metadata genom den neutrala `Composition`-gränsen. AppShell konsumerar den färdiga composition-staten och äger konkreta shell-ytor.

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

### Composition

`Composition` äger den generella mekanismen för att samla modulbidrag under bootstrap. Den får innehålla `AppRegistrationContext`, UI-bidragsmodeller, navigation- och settings-registries samt generella registries för access-claim- och authorization-policy-bidrag.

`Composition` äger inte produktsemantik eller runtime-accessbeslut. En modul äger betydelsen av sina bidrag och `Access` äger tolkning av portalclaims, policies och accessbeslut.

`Composition` får inte bero på `Modules`, framtida `*.Contracts`-projekt eller `AppShell`. Detta förhindrar cirkulära projektreferenser vid en framtida projektuppdelning.

### Kernel

Får innehålla små och stabila systemövergripande typer, till exempel starka identiteter och minimala CQRS-kontrakt. Att en typ används av två moduler är inte i sig skäl att lägga den i Kernel.

### UiKit

Får innehålla generella UI-primitiver utan produktspecifik logik, modulkunskap, routeägarskap eller shellbeteende.

### Top-level Infrastructure

Får innehålla neutral teknisk implementation som flera ägare kan använda. Den får inte känna till modulernas entities, stores, collection names eller externa integrationsmodeller.

Varje modul äger sin egen persistence-implementation, mappings, collections och registrering. Modulintern `Infrastructure` får bero på modulens egna kontrakt och på neutral top-level-infrastruktur.

## Modulregistrering Och UI Composition

Målmoduler registreras explicit genom statiska `IServiceCollection`-extensions och bidrar metadata till `AppRegistrationContext`. Det historiska beslutet om registreringsformen finns i [../adr/global/0040-use-static-module-registration-functions.md](../adr/global/0040-use-static-module-registration-functions.md), medan ägarskap och beroenderiktning för composition beskrivs i [../adr/global/0043-introduce-dedicated-composition-boundary.md](../adr/global/0043-introduce-dedicated-composition-boundary.md).

`AppRegistrationContext` behåller sitt namn och fungerar som en samlad bootstrap-fasad för UI-, access-claim- och authorization-policy-bidrag. Den delas inte upp förrän ett konkret behov motiverar det.

Navigation och settings areas registreras som data. Loginpresentation är inte ett modulbidrag och ägs av AppShell/hostens auth-konfiguration.

`Composition` samlar bidragen, men hostens composition root i `ServiceAndFeatureRegistrations` äger hela livscykeln: skapa context, registrera AppShell och moduler, välja hostspecifik auth/loginpresentation och applicera den färdiga staten på DI och ASP.NET Core Authorization. Ingen separat `Bootstrap`-gräns införs.

## Authentication Och Authorization

Ansvar delas vid en tydlig gräns:

- AppShell mappar generell extern identitet till en smal aktuell användarsession och äger loginpresentation.
- Composition äger generiska bootstrap-modeller och registries för claim- och policybidrag.
- Access tolkar portalclaims, policies, application-admin och accessbeslut.
- Featurekod konsumerar sessions- och authorization-kontrakt i stället för att tolka `ClaimsPrincipal` direkt.
- Råa Entra- och Microsoft Graph-modeller hålls privata i integrationsadaptern.

## Beroenderiktningar

```text
Web / composition root
  -> Composition
  -> Modules
  -> AppShell
  -> UiKit
  -> Infrastructure

Modules
  -> Composition
  -> UiKit
  -> Kernel
  -> neutral Infrastructure
  -> avsiktliga publika modulkontrakt

AppShell
  -> Composition
  -> UiKit
  -> Kernel
  -> avsiktliga publika modulkontrakt

Composition
  -> frameworktyper som krävs av bidragsmodellerna
  -X-> Modules
  -X-> AppShell

Infrastructure
  -> Kernel, vid behov
  -> externa paket

Kernel
  -X-> Composition
```

Moduler får inte nå in i varandras interna use cases, persistence, integrationer eller UI. Cross-module-interaktion sker genom den ägande modulens avsiktliga yta under `Modules/{Module}/Public`.

## Frontendstrategi

Blazor Server behålls som snabbaste utvecklingsmiljö i nuvarande fas. Komponenterna ska stegvis göras tunnare och use cases ska kunna exponeras genom HTTP API eller andra adapters när produktmodellen stabiliserats.

## Öppna Arkitekturfrågor

- när lösningen, om någonsin, bör delas i fler deploybara delar
- hur långt modulernas interna projektuppdelning ska drivas
- när `Composition` ska extraheras till ett eget projekt
- vilka backend-gränssnitt som först bör exponeras för alternativa klienter

## Slutsats

Bygg vidare som Blazor Server, men håll varje typ hos en explicit ägare. Moduler bidrar genom en neutral Composition-gräns och får aldrig bero på AppShell. Det ger snabb utveckling idag och en tydlig väg mot projektbaserade moduler, utbytbara klienter och renare ansvarsfördelning.