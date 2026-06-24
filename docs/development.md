# Utveckling

Det här dokumentet samlar lokal utveckling, kodriktlinjer och praktiska arbetsregler.

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
- feature-baserad indelning i `Features/*`
- tvärgående kod i `Shared/*`
- registrering via `ServiceAndFeatureRegistrations`
- UI med Blazor Server och MudBlazor

Följ målarkitekturen för ny produktkod:
- `Modules/Projects` för projekt, estimatkopplingar och snapshots
- `Modules/Reports` för views, rapportmallar, rendering, formler och PDF/WYSIWYG
- `Modules/Bidcon` för extern integration, credentials och import
- `Modules/Access` för funktions- och view-rättigheter
- `Modules/Organizations` för tenant-kontext och organisationsinställningar

`Shared` är inte default. Använd bara shared när flera områden faktiskt delar samma ansvar.

## Blazor Och UI

- håll komponenter fokuserade på rendering, interaktion och enkel orchestration
- flytta testbar logik till use cases, tjänster, helpers eller state-objekt
- undvik integrationslogik direkt i UI
- återanvänd befintliga MudBlazor-mönster
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

