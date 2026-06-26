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
- strukturera ny modulär logik som feature/use case-slices
- skapa interna mappar som `Domain`, `Infrastructure` och `Components` bara när de löser ett konkret behov
- använd avsiktliga kontrakt mellan moduler i stället för att nå in i intern implementation

Använd `Shared` restriktivt:

- lägg bara sådant i `Shared` som faktiskt är stabilt och tvärgående
- använd `Shared/UI` för generiska UI-primitiver
- använd `Shared/Kernel` för små stabila gemensamma typer
- använd `Shared/Infrastructure` för teknisk implementation som delas av flera moduler
- flytta äldre `Shared/*` stegvis när koden ändå berörs eller blockerar en tydlig modulgräns

## Blazor Och UI

- håll komponenter fokuserade på rendering, interaktion och enkel orchestration
- flytta testbar logik till use cases, tjänster, helpers eller state-objekt
- undvik integrationslogik direkt i UI
- undvik att ny produktlogik kräver Blazor-specifika typer

## Services, Stores Och Kontrakt

- lägg integrationskod bakom tjänster
- lägg persistence bakom stores eller tydliga kontrakt
- håll beroenden explicita via DI
- låt moduler bero på avsiktliga kontrakt eller use cases, inte på varandras interna implementationer

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
