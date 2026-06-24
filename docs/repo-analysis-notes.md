# Repoanalys: Kalkylinspektionsportalen

Datum: 2026-06-24

Det här dokumentet är ett arbetsunderlag efter en ny genomgång av den uppdaterade
lokala koden. Syftet är att samla observationer som kan användas när de mer
långlivade dokumenten i `src/docs/` utvecklas vidare.

## Sammanfattning

Repot har gått från att vara relativt odokumenterat till att redan ha en tydlig
dokumentationsstruktur under `src/docs/`. Det finns nu dokument för produkt,
domän, arkitektur, utvecklingsriktlinjer, agentarbete och förbättringsområden.

Den aktuella koden och dokumentationen pekar mot ett tydligare produktnamn och
syfte: **Kalkylinspektionsportalen**, en Blazor Server-applikation för att hämta,
strukturera och presentera Bidcon-kalkyldata i anpassningsbara rapportvyer.

Den största skillnaden mot tidigare analys är att repoet nu redan innehåller:

- README-filer som beskriver produkten och dokumentationsprincipen.
- Ett `src/docs/`-lager med arkitektur-, domän- och utvecklingsdokument.
- Ett testprojekt: `src/Tests/BlazorServer.Tests`.
- Mer utvecklad view-/rapportlogik, inklusive editor-klasser och formelutvärdering.
- En säkrare placeholder för `AzureAd:ClientSecret` i `appsettings.json`.

## Nuvarande repoidentitet

### Produktnamn

De senaste README- och domändokumenten använder **Kalkylinspektionsportalen**.
Det bör ses som nuvarande arbetsnamn.

Tidigare namn och repo-/lösningsnamn innehåller fortfarande variationer:

- `Kalkylinsiktsportalen`
- `Kalkylinspektionsportalen`
- Git-remote/reponamn: `AppPlatform`

Det här är värt ett litet namn-/identitetsbeslut längre fram, så att repo,
produkt, solution och dokumentation inte driver åt olika håll.

### Lösningsstruktur

Nuvarande huvuddelar:

- `src/BlazorServer` - huvudapplikationen.
- `src/Tests/BlazorServer.Tests` - NUnit-testprojekt för huvudappen.
- `src/PwCracker` - separat konsolprojekt i samma lösning.
- `src/docs` - långlivat dokumentationslager.
- `docs/repo-analysis-notes.md` - denna tillfälliga/metaanalytiska arbetsfil.

## Produkt- och domänbild

Den dokumenterade produkten handlar om att göra kalkylinformation från Bidcon
användbar utanför källsystemet.

Kärnan är:

- Bidcon som extern datakälla.
- Egna views/rapportvyer som presentationslager.
- Organisationsbunden konfiguration.
- Rättighetsstyrd administration och användning.

Den långsiktiga målbilden är bredare än en ren rapportportal. Dokumentationen
beskriver en framtida riktning med projekt, flera kalkyler per projekt,
versionshistorik, delning och jämförelse mellan versioner.

Det gör att produkten bör beskrivas som ett verksamhetsnära
presentations-, styrnings- och analyslager ovanpå kalkyldata, inte som ett
ersättningssystem för Bidcon.

## Arkitektoniska mönster

### Feature-baserad struktur

Applikationen organiseras fortfarande runt features. Varje feature implementerar
`IFeature` och kan bidra med:

- services
- renderbara komponenter
- claims
- authorization policies

Observerade features:

- `Settings`
- `Access`
- `BidconAccess`
- `BidconBrowser`
- `ViewSettings`

Detta är ett starkt mönster i repot och bör fortsätta dokumenteras som
standardvägen för ny produktfunktionalitet.

### Central composition root

`ServiceAndFeatureRegistrations.cs` fungerar som composition root för:

- auth-läge per environment
- feature-registrering
- HTTP-klient för Bidcon
- MongoDB-registrering
- claims/cache/auth-services
- MudBlazor och localization

`Program.cs` är relativt tunn och delegerar huvuddelen av appens sammansättning
till denna registreringsfil.

### UI-komposition via RenderCollection

Features registrerar komponenttyper i `RenderCollection`, som sedan används av
layout och settings-skal. Det ger en modulär navigation och gör att features kan
exponera UI utan att layouten känner till varje feature direkt.

Det här är ett viktigt repo-mönster och bör beskrivas i
`src/docs/architecture/features-and-modules.md`.

### Auth och access

Auth-modellen har två driftlägen:

- Development: mockad auth, mockade user/role-services.
- Production: Microsoft Identity Web, OpenID Connect och Microsoft Graph.

Intern åtkomst styrs med application claims. Features kan registrera claim-info
och policies. Claims laddas/transformeras via middleware och cache.

Denna modell är central nog för en egen arkitektursektion eller ADR.

### Persistens med stores och MongoDB

Persistens går via store-interface i `Shared/Stores` och MongoDB-implementationer
i `Shared/Stores/MongoDb`.

Mönstret är:

- store-interface
- Mongo-store
- Mongo-entitet
- mapper mellan DTO/modell och Mongo-entitet
- Mongo collection-registrering i `Registrations.cs`

Detta är tydligt och dokumentationsvärt, men `Shared/Stores/MongoDb/Enteties`
har fortfarande stavfel i namn och namespace.

## View- och rapportmönster

Den största kodmässiga utvecklingen i senaste versionen ligger runt views och
rapportredigering.

Nya eller tydligare byggblock:

- `ViewEditor`
- `DataSectionEditor`
- `DataCellFormulaEvaluator`
- `FormatBuilder`
- `DataSectionComponent`
- `DataCellComponent`
- `CellFormatEditor`
- `TextFormatEditor`

### ViewEditor

`ViewEditor` hanterar en `ViewDto` och ansvarar för att:

- normalisera sektioner
- lägga till data-, sheet- och page break-sektioner
- ta bort sektioner
- hålla `SectionIndex` konsekvent
- returnera sektioner i ordning

Det visar att view-modellen börjar få domänliknande beteende, även om klassen
just nu ligger under `Shared/Components/Views`.

### DataSectionEditor

`DataSectionEditor` kapslar grid-/tabellbeteende för datasektioner:

- initiering av rader och kolumner
- tillägg/borttag av rader och kolumner
- cellskapande
- normalisering av celler och definitioner
- stabila cellnycklar som `R{row}C{column}`

Detta är testbar logik och bör på sikt få mer testtäckning.

### DataCellFormulaEvaluator

`DataCellFormulaEvaluator` gör cellvärden mer uttrycksfulla:

- råvärden kan vara vanliga strängar
- formler börjar med `=`
- referenser till `Estimation.*` kan slås upp via reflection
- enklare aritmetik utvärderas med `DataTable.Compute`
- strängkonkatenering stöds
- tal och datum formateras via `TextFormatDto`

Den här klassen är idag den tydligaste testade domän-/presentationslogiken.
Den bör behandlas som kritisk, eftersom formler lätt blir en egen liten DSL.

## Tester

Det finns nu ett testprojekt:

- `src/Tests/BlazorServer.Tests`
- NUnit
- referens till `BlazorServer`
- testtäckning för `DataCellFormulaEvaluator`

Detta är en viktig förbättring jämfört med tidigare läge. Testytan är dock smal.
Nästa rimliga testkandidater är:

- `DataSectionEditor`
- `ViewEditor`
- Mongo-mappers
- authorization policies
- access- och claimlogik
- formatbyggande om CSS-output blir affärskritisk

## Befintligt dokumentationslager

`src/docs/` är nu den primära platsen för långlivad dokumentation.

Viktiga filer:

- `src/docs/domain/product-description-draft.md`
- `src/docs/domain/overview.md`
- `src/docs/domain/glossary.md`
- `src/docs/domain/domain-map.md`
- `src/docs/architecture/overview.md`
- `src/docs/architecture/features-and-modules.md`
- `src/docs/development/coding-guidelines.md`
- `src/docs/development/local-development.md`
- `src/docs/backlog/improvement-areas.md`
- `src/docs/backlog/open-questions.md`
- `src/docs/agents/repository-context.md`

Dokumentationsprincipen är redan beskriven: README ska vara korta ingångar,
medan produkt, domän, arkitektur, utveckling och backlog bor i separata
underkataloger.

Det här analysdokumentet bör därför inte bli en permanent parallell sanning.
När punkterna är genomarbetade bör de flyttas in i rätt `src/docs/`-dokument.

## Kvarvarande risker och förbättringsområden

Många förbättringsområden finns redan i
`src/docs/backlog/improvement-areas.md`. Nedan är en uppdaterad tolkning efter
senaste kodgenomgången.

### 1. Produktnamn och repoidentitet

Det finns fortfarande flera namn i omlopp. Bestäm gärna ett officiellt namn och
hur repo-/solutionnamn ska förhålla sig till det.

### 2. Secrets och konfiguration

`appsettings.json` innehåller nu placeholdern `__SET_IN_USER_SECRETS_OR_ENV__`,
vilket är bättre än en faktisk hemlighet. Eftersom nyckeln fortfarande finns i
filen bör setupen dokumenteras tydligt i local development-dokumentet.

### 3. `.Result` i async-flöde

`BidconApiAccess` innehåller fortfarande `.Result` i tokenhämtningen. Det bör
ersättas med en ren `await`-kedja.

### 4. Feature-specifik logik i Shared

`Shared/Components/Views` innehåller mycket view-/rapport-specifik logik.
Det kan vara rimligt under MVP, men långsiktigt bör man besluta om detta är ett
verkligt shared område eller egentligen tillhör `ViewSettings` eller en framtida
`Reports`/`Views`-feature.

### 5. Formelspråkets säkerhet och avgränsning

`DataCellFormulaEvaluator` använder `DataTable.Compute` och reflection mot
`EstimationDto`. Det fungerar för nuvarande behov men bör få tydliga
begränsningar:

- vilka uttryck är tillåtna?
- vilka `Estimation`-fält får refereras?
- hur ska fel visas för användaren?
- ska formler valideras vid inmatning eller bara vid rendering?
- ska detta vara kompatibelt med ett framtida export-/printflöde?

Detta är en bra ADR-kandidat.

### 6. FeatureRegistrator bygger temporär service provider

`FeatureRegistrator` bygger en service provider under registrering. Det är
dokumenterat i backloggen och bör ses över efter MVP eller när DI-livscykler
börjar bli svåra att resonera om.

### 7. Namngivning och stavning

Kvarvarande exempel:

- `IFreature`
- `Enteties`
- `MongoRoleAccssMapper`
- `Pageination`
- `AppNavMenue`
- `HasThoasandsSeparator`
- `claimsTransofmation`
- interna fält som `__sharedSettingsCompoenents`

Det här är inte nödvändigtvis MVP-blockerande men påverkar onboarding,
sökbarhet och agentarbete.

### 8. PwCracker i lösningen

`PwCracker` ligger kvar som separat konsolprojekt. Det bör finnas ett tydligt
beslut om det är ett legitimt utvecklingsverktyg i repot eller om det ska bort.

## ADR-kandidater

Följande ADR:er är mest relevanta efter senaste genomgång:

1. Använd Blazor Server som applikationsmodell.
2. Använd feature-registrering via `IFeature`.
3. Använd claims-baserad intern accessmodell ovanpå Microsoft Identity.
4. Använd MongoDB och store/mapping-mönster för persistens.
5. Behandla Bidcon som externt källsystem och portalen som presentationslager.
6. Definiera view/rapportmodellen som produktens interna presentationsmodell.
7. Avgränsa cellformler: syntax, säkerhet, felhantering och framtida DSL-riktning.
8. Bestäm om view-/rapportlogik ska ligga i `Shared/Components/Views` eller i en egen feature.
9. Bestäm repo-/produktnamn och hur gammalt `AppPlatform`-arv ska hanteras.
10. Bestäm om `PwCracker` ska ligga kvar i lösningen.

## Rekommenderade nästa steg

1. Flytta in relevanta observationer från detta dokument i rätt `src/docs/`-fil.
2. Uppdatera `src/docs/architecture/features-and-modules.md` med view-editor/formel-mönstret.
3. Lägg till tester för `ViewEditor` och `DataSectionEditor`.
4. Ersätt `.Result` i `BidconApiAccess` med full async/await.
5. Skriv en kort ADR om cellformler innan formelspråket växer.
6. Bestäm produkt-/repo-namngivning.
7. Besluta om `PwCracker` ska dokumenteras, flyttas eller tas bort.

## Kort slutsats

Repot är nu mycket mer moget än vid första analysen. Det har en tydlig
dokumentationsstruktur, ett första testprojekt och en mer konkret produktvision.

Den viktigaste arkitektoniska frågan framåt är inte längre "vad är systemet?",
utan hur produktens kärnmodell ska stabiliseras: views, rapporter, celler,
formler, Bidcon-data, projekt och versioner. Det är där nästa dokumentations- och
designarbete bör lägga mest energi.
