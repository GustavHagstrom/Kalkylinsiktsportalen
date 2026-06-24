# Repoanalys: Kalkylinsiktsportalen

Datum: 2026-06-24

Det här dokumentet är ett arbetsunderlag för framtida dokumentation om repots mål,
arkitektur, domän, ADR:er, vision och development guidelines.

## Kort sammanfattning

Kalkylinsiktsportalen är en Blazor Server-applikation som verkar byggas som en
intern portal för att hämta, välja, administrera och visualisera Bidcon-kalkyler.
Systemet kombinerar Microsoft Identity-baserad inloggning, claims-baserad intern
åtkomststyrning, MongoDB-persistens och en feature-baserad modulstruktur.

Repot är fortfarande tidigt i sin dokumentationsmognad. README beskriver appen
generiskt som "App platform for personal projects", medan kod och filstruktur
tydligt pekar mot en mer specifik produkt: Kalkylinsiktsportalen.

## Lösningsstruktur

- `src/Kalkylinsiktsportalen.sln` är Visual Studio-lösningen.
- `src/BlazorServer` är huvudapplikationen.
- `src/PwCracker` är ett separat konsolprojekt i samma lösning.
- `docs` innehåller dokumentationsmaterial.

Huvudappen använder:

- .NET / Blazor Server
- MudBlazor för UI
- Microsoft Identity Web och Microsoft Graph för autentisering och Graph-integration
- MongoDB.Driver för persistens
- Razor components med interactive server render mode

## Produkt- och domänbild

Den domän som framträder i koden består av:

- Organisationer
- Användare
- Roller
- Åtkomstclaims
- Bidcon API-credentials
- Bidcon-kalkyler / estimations
- Vyer och rapportmallar
- Data-, sheet- och page break-sektioner i vyer

Det troliga produktmålet är att ge användare insikt i Bidcon-kalkyler genom en
portal där man kan:

- Logga in via Microsoft Identity.
- Hämta kalkyler från Bidcon per användare och organisation.
- Välja kalkyler att arbeta vidare med.
- Administrera användar- och rollbaserade rättigheter.
- Administrera Bidcon API-åtkomst per organisation.
- Skapa eller ändra vy-/rapportmallar för hur kalkyldata ska presenteras.

## Arkitektoniska mönster

### Feature-baserad modulstruktur

Applikationen organiseras runt features. Varje feature implementerar ett gemensamt
interface med fyra ansvar:

- Registrera services.
- Registrera renderbara UI-objekt.
- Registrera claims.
- Registrera authorization policies.

Detta gör features till självbeskrivande moduler som kan bidra med både backend-
och UI-beteende.

Existerande features:

- `SettingsFeature`
- `AccessFeature`
- `BidconAccessFeature`
- `BidconBrowserFeature`
- `ViewSettingsFeature`

### Central composition root

`ServiceAndFeatureRegistrations` fungerar som applikationens composition root.
Där kopplas autentisering, features, HTTP-klienter, MongoDB, MudBlazor,
localization och gemensamma services ihop.

Det finns en tydlig skillnad mellan development och övriga miljöer:

- Development använder mockad autentisering och mockade user/role-services.
- Övriga miljöer använder Microsoft Identity Web, OpenID Connect och Graph.

### UI-komposition via RenderCollection

Features hårdkodar inte global navigation direkt i layouten. De registrerar i
stället komponenttyper i `RenderCollection`, som sedan renderas av layout och
settings-skal.

Detta mönster används för:

- Appbar-navigation
- Settings-navigation
- Login display-komponent
- Potentiellt delade settings-komponenter

### Claims-baserad åtkomststyrning

Åtkomstmodellen bygger på applikationsclaims. Features registrerar vilka claims
de kräver, och policies kontrollerar om användaren har rätt claim eller admin-roll.

Claims laddas per request via middleware och transformeras med hjälp av en store
och en cache. Admin markeras som en särskild role-claim.

### Store- och mapper-mönster för persistens

Persistens abstraheras via interface i `Shared/Stores`.

MongoDB-specifik kod ligger under `Shared/Stores/MongoDb` och delas upp i:

- `Stores`
- `Enteties`
- `Mappers`
- Mongo-registrering

Exempel: `IViewStore` implementeras av `MongoViewStore`, som använder
`MongoViewMapper` för att översätta mellan Mongo-entitet och `ViewDto`.

### Bidcon som extern integration

Bidcon API hanteras via en namngiven HTTP-klient och `BidconApiAccess`.
Access token cachas per organisation. API-credentials hämtas via store.

Detta är en tydlig integrationsgräns som bör dokumenteras separat.

## Kod- och namngivningsmönster

Positiva mönster:

- Features är lätta att hitta och avgränsa.
- Behörighetskrav ligger nära respektive feature.
- Stores gör persistens åtkomlig bakom interface.
- Mongo-mappning är explicit.
- UI-skal och navigation är modulärt uppbyggda.
- Development-auth är separerad från produktionsauth.

Mönster som bör stabiliseras:

- Svenska används i UI-texter och vissa descriptions.
- Engelska används i kodnamn.
- Domäntermer från Bidcon används direkt i DTO:er och komponenter.

Rekommenderad guideline:

- Använd svenska för produkttext och användarvända labels.
- Använd engelska för kodsymboler.
- Dokumentera centrala domäntermer i en ordlista så att svenska och engelska namn
  inte glider isär.

## Identifierad teknisk skuld

### Testprojekt saknas

Det finns inga uppenbara testprojekt i lösningen. Det gör framtida refaktorering
av auth, features, stores och Bidcon-integration mer riskfylld.

### PwCracker ligger i produktlösningen

`PwCracker` är ett separat konsolprojekt som skriver dekrypterat lösenord till
console. Det bör dokumenteras som ett lokalt utvecklingsverktyg, flyttas till en
tydligare plats, eller tas bort från produktlösningen.

### Hemligheter i appsettings

`appsettings.json` innehåller en `ClientSecret` med kommentar om development-only.
Även om den är avsedd för utveckling bör riktlinjen vara att hemligheter inte
checkas in.

### Async-blockering

`BidconApiAccess` innehåller `.Result` inne i ett async-flöde vid tokenhämtning.
Det bör ersättas med full `await`-kedja.

### Stavfel i kodnamn och mappar

Flera namn innehåller stavfel, till exempel:

- `IFreature`
- `Enteties`
- `MongoRoleAccssMapper`
- `Pageination`
- `AppNavMenue`
- `Compoenents`
- `ClaimsTransofmation`

Det påverkar läsbarhet, onboarding och framtida dokumentation. Bör hanteras med
kontrollerade refaktoreringar, inte som spontan sidostädning.

### Nullability-kontrakt

Vissa mapper-metoder och stores signalerar nullable i anrop men har signaturer
som inte alltid uttrycker det tydligt. Exempel: mapper från Mongo-entitet till DTO
bör antingen hantera null explicit eller kräva non-null.

## ADR-kandidater

Följande beslut är värda egna ADR:er:

1. Varför Blazor Server används som applikationsmodell.
2. Varför MudBlazor är valt som UI-komponentbibliotek.
3. Varför applikationen använder feature-registrering via `IFeature`.
4. Varför MongoDB används som primär persistens.
5. Varför intern behörighet modelleras med claims ovanpå Microsoft Identity.
6. Hur development-auth ska fungera och vilka garantier mock-läget ska ge.
7. Hur Bidcon API-credentials ska lagras, roteras och användas.
8. Om `PwCracker` ska ingå i produktlösningen eller hanteras separat.
9. Om `ViewDto` ska fungera som intern modell eller om en tydligare domänmodell
   ska introduceras.
10. Hur lokalisering och språkgränser ska hanteras i kod, UI och dokumentation.

## Förslag till dokumentstruktur

En framtida dokumentationsyta kan börja med:

- `docs/product-vision.md`
- `docs/domain.md`
- `docs/architecture.md`
- `docs/development-guidelines.md`
- `docs/adr/0001-use-blazor-server.md`
- `docs/adr/0002-feature-registration.md`
- `docs/adr/0003-claims-based-access.md`
- `docs/adr/0004-mongodb-persistence.md`
- `docs/adr/0005-bidcon-integration.md`

## Förslag till architecture.md

Arkitekturdokumentet bör beskriva följande lager:

- Startup och composition root
- Feature-moduler
- Shared UI-komponenter
- Services och externa integrationer
- Stores och persistensadaptrar
- DTO:er och modeller
- Auth, claims och policies
- Konfiguration och miljöskillnader

## Förslag till development-guidelines.md

Guidelines bör minst täcka:

- Hur man lägger till en ny feature.
- Hur man lägger till navigation.
- Hur man lägger till settings-sidor.
- Hur man definierar en ny access claim.
- Hur man skriver en ny store.
- Hur Mongo-entiteter och mappers ska struktureras.
- Hur komponenter bör använda services och stores.
- Hur localization ska användas.
- Hur secrets ska hanteras.
- Hur async-kod ska skrivas.
- Hur testprojekt bör namnges och placeras.

## Nästa steg

Rekommenderade nästa steg:

1. Uppdatera README så det beskriver Kalkylinsiktsportalen, inte en generisk app-plattform.
2. Skapa `architecture.md` baserat på mönstren ovan.
3. Skapa en enkel domänordlista för Bidcon, kalkyler, vyer, claims och organisationer.
4. Skapa de första ADR:erna för Blazor Server, feature-registrering, MongoDB och claims.
5. Bestäm om `PwCracker` ska vara kvar i lösningen.
6. Flytta secrets ur `appsettings.json`.
7. Lägg till ett testprojekt innan större refaktoreringar görs.
