# Features och moduler

Det här dokumentet beskriver hur lösningen i nuläget är uppdelad och vilka ansvar som verkar finnas i respektive område.

## Översikt

### `BlazorServer/Features`
Feature-katalogen ser ut att användas för funktionsspecifika delar av applikationen.

Observerade features:
- `Settings`
- `Access`
- `BidconAccess`
- `BidconBrowser`
- `ViewSettings`

### `BlazorServer/Shared`
Shared-katalogen innehåller tvärgående byggblock:
- komponenter
- tjänster
- DTO:er
- modeller
- stores
- registrering och rendering
- auth-relaterade hjälpklasser

### `Tests/BlazorServer.Tests`
Testprojekt för BlazorServer.

## Feature-tabell

| Feature | Troligt ansvar | Kommentar |
|---|---|---|
| `Settings` | Samlingspunkt för inställningsnavigation | Verkar främst exponera navigation/rendering |
| `Access` | Hantering av användar- och rollrättigheter | Har authorization policy och claims |
| `BidconAccess` | Hantering av API-åtkomst/credentials för Bidcon | Kopplar till settings och access claims |
| `BidconBrowser` | Utforskning/listning av Bidcon-estimat | Innehåller state och UI för listning |
| `ViewSettings` | Hantering av views och view-redigering | Troligen lagrat i MongoDB |

## Extension pattern via `IFeature`

Varje feature implementerar i nuläget ett gemensamt kontrakt:
- registrera services
- registrera render-objekt
- registrera claims
- registrera authorization policies

Det ger en bra grund för ett standardiserat sätt att lägga till ny funktionalitet.

## Rekommenderad tumregel för placering av kod

### Lägg i `Features/*` när:
- koden tillhör ett specifikt affärsområde eller en specifik skärmyta
- beroendet främst används av en feature
- UI, state och regler hör ihop inom samma funktion

### Lägg i `Shared/*` när:
- koden används av flera features
- det är en verkligt tvärgående komponent eller tjänst
- det handlar om generella kontrakt, gemensamma modeller eller infrastruktur

## Föreslagen ansvarsfördelning framåt

### Features
Varje feature bör om möjligt äga:
- routes
- UI-komponenter
- feature-specifik state
- feature-specifika tjänster
- feature-specifika authorizationregler
- länkar in i settings eller navigation

### Shared
Shared bör om möjligt begränsas till:
- gemensamma UI-primitiver
- auth/infrastruktur
- generella stores och kontrakt
- rendering/registreringsmekanismer
- DTO:er som delas mellan flera features eller externa integrationer

## Förslag på framtida strukturprinciper

1. **Feature först**
   - ny funktionalitet ska i första hand placeras i en feature
2. **Shared med återhållsamhet**
   - flytta inte upp kod till shared förrän återanvändningen är verklig
3. **Tydliga kontrakt**
   - stores och externa integrationer bör ligga bakom interface där det ger värde
4. **Registrering nära ansvaret**
   - feature-specifik registrering ska göras via feature-kontraktet
5. **Blazor-komponenter tunna där det går**
   - flytta återanvändbar logik till tjänster, state-objekt eller hjälpare

## Kända förbättringsområden att utreda

- Om `Shared` börjar växa snabbt kan den behöva delas upp mer explicit i exempelvis:
  - `Shared/UI`
  - `Shared/Auth`
  - `Shared/Infrastructure`
  - `Shared/Domain`
- Det kan vara värt att dokumentera vilka features som äger vilka routes och policies.
- Testprojektet täcker i nuläget bara en liten del av lösningen och kan behöva breddas över tid.
