# Nuläge

Det här dokumentet beskriver hur lösningen ser ut idag och hur dagens struktur mappas mot målarkitekturen. Målbilden finns i [target-architecture.md](target-architecture.md) och vägen dit i [migration-plan.md](migration-plan.md).

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

## Placering Av Kod

Använd `Features/*` när ändringen är liten och hör nära ihop med befintlig UI, state eller feature-registrering.

Använd `Modules/*` för ny produktlogik som passar målmodulerna och bör kunna återanvändas av API, tester eller framtida frontend.

Använd `Shared/*` bara för verkligt tvärgående ansvar. På sikt bör `Shared` delas upp mot:
- `Shared/UI`
- `Shared/Kernel`
- `Shared/Infrastructure`

## Styrkor

- Tydlig feature-struktur
- Central registrering
- Enkel hög nivå att förstå
- Rimlig separation mellan UI, auth, stores och externa integrationer
- Ett etablerat mönster för att lägga till features

## Saker Att Hålla Öga På

- `Shared` innehåller produktnära kod, särskilt view-/rapportlogik.
- UI-komponenter bör göras tunnare när kärnflödena växer.
- Stores och externa integrationer bör ligga bakom tydliga kontrakt.
- `IFeature`-mönstrets roll behöver beslutas när `Modules/*` blir tydligare.
- Testytan är fortfarande smal.

