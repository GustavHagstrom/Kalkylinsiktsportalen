# Nuläge

Det här dokumentet beskriver hur lösningen ser ut idag och hur dagens struktur mappas mot målarkitekturen. Målbilden finns i [target-architecture.md](target-architecture.md) och vägen dit i [migration-plan.md](migration-plan.md).

Det här dokumentet beskriver observerad kod. Det äger inte riktningen för var ny kod bör hamna eller vilka arbetsregler som ska följas; sådant ägs av [target-architecture.md](target-architecture.md) och [../development.md](../development.md).

## Systembild

Lösningen består främst av:

- `src/BlazorServer` - huvudapplikationen
- `src/Tests/BlazorServer.Tests` - testprojekt
- `docs` - långlivad dokumentation

Applikationen använder:

- Blazor Server för UI och interaktivitet
- MudBlazor som komponentbibliotek
- MongoDB för användarinställningar, access, credentials och views
- Microsoft Identity / OpenID Connect i production
- mockad auth i development
- Bidcon API via `HttpClient`

## Nuvarande Arkitekturstil

Den dominerande strukturen är:

- `Features/*` för funktionsområden
- `Shared/*` för komponenter, DTO:er, modeller, services, stores och registrering
- `ServiceAndFeatureRegistrations.cs` som central composition root
- `Program.cs` som relativt tunn app-start
- `IFeature` som mönster för feature-registrering

Varje feature kan registrera:

- services
- renderbara komponenter
- claims
- authorization policies

Det ger en modulär riktning, men koden är ännu inte organiserad enligt målmodulerna.

## Runtime-Flöden

**App-start**
1. `Program.cs` skapar buildern.
2. Razor Components och controllers registreras.
3. `AddKipServicesAndFeatures()` registrerar applikationens services och features.
4. Appen bygger middleware, static assets, controllers och Razor components.

**Feature-registrering**
1. `ServiceAndFeatureRegistrations.cs` väljer auth-upplägg per environment.
2. `ConfigureFeatures(...)` registrerar features.
3. HTTP-klienter, MongoDB, renderingstjänster och auth-relaterade tjänster registreras.

**Feature discovery**
`FeatureRegistrator` skapar feature-instanser och låter dem bidra med services, renderkomponenter, claims och policies.

## Infrastruktur

**MongoDB** används för:

- användarinställningar
- användaråtkomst
- rollåtkomst
- Bidcon API credentials
- views

**Bidcon API** nås idag via `BidconApiAccess`, som hämtar credentials, autentiserar, cache:ar access token och läser estimationsdata.

**Auth** har två lägen:

- development: mockad auth handler och mockade user/role-services
- production: Microsoft Identity Web, OpenID Connect och Graph-klient

## Features I Nuläget

| Feature | Ansvar idag | Målmodul/riktning |
|---|---|---|
| `Settings` | Inställningsnavigation och shell | Shared UI / Web shell |
| `Access` | Användar- och rollrättigheter | `Access` |
| `BidconAccess` | API-åtkomst och credentials | `Bidcon` + `Organizations` |
| `BidconBrowser` | Utforskning/listning av Bidcon-estimat | `Bidcon`, senare delvis `Projects` |
| `ViewSettings` | Views och view-redigering | `Reports` |

Tabellen ovan är en mapping mellan dagens featurestruktur och den tänkta målstrukturen. Den beskriver nuläget, inte bindande placeringsregler för framtida ändringar.

## Observerade Spänningar

- `Shared/*` innehåller både verkligt tvärgående kod och produktnära ansvar, särskilt view-/rapportlogik.
- UI-komponenter orkestrerar fortfarande en del logik som på sikt passar bättre bakom use cases eller tydligare kontrakt.
- Stores och externa integrationer är ännu inte konsekvent kapslade bakom modulgränser.
- `IFeature`-mönstrets roll i förhållande till `Modules/*` är fortfarande under utveckling.
- Testytan är fortfarande smal.

## Styrkor I Nuläget

- tydlig feature-struktur
- central registrering
- enkel hög nivå att förstå
- rimlig separation mellan UI, auth, stores och externa integrationer
- ett etablerat mönster för att lägga till features
