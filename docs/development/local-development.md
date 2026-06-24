# Lokal utveckling

## Förutsättningar

Nuvarande repo antyder följande lokala beroenden:
- .NET SDK **10**
- lokal eller åtkomlig **MongoDB**
- möjlighet att köra Blazor Server lokalt
- eventuellt Azure/Microsoft Identity-konfiguration om production-lik auth ska testas

## Startprojekt

Normalt startprojekt bör vara:
- `BlazorServer`

Övriga projekt:
- `Tests/BlazorServer.Tests` för tester

## Konfiguration

Observerade konfigurationsområden i `BlazorServer/appsettings.json`:
- `AzureAd`
- `DownstreamApi`
- `MongoDb`
- `Bidcon`
- `ConnectionStrings`

## Rekommenderad lokal setup

### Development-läge
I development använder applikationen mockad auth. Det gör lokal utveckling enklare eftersom delar av systemet kan testas utan full produktionsinloggning.

### MongoDB
Säkerställ att MongoDB finns tillgänglig enligt konfigurationen, exempelvis:
- `MongoDb:ConnectionString`
- `MongoDb:DatabaseName`

### Bidcon API
För funktioner som använder Bidcon behöver följande fungera:
- korrekt `BaseUrl`
- credentials lagrade i applikationens store
- organisation-/användarkontext vid anrop

## Vanliga kommandon

Kör från lösningens rot om du arbetar via terminal.

### Starta Blazor-appen
`dotnet run --project BlazorServer`

### Köra tester
`dotnet test Tests/BlazorServer.Tests`

### Köra konsolverktyget
`dotnet run --project PwCracker`

## Rekommenderat utvecklingsflöde

1. Starta `BlazorServer`
2. Verifiera att lokal auth/dev-läge fungerar
3. Säkerställ att MongoDB är uppe
4. Testa den feature du arbetar i
5. Kör relevanta tester
6. Uppdatera dokumentation om ansvar, regler eller struktur ändrats

## Checklista för nya features

När du lägger till en ny feature:
- skapa feature-klass som implementerar `IFeature`
- registrera featuren i `AddKipServicesAndFeatures()`
- lägg routes, komponenter och state nära featuren
- håll shared-ändringar till ett minimum
- lägg till eller uppdatera tester där logik tillkommer
- uppdatera docs om arkitektur eller arbetssätt påverkas

## Säkerhetsnotering att följa upp

Det bör bekräftas hur secrets ska hanteras lokalt och i repo. Som generell riktlinje bör hemligheter inte ligga permanent i versionshanterad konfiguration, även om något just nu används för development.
