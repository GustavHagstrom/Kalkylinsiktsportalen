# Arkitekturöversikt

## Syfte

Det här dokumentet beskriver den nuvarande arkitekturen så som den kan utläsas från koden idag.
Det är ett arbetsdokument, inte en slutgiltig målarkitektur.

## Systembild

Lösningen består idag av tre tydliga delar:

1. **BlazorServer** — huvudapplikationen
2. **BlazorServer.Tests** — testprojekt kopplat till huvudapplikationen

## BlazorServer i stora drag

Applikationen använder:
- **Blazor Server** för UI och interaktivitet
- **MudBlazor** för komponentbibliotek
- **MongoDB** som lagringsmekanism för flera typer av applikationsdata
- **Microsoft Identity / OpenID Connect** i production
- **mockad auth** i development
- ett externt **Bidcon API** via `HttpClient`

## Arkitekturstil

Den dominerande strukturen är idag en kombination av:
- **feature-baserad organisering**
- **shared/cross-cutting lager**
- **DI-baserad registrering**

Några tydliga byggblock:
- `Features/*` innehåller funktionsområden
- `Shared/*` innehåller återanvändbara komponenter, DTO:er, tjänster, stores och registreringsmekanismer
- `ServiceAndFeatureRegistrations.cs` kopplar ihop features och infrastruktur
- `Program.cs` startar applikationen och sätter upp pipeline

## Viktiga runtime-flöden

### 1. App-start
`Program.cs`:
- skapar `WebApplicationBuilder`
- registrerar Razor Components och controllers
- anropar `AddKipServicesAndFeatures()`
- bygger appen
- sätter upp middleware, static assets, controllers och Razor components

### 2. Feature-registrering
`ServiceAndFeatureRegistrations.cs`:
- väljer auth-upplägg beroende på environment
- registrerar features via `ConfigureFeatures(...)`
- konfigurerar HTTP-klienter, MongoDB, renderingstjänster och auth-relaterade tjänster

### 3. Feature discovery / extensibility
`FeatureRegistrator` skapar feature-instanser och låter varje feature:
- registrera services
- registrera render-komponenter
- registrera claims
- lägga till authorization policies

Detta tyder på att systemet försöker vara modulärt utan att fullt ut vara plugin-baserat.

## Infrastruktur och externa beroenden

### MongoDB
Mongo används just nu bland annat för:
- användarinställningar
- användaråtkomst
- rollåtkomst
- Bidcon API credentials
- views

### Bidcon API
Det finns en dedikerad tjänst (`BidconApiAccess`) som:
- hämtar credentials från store
- autentiserar mot externt API
- cache:ar access token
- läser estimationsdata

### Identity / Authentication
Två driftlägen observeras:

#### Development
- mockad auth handler
- mockade tjänster för user och role

#### Production
- Microsoft Identity Web
- OpenID Connect
- Graph-klient registrerad via auth provider

## Rekommenderade beroenderiktningar

Nuvarande struktur antyder följande avsedda riktning:
- `Features/*` får bero på `Shared/*`
- `Shared/*` bör undvika beroenden på enskilda features
- UI-komponenter bör inte bära mer affärslogik än nödvändigt
- externa integrationer bör ligga bakom tjänster och stores

## Identifierade styrkor

- Tydlig feature-struktur i huvudapplikationen
- En central registreringspunkt för features
- Enkel att förstå på hög nivå
- Rimlig separation mellan UI, auth, stores och externa integrationer

## Identifierade frågor att arbeta vidare med

- Var går gränsen mellan `Features/*` och `Shared/*`?
- Vilken del av systemet innehåller egentlig domänlogik?
- Hur mycket av auth- och accessmodellen är långsiktig design kontra temporär implementation?
- Hur bör views, access och Bidcon-domänen beskrivas mer affärsnära?

## Målbild för fortsatt dokumentation

Det här dokumentet bör över tid kompletteras med:
- domänmodell / begreppslista
- container-/komponentdiagram
- viktigaste användarflöden
- auth- och accessmodell
- datamodell för centrala collections
