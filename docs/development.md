# Utveckling

Det här dokumentet samlar lokal utveckling, kodriktlinjer och praktiska arbetsregler.

Arkitekturriktning, modulgränser och beroendeprinciper ägs av [architecture/target-architecture.md](architecture/target-architecture.md). Det här dokumentet översätter den riktningen till praktiskt arbete i repoet.

## Lokal Setup

Förutsättningar:

- .NET SDK 10
- lokal eller åtkomlig MongoDB
- möjlighet att köra Blazor Server lokalt
- eventuell Azure/Microsoft Identity-konfiguration om production-lik auth ska testas

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

## Vanliga Kommandon

Kör från `src` om inget annat anges.

```powershell
dotnet run --project BlazorServer
dotnet test Tests/BlazorServer.Tests
```

## Arbetsflöde

1. Starta `BlazorServer`.
2. Kontrollera att development-auth fungerar.
3. Säkerställ att MongoDB är tillgänglig.
4. Arbeta i berörd feature eller målmodul.
5. Kör relevanta tester.
6. Uppdatera docs om ansvar, regler eller struktur ändrats.

## Kodprinciper

Gör minsta rimliga ändring:

- ändra så få filer som möjligt
- håll dig inom rätt feature eller modul
- undvik bred refaktorering om den inte behövs

Följ nuläget när ändringen är liten:

- använd befintlig feature-baserad indelning i `Features/*` när ändringen hör nära ihop med dagens kod
- håll dig nära befintlig registrering via `ServiceAndFeatureRegistrations`
- återanvänd etablerade UI-mönster i Blazor Server och MudBlazor

Följ målarkitekturen när du lägger ny produktlogik:

- lägg ny produktlogik i relevant `Modules/*`
- registrera nya målmoduler via `IModule` och `ModuleRegistration`
- strukturera ny modulär logik som feature/use case-slices
- skapa interna mappar som `Domain`, `Infrastructure` och `Components` bara när de löser ett konkret behov
- använd avsiktliga kontrakt mellan moduler i stället för att nå in i intern implementation

Praktiska regler för nya feature slices:

- `Features/*` innehåller application use cases: commands, queries, handlers, validators, result/read models och slice-ägda store-kontrakt
- `Components/*` innehåller Blazor UI och ska ligga separat från `Features/*`
- components får bero på features, men features får inte bero på components
- Blazor Server-components får anropa handlers direkt
- handlers ska vara fria från Blazor-, HTTP-, navigation-, dialog-, snackbar-, JS- och andra host/UI-specifika beroenden
- handlers returnerar application-resultat som kan visas av Blazor idag och mappas till HTTP senare
- skapa inte endpoints, HTTP request/response-DTO:er eller OpenAPI-kontrakt som standard innan en faktisk API-klient eller migrationsövning behöver dem
- framtida endpoints ska vara tunna adapters över samma handlers, inte en alternativ plats för produktlogik

Använd `Shared` restriktivt:

- lägg bara sådant i `Shared` som faktiskt är stabilt och tvärgående
- använd `Shared/UI` för generiska UI-primitiver
- använd `Shared/Kernel` för små stabila gemensamma typer
- använd `Shared/Infrastructure` för teknisk implementation som delas av flera moduler
- flytta äldre `Shared/*` stegvis när koden ändå berörs eller blockerar en tydlig modulgräns
- externa API-DTO:er ska på sikt ligga i modulen som äger integrationen; äldre kopior i `Shared/DTOs` får ligga kvar som kompatibilitet tills berörda call sites flyttas

## Blazor Och UI

- håll komponenter fokuserade på rendering, interaktion och enkel orchestration
- flytta testbar logik till use cases, tjänster, helpers eller state-objekt
- undvik integrationslogik direkt i UI
- undvik att ny produktlogik kräver Blazor-specifika typer
- registrera top navigation och settings navigation som data via `module.Ui`, inte som godtyckliga komponenttyper
- registrera settings sections som komponenter när modulen äger innehåll, formulär eller state

Exempel på modulregistrering:

```csharp
public sealed class ExampleModule : IModule
{
    public void Register(ModuleRegistration module)
    {
        module.Services.AddTransient<ExampleHandler>();

        module.Ui.AddTopNavigation(
            text: "Exempel",
            href: "/example",
            order: 50);

        module.Ui.AddSettingsNavigation(
            text: "Exempel",
            href: "/settings/example",
            order: 50,
            icon: Icons.Material.Sharp.Settings,
            policy: "ExampleSettingsPolicy");

        module.Ui.AddSettingsSection<ExampleSettingsSection>(order: 50);
    }
}
```

## Services, Stores Och Kontrakt

- lägg integrationskod bakom tjänster
- lägg persistence bakom stores eller tydliga kontrakt
- låt handlers bero på module- eller slice-ägda store-kontrakt, inte direkt på `DbContext`, `IMongoCollection<T>` eller andra persistence-API:er
- använd `DbContext`, `IMongoCollection<T>` och provider-specifika query builders i infrastructure, store-implementationer, migrations/setup eller tekniska adapters
- håll store-kontrakt små och use case-nära; namnge nya persistenskontrakt som stores och skapa inte repositories eller generiska CRUD-abstraktioner som standard
- håll beroenden explicita via DI
- låt moduler bero på avsiktliga kontrakt eller use cases, inte på varandras interna implementationer
- använd `Contracts` för avsiktliga modulgränser, till exempel små interface, read models eller delade identitetstyper; lägg inte konkreta domänmodeller, persistence-modeller eller implementation där

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
- ändringen följer feature- och modulgränser
- ny struktur, nytt ansvar eller nytt arbetssätt är dokumenterat

## Säkerhet

Hemligheter ska inte ligga permanent i versionshanterad konfiguration. Använd User Secrets, miljövariabler eller annan dokumenterad hemlighetshantering.
