# Utveckling

Det här dokumentet samlar lokal utveckling, kodriktlinjer och praktiska arbetsregler.

Arkitekturriktning, modulgränser och beroendeprinciper ägs av [architecture/target-architecture.md](architecture/target-architecture.md). Det här dokumentet översätter den riktningen till praktiskt arbete i repoet.

## Lokal Setup

Förutsättningar:

- .NET SDK 10 som kan bygga `net10.0`
- möjlighet att köra Blazor Server lokalt

Startprojekt:

- `src/BlazorServer`

Testprojekt:

- `src/Tests/BlazorServer.Tests`

Viktiga konfigurationsområden:

- `AzureAd`
- `DownstreamApi`
- `MongoDb`
- `Bidcon`
- `ConnectionStrings`

I development används mockad auth, vilket gör lokal utveckling möjlig utan full production-inloggning. Azure/Microsoft Identity-konfiguration behövs bara när production-lik auth uttryckligen ska testas. MongoDB behövs för appkörning och persistence-flöden som använder Mongo-konfiguration.

## Vanliga Kommandon

Kör från `src` om inget annat anges.

```powershell
dotnet run --project BlazorServer
dotnet test Tests/BlazorServer.Tests
```

## Arbetsflöde

1. Starta `BlazorServer`.
2. Kontrollera att development-auth fungerar.
3. Säkerställ att MongoDB är tillgänglig när ändringen använder Mongo-baserad persistence.
4. Arbeta hos den explicita ägaren: modul, `AppShell`, `Kernel`, `UiKit` eller top-level `Infrastructure`.
5. Kör relevanta tester och arkitekturtester.
6. Uppdatera docs om ansvar, regler eller struktur ändrats.

## Kodprinciper

Gör minsta rimliga ändring:

- ändra så få filer som möjligt
- håll dig inom rätt modul eller ägargräns
- undvik bred refaktorering om den inte behövs

Följ befintlig kod när ändringen är liten:

- håll dig nära befintlig struktur när ändringen hör nära ihop med dagens kod
- återanvänd etablerade UI-mönster i Blazor Server och MudBlazor

Följ målarkitekturen när du lägger ny produktlogik:

- lägg ny produktlogik i relevant `Modules/*`
- registrera modulen via dess statiska `IServiceCollection`-extension och `AppRegistrationContext`
- strukturera modulär logik som feature/use case-slices
- skapa interna mappar som `Domain`, `Infrastructure`, `Features` och `Components` bara vid konkret behov
- exponera cross-module-ytor under `Modules/{Module}/Public`
- använd avsiktliga publika kontrakt i stället för att nå in i intern implementation

## Hårda Ägar- Och Modulregler

- ingen production-kod får ligga i en `Shared`-katalog eller använda namespace `BlazorServer.Shared`
- varje production-typ ska ägas av en modul, `AppShell`, `Kernel`, `UiKit` eller top-level `Infrastructure`
- andra moduler får bara referera till avsiktligt exponerad kod under `Modules/{Module}/Public`
- använd `Public/Contracts` för modulkontrakt och `Public/Components` för avsiktliga entry components
- moduler får inte anropa varandras interna use cases, stores, integrationsklienter eller helper components
- om en annan modul behöver beteende ska ägaren exponera ett smalare kontrakt
- råa externa DTO:er och provider-modeller ska vara privata för integrationsägande modul

## Feature Slices Och Resultattyper

- `Features/*` innehåller application use cases
- `Components/*` innehåller Blazor UI
- components får bero på features, men features får inte bero på components
- Blazor Server-components får anropa handlers direkt
- handlers ska vara fria från Blazor-, HTTP-, navigation-, dialog-, snackbar-, JS- och andra host/UI-specifika beroenden
- framtida endpoints ska vara tunna adapters över samma handlers
- handlers returnerar faktiska use-case-resultat
- använd explicita use-case-specifika resultattyper när flera legitima utfall behöver representeras
- pressa inte success, validation, not found och andra utfall in i ett globalt `CommandResult`- eller `QueryResult`-format

## Gemensamma Ytor

- använd `UiKit` för neutrala UI-primitiver utan modul-, domän-, route- eller shellkunskap
- använd `Kernel` endast för små, stabila och teknikneutrala systemprimitiver
- använd top-level `Infrastructure` för neutral teknisk implementation, exempelvis Mongo-klient, provider, serializers och conventions
- top-level `Infrastructure` får inte känna till modulernas entities, stores, collection names eller integrationsmodeller
- komponenter med endast en verklig konsument ska normalt ligga hos konsumenten
- abstrahera inte hypotetisk återanvändning

## Authentication Och Authorization

- AppShell äger aktuell användarsession, generell identity mapping och loginpresentation
- Access äger portalclaims, authorization policies, application-admin-semantik och accessbeslut
- feature- och UI-kod ska konsumera sessions- och authorization-kontrakt i stället för att tolka `ClaimsPrincipal` direkt
- claims parsing ska begränsas till auth-/authorization-gränsen
- råa Entra- och Microsoft Graph-modeller ska stanna i integrationsadaptern
- runtime mocks ska ligga bredvid den adapter eller modul de ersätter
- test fixtures ska ligga i testprojekt och inte dela globalt muterbart runtime-state

## Blazor Och UI

- håll komponenter fokuserade på rendering, interaktion och enkel orchestration
- flytta testbar logik till use cases, tjänster, helpers eller state-objekt
- undvik integrationslogik direkt i UI
- undvik att ny produktlogik kräver Blazor-specifika typer
- registrera navigation och settings areas som data i `AppRegistrationContext`
- registrera settings area med explicit module key
- följ befintliga modulregistreringar, till exempel `Modules/Access/AccessModule.cs`

## Services, Stores Och Persistence

- lägg integrationskod bakom smala tjänste- eller klientkontrakt
- låt handlers bero på modul- eller slice-ägda store-kontrakt
- använd inte `DbContext`, `IMongoCollection<T>` eller provider-specifika API:er direkt i handlers
- varje modul äger sina stores, persistence entities, mappings, collection names och DI-registreringar
- AppShell äger persistence för portalövergripande inställningar och preferenser
- skapa inte generiska CRUD-repositories eller generiska datastores som standard
- håll store-kontrakt små och use case-nära
- använd typed clients eller integrationslokala klientnamn i stället för globala strängkonstanter

## Tester

Prioritera tester för:

- beräkningar
- formattering
- accessregler
- mappningar
- use cases
- snapshot-/rapportlogik
- dependency direction och publika modulgränser

Arkitekturtester ska verifiera meningsfulla regler, särskilt:

- ingen production-`Shared`-kod eller `BlazorServer.Shared`-namespace
- `Kernel` är oberoende av moduler, AppShell, UiKit och Infrastructure
- moduler når inte in i varandras interna implementationer
- cross-module-interaktion sker genom publika modulkontrakt

Undvik tester som låser implementationen i exakta filplaceringar när en tydligare beroenderegel kan testas.

## Definition Of Done

En ändring är normalt klar när:

- lösningen kompilerar eller berörda delar verifierats
- relevant beteende är testat
- arkitekturtester är gröna när modul- eller ägargränser berörs
- ändringen följer modulgränser och berörd use case-struktur
- ny struktur, nytt ansvar eller nytt arbetssätt är dokumenterat
- inga nya generella abstraheringar har införts utan verkligt behov

## Säkerhet

Hemligheter ska inte ligga permanent i versionshanterad konfiguration. Använd User Secrets, miljövariabler eller annan dokumenterad hemlighetshantering.