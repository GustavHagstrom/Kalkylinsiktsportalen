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

I development används mockad auth, vilket gör lokal utveckling möjlig utan full production-inloggning.
Azure/Microsoft Identity-konfiguration behövs bara när production-lik auth uttryckligen ska testas.
MongoDB behövs för appkörning och persistence-flöden som använder Mongo-konfiguration.

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
4. Arbeta i berörd modul eller befintligt kodområde.
5. Kör relevanta tester.
6. Uppdatera docs om ansvar, regler eller struktur ändrats.

## Kodprinciper

Gör minsta rimliga ändring:

- ändra så få filer som möjligt
- håll dig inom rätt modul eller kodområde
- undvik bred refaktorering om den inte behövs

Följ befintlig kod när ändringen är liten:

- håll dig nära befintlig struktur när ändringen hör nära ihop med dagens kod
- återanvänd etablerade UI-mönster i Blazor Server och MudBlazor

Följ målarkitekturen när du lägger ny produktlogik:

- lägg ny produktlogik i relevant `Modules/*`
- registrera nya målmoduler via statiska `IServiceCollection`-extensions som får `AppRegistrationContext`
- strukturera ny modulär logik som feature/use case-slices
- skapa interna mappar som `Domain`, `Infrastructure` och `Components` bara när de löser ett konkret behov
- exponera cross-module-ytor under `Modules/{Module}/Public`
- använd avsiktliga publika kontrakt mellan moduler i stället för att nå in i intern implementation

Hård regel för modulgränser:

- andra moduler får bara referera till kod under `Modules/{Module}/Public`
- använd `Public/Contracts` för avsiktliga modulkontrakt och `Public/Components` för avsiktliga entry components
- moduler får inte anropa varandras interna use cases, stores, API-klienter eller helper components direkt
- om en annan modul behöver beteende ska den ägande modulen exponera ett smalare kontrakt under `Public`

Praktiska regler för nya feature slices:

- `Features/*` innehåller application use cases
- `Components/*` innehåller Blazor UI
- components får bero på features, men features får inte bero på components
- Blazor Server-components får anropa handlers direkt
- handlers ska vara fria från Blazor-, HTTP-, navigation-, dialog-, snackbar-, JS- och andra host/UI-specifika beroenden
- handlers returnerar application-resultat som kan visas av Blazor idag och mappas till HTTP senare
- framtida endpoints ska vara tunna adapters över samma handlers, inte en alternativ plats för produktlogik

Använd gemensamma ytor restriktivt:

- lägg inte ny kod i `Shared`; befintlig `Shared`-kod är en migreringsyta som ska få tydligare ägare
- använd `UiKit` för generiska, neutrala UI-primitiver utan modul-, domän-, route- eller shellkunskap
- använd `Kernel` för små stabila gemensamma typer när de har verkligt systemövergripande ansvar
- använd `Infrastructure` för neutral teknisk implementation som delas av flera moduler
- `Shared/DTOs` är befintlig intern/legacy-modell och inte en defaultplats för ny kod
- externa integrations-DTO:er ska ligga i modulen som äger integrationen

## Blazor Och UI

- håll komponenter fokuserade på rendering, interaktion och enkel orchestration
- flytta testbar logik till use cases, tjänster, helpers eller state-objekt
- undvik integrationslogik direkt i UI
- undvik att ny produktlogik kräver Blazor-specifika typer
- registrera top navigation och settings areas som data i `AppRegistrationContext`, inte som godtyckliga komponenttyper
- registrera settings area med explicit module key så Settings Hub kan hålla en synlig settings-ingång per modul
- följ befintliga modulregistreringar, till exempel `Modules/Access/AccessModule.cs`

## Services, Stores Och Kontrakt

- lägg integrationskod bakom tjänster
- lägg persistence bakom stores eller tydliga kontrakt
- låt handlers bero på module- eller slice-ägda store-kontrakt, inte direkt på `DbContext`, `IMongoCollection<T>` eller andra persistence-API:er
- använd `DbContext`, `IMongoCollection<T>` och provider-specifika query builders i infrastructure, store-implementationer, migrations/setup eller tekniska adapters
- håll store-kontrakt små och use case-nära; namnge nya persistenskontrakt som stores och skapa inte repositories eller generiska CRUD-abstraktioner som standard
- håll beroenden explicita via DI

## Tester

Prioritera tester för:

- beräkningar
- formattering
- accessregler
- mappningar
- use cases
- snapshot-/rapportlogik

Undvik att låsa testsviten i UI-detaljer när logiken kan testas längre ned.

## Definition Of Done

En ändring är normalt klar när:

- lösningen kompilerar eller berörda delar verifierats
- relevant beteende är testat
- ändringen följer modulgränser och berörd use case-struktur
- ny struktur, nytt ansvar eller nytt arbetssätt är dokumenterat

## Säkerhet

Hemligheter ska inte ligga permanent i versionshanterad konfiguration. Använd User Secrets, miljövariabler eller annan dokumenterad hemlighetshantering.
