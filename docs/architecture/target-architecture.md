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

## Vad Riktningen Betyder I Praktiken

### Modulär Monolit Som Systemform

Systemet deployas och körs som en sammanhållen applikation, men koden ska behandlas som flera interna produktmoduler med tydliga ägare. Modulgränser är alltså en designregel redan innan modulerna eventuellt blir separata projekt eller deploybara tjänster.

En modul äger sin domänlogik, sina use cases, sin persistence, sina integrationer, sitt feature-UI och sina avsiktliga publika kontrakt. Annan kod får bara använda modulens publika yta, inte dess interna implementation.

Varje modul äger sina egna collection names, dokumentformer, mappings och store-implementationer. Top-level `Infrastructure` får tillhandahålla neutral teknik som Mongo-klient, conventions och serialization, men får inte äga modulernas datamodeller eller produktsemantik.

Cross-module-interaktion ska gå via den ägande modulens avsiktliga publika yta, till exempel kontrakt under `Modules/{Module}/Public`. Andra moduler får inte anropa interna handlers, stores, adapters, components eller domäntyper bara för att de är nåbara i samma assembly.

Modulär monolit betyder inte att all kod får dela datamodeller, stores eller hjälpklasser fritt bara för att den ligger i samma process.

### Vertical Slices Som Huvudsaklig Arbetsform

Nya features organiseras i första hand som vertical slices inom den modul som äger beteendet. En slice samlar use case-koden kring ett konkret användningsfall och är UI-oberoende.

En feature slice har minst en tydlig användningsfallsgräns och en primär körbar ingång, normalt en command/query och en handler. När behov finns kompletteras slicen med result-typ, validator, slice-nära store-port, adapter och tester. Alla delar ska inte skapas av vana; de ska finnas när de gör användningsfallet tydligare, testbart eller adapteroberoende.

Handlers returnerar normalt slice-ägda result contracts i stället för domänentiteter direkt. Domänmodellen ska inte behöva formas av UI-, API- eller serializationbehov, och klientkontrakt ska kunna hållas stabila även när den interna modellen ändras. Helt intern modulnära kod får returnera domäntyper när det är lokalt, avsiktligt och inte etablerar ett externt kontrakt.

Validators i en slice ansvarar för request- och input-validering, till exempel obligatoriska fält, format och enkla gränsvärden. Affärsinvarianter som måste gälla oavsett klient eller use case hör hemma i domänmodellen och ska inte bara skyddas av en validator.

Store-portar ligger nära slicen när de bara behövs av ett användningsfall. När flera slices i samma modul behöver samma persistence-förmåga kan porten flyttas upp till en avsiktlig modulnivå. Store-implementationer, dokumentmodeller och mappings ligger fortsatt i modulens `Infrastructure`.

Handlern är den primära testytan för use case-beteende i en slice. Domänmodellen testas separat när den bär affärsregler eller invariants. UI-komponenttester ska fokusera på presentation och interaktion, inte vara den enda verifieringen av affärsregler.

Blazor-komponenter får inte placeras i feature slices. UI-komponenter hör hemma i modulens separata komponent-/UI-yta och anropar slicens use case-kontrakt. Slice-resultat får vara klientvänliga men inte Blazor-specifika. View models, komponent-state, tabell-state, dialog-state, rendering helpers och annan presentationsnära state hör hemma på UI-sidan.

Den här gränsen gör att slicen kan återanvändas av andra klienter, till exempel React, HTTP endpoints eller tester, utan att dra med sig Blazor.

Delad domänlogik som behövs av flera slices inom samma modul hör hemma i modulens `Domain`. Slice-specifik logik ska stanna i slicen tills den faktiskt behöver delas.

Vertical slices betyder inte att varje teknisk typ måste dupliceras per feature. Det betyder att användningsfallet är den primära samlingspunkten och att breda horisontella `Application`-lager inte är målformen för ny produktkod.

### Pragmatisk CQRS För Nya Feature Slices

Nya feature slices använder normalt separata commands och queries när det finns tydliga förändrings- respektive läsintentioner. Commands beskriver avsikten att ändra systemets tillstånd. Queries beskriver avsikten att läsa fram data utan sidoeffekter. Handlers är den primära testytan för modulens use case-beteende.

CQRS används här för tydligare intention, smalare kontrakt och bättre testbarhet. Det kräver inte separat read database, event sourcing, mediator-ramverk eller generell pipeline. Sådant införs först när upprepad orchestration eller verkliga prestanda-/skalningsbehov motiverar det.

För trivial read-only kod, enkel intern logik eller små ändringar nära äldre kod får enklare former användas. Avvikelsen ska vara lokal och begriplig; ny produktlogik som etablerar ett användningsfall bör däremot formas som en command- eller query-slice.

### Clean Architecture Som Beroendeprincip

Clean architecture används som beroendeprincip, inte som obligatorisk global lagerstruktur. Domänmodell och use case-beteende ska kunna förstås, testas och utvecklas utan beroenden på Blazor, MongoDB, Microsoft Graph, HTTP, sessions-UI eller andra tekniska adapters.

Inom en modul får mappar som `Domain`, `Features`, `Infrastructure` och `Components` användas när de gör ägarskap och beroenden tydligare. Små slices behöver inte skapa alla lager i förväg, men beroenden ska fortfarande peka inåt mot stabilare policy och utåt mot adapters via avsiktliga kontrakt.

Clean architecture betyder inte att varje modul behöver en egen abstraktion för varje teknisk detalj. Abstraktioner ska introduceras när de skyddar ett use case, bevarar modulgränsen eller gör testbarheten väsentligt bättre.

### DDD-Light Där Domänmodellering Ger Verkligt Värde

DDD-light betyder att domänspråk, ägarskap och affärsinvarianter modelleras noggrant där de påverkar beteende, beslut eller samarbete mellan moduler. Domäntermer ska hållas konsekventa med `CONTEXT.md` och fördjupas i `docs/domain/*.md` när en modul behöver mer precision.

När ett område har verkliga regler ska modellen uttrycka dem genom tydliga typer, value objects, entities, aggregates eller domain services efter behov. Use cases ska be domänmodellen göra affärsbeslut i stället för att sprida regler i UI, persistence eller integrationer.

DDD-light betyder inte att varje datatyp måste bli en aggregate root eller att CRUD-, integrations- och presentationsnära kod ska få onödig domänceremoni. Där beteendet är enkelt räcker enklare modeller, så länge språket är tydligt och ägarskapet är explicit.

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

Tvärgående kod ska ägas av en av de avsiktliga top-level-gränserna: `Composition`, `Identity`, `Kernel`, `AppShell`, `UiKit` eller `Infrastructure`. Produktnära kod ägs av en modul.

### 5. Organisation Är Hård Systemgräns

Organisationen är den grundläggande tenant-gränsen. Data, rättigheter, integrationer och konfiguration ska utformas så att organisationsgränsen förblir tydlig.

## Målstruktur

```text
Composition/
  Composition äger gemensam bootstrap-state och registries. Generella bidragsmodeller kan ägas av Composition, medan produktspecifika bidragskontrakt ägs av den modul som definierar deras betydelse

Identity/
  Äger gemensamma identitetsvärden och ett teknikneutralt kontrakt
  för aktuell användare i en aktiv organisationskontext.

AppShell/
  Äger konkreta portalytor, authentication-adaptern, sessionslivscykel,
  loginpresentation och portalpreferenser.

Infrastructure/
  Äger neutral teknisk infrastruktur, till exempel Mongo-klient,
  databas-provider, serializers och conventions.

Kernel/
  Äger små, stabila och teknikneutrala systemprimitiver utan tydligare ägare.

Modules/
  Äger produktförmågor, use cases, persistence, integrationer,
  feature-UI och avsiktliga publika kontrakt.

UiKit/
  Äger neutrala UI-primitiver utan modul-, domän-, route- eller shellkunskap.
```

`Composition` införs tills vidare som top-level-mapp och namespace i samma assembly. Den samlar generella bidrag från moduler och AppShell genom `AppRegistrationContext`, men äger ingen produktsemantik eller konkret shell-presentation.

`AppRegistrationContext` förblir en samlad bootstrap-fasad för UI-, access-claim- och authorization-policy-bidrag. Access äger betydelsen och runtime-tolkningen av authorization-bidrag; AppShell äger loginpresentationen; hostens composition root applicerar den färdiga registreringsstaten på DI och ASP.NET Core.

`Identity` är en neutral runtime-gräns, inte en authentication- eller authorization-modul. Den äger `UserId`, `OrganizationId`, `CurrentUser` och det läsbara `ICurrentUserContext`. En `CurrentUser` finns bara i en aktiv organisationskontext. Claims, refresh, providerintegration och accessbeslut ligger utanför gränsen.

## Målmoduler

- `Projects` äger projekt, versioner, snapshots och aktiv renderbar kalkylstate.
- `Reports` äger views, rapportstruktur, rendering och export.
- `Bidcon` äger extern Bidcon-integration, credentials, import och integrations-DTO:er.
- `Access` äger portalclaims, authorization policies, application-admin-semantik, grants och accessbeslut.
- `Organizations` äger tenant-kontext och organisationsinställningar.

Den detaljerade modellen inom varje modul beskrivs i `docs/domain/*`.

## Övergång Från Äldre Feature-Struktur

Äldre produktkod kan fortfarande ligga i `Features/*`. För små ändringar kan det vara rätt att arbeta nära befintlig kod, men ny eller flyttad production-kod ska placeras hos en explicit ägare.

Målarkitekturen är normativ för ny eller flyttad production-kod från och med nu. Det innebär att nya användningsfall ska följa modulägarskap, vertical slices, pragmatisk CQRS, clean architecture som beroendeprincip och DDD-light där domänen motiverar det.

Små ändringar i äldre `Features/*` behöver inte starta en modulmigrering. De ska däremot inte introducera nya beroenden, delade modeller, stores eller UI-bunden affärslogik som går emot målarkitekturen.

Ingen production-kod får ligga i en `Shared`-katalog eller använda namespace `BlazorServer.Shared`.

| Äldre feature | Målmodul/riktning |
|---|---|
| `Settings` | `AppShell` / neutral `UiKit` |
| `Access` | `Access` |
| `BidconAccess` | `Bidcon` + `Organizations` |
| `BidconBrowser` | `Bidcon`, senare delvis `Projects` |
| `ViewSettings` | `Reports` |

## Gemensamma Byggblock

### Identity

Får innehålla gemensamma identitetsvärden, neutrala identitetsclaim-namn, helpers som projicerar `ClaimsPrincipal` till identitetsvärden och smala, teknikneutrala runtime-kontrakt. Den får inte innehålla access-claim-tolkning, authentication providers, sessionsuppdatering, login-UI eller authorization-regler.

### Kernel

Får innehålla små och stabila systemövergripande typer utan tydligare ägare, till exempel minimala CQRS-kontrakt. Att en typ används av två moduler är inte i sig skäl att lägga den i Kernel.

### UiKit

Får innehålla generella UI-primitiver utan produktspecifik logik, modulkunskap, routeägarskap eller shellbeteende.

### Top-level Infrastructure

Får innehålla neutral teknisk implementation som flera ägare kan använda. Den får inte känna till modulernas entities, stores, collection names eller externa integrationsmodeller.

Varje modul äger sin egen persistence-implementation, mappings, collections och registrering. Modulintern `Infrastructure` får bero på modulens egna kontrakt och på neutral top-level-infrastruktur.

## Modulregistrering Och UI Composition

Målmoduler registreras explicit genom statiska `IServiceCollection`-extensions och bidrar metadata till `AppRegistrationContext`. Registreringsformen beskrivs i [ADR-0040](../adr/global/0040-use-static-module-registration-functions.md) och ägarskapet för composition-kontrakten i [ADR-0043](../adr/global/0043-introduce-dedicated-composition-boundary.md).

Navigation och settings areas registreras som data. Loginpresentation är host-/AppShell-ansvar och inte ett modulbidrag.

## Identity, Authentication Och Authorization

- `Identity` beskriver aktuell användares identitet och organisation genom ett läsbart, teknikneutralt kontrakt.
- AppShell mappar extern identitet till `CurrentUser` och äger authentication, refresh, sessionslivscykel och loginpresentation.
- Access tolkar portalclaims, policies, application-admin och accessbeslut.
- Featurekod konsumerar `ICurrentUserContext` och explicita authorization-kontrakt i stället för `AuthenticationStateProvider`, `ClaimsPrincipal` eller råa claims.
- Råa Entra- och Microsoft Graph-modeller hålls privata i integrationsadaptern.

## Beroenderiktningar

```text
Web / composition root
  -> Composition
  -> Identity
  -> Modules
  -> AppShell
  -> UiKit
  -> Infrastructure

Modules
  -> Composition
  -> Identity
  -> UiKit
  -> Kernel
  -> neutral Infrastructure
  -> avsiktliga publika modulkontrakt

AppShell
  -> Composition
  -> Identity
  -> UiKit
  -> Kernel
  -> avsiktliga publika modulkontrakt

Identity
  -X-> Composition
  -X-> Modules
  -X-> AppShell
  -X-> Access

Composition
  -> avsiktliga publika modulkontrakt
  -X-> modulernas interna implementation
  -X-> AppShell

Kernel
  -X-> Composition
  -X-> Identity
  -X-> Modules
  -X-> AppShell
  -X-> UiKit
  -X-> Infrastructure
```

`Composition` får bara använda modulernas avsiktliga publika registreringskontrakt under `Modules/{Module}/Public`; intern implementation och `AppShell` är fortsatt förbjudna beroenden.

Moduler får inte bero på `AppShell` eller nå in i andra modulers interna use cases, persistence, integrationer eller UI. Cross-module-interaktion sker genom den ägande modulens avsiktliga yta under `Modules/{Module}/Public`.

## Frontendstrategi

Blazor Server behålls som snabbaste utvecklingsmiljö i nuvarande fas. Komponenterna ska stegvis göras tunnare och use cases ska kunna exponeras genom HTTP API eller andra adapters när produktmodellen stabiliserats.

## Öppna Arkitekturfrågor

- när lösningen, om någonsin, bör delas i fler deploybara delar
- hur långt modulernas interna projektuppdelning ska drivas
- när `Composition` ska extraheras till ett eget projekt
- vilka backend-gränssnitt som först bör exponeras för alternativa klienter

## Slutsats

Bygg vidare som Blazor Server, men håll varje typ hos en explicit ägare och skydda modulgränserna. Det ger snabb utveckling idag och en tydlig väg mot projektbaserade moduler och utbytbara klienter.
