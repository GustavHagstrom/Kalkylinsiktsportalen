# Access

## Syfte Och Modulgräns

Access äger portalens authorization-modell och organisationsbundna rättigheter.

Modulen äger:

- portalclaims och deras metadata
- authorization policies och access checks
- application-admin-semantik och bootstrap-beslut
- access grants för global feature access
- access grants för view access
- accessbeslut som kombinerar direkta User grants och externa Role/Group grants
- use cases för att läsa, bevilja och återkalla access grants
- användar- och rollkataloguppslag som behövs för grantadministration
- Access-specifika katalogprojektioner, caches, endpoints och settingsyta

Access äger inte:

- generell extern identitet eller inloggningspresentation
- den aktuella användarsessionen
- den externa katalogens livscykel eller gruppmedlemskap
- råa Entra- eller Microsoft Graph-modeller
- view-definition, rendering eller `EnableChanges`

AppShell äger generell identity mapping, aktuell användarsession och loginpresentation. Access tolkar sessionens och autentiseringsgränsens information till portalclaims, policies, adminsemantik och accessbeslut.

`AccessClaimRegistry` är en Access-ägd katalog över globalt stabil claim metadata. Andra moduler kan registrera sina claim-definitioner genom Access-ägda eller composition-medierade kontrakt, men claim-vokabulären och dess authorization-betydelse ägs av Access.

## Huvudaggregates

### `Access grant`

Organisationsbunden tilldelning av en `Access claim` eller view-permission till en `User` eller extern `Role/Group`.

Ansvar:

- uttrycka vem som har fått en rättighet i en viss Organization-kontext
- referera en globalt stabil access claim eller en view-specifik permission
- skilja direkta User grants från externa Role/Group grants
- kunna beviljas och återkallas utan att andra claim-värden ändras
- bevara kompatibilitet med befintliga User- och Role-tilldelningar under migrationen

### `Access claim`

Globalt stabilt rättighetsvärde med metadata i Access-ägda `AccessClaimRegistry`.

Ansvar:

- ha ett globalt unikt value
- beskriva vilken portalförmåga ett grant kan ge åtkomst till
- vara samma identitet oavsett Organization
- ge Access ett sammanhållet authorization-språk för policies och administration

`Access claim` är inte en tilldelning. När en User eller extern Role/Group får en claim i en Organization är det ett `Access grant`.

### View Access

View access är separata accessbeslut för en viss `View`.

Initiala permissions:

- `UseView`
- `EditView`
- `AdministerViewAccess`

Reports äger `View`, view-definition, rendering och `EnableChanges`. Access äger grants och beslut om vem som får använda, redigera eller administrera access för en view. Cross-module-sambandet ska gå genom Reports publika View-identitet och Access publika authorization-kontrakt.

## Katalogprojektioner

Access använder användar- och rollkatalogen för att hitta mottagare av grants.

- Access äger projektionerna som behövs av grantadministrationen.
- Access äger cachepolicy för dessa uppslag.
- Integrationsadaptern mappar råa Graph- eller provider-modeller till Access-ägda projektioner.
- Råa externa modeller får inte exponeras som application- eller cross-module-kontrakt.
- Access äger inte den externa katalogens livscykel eller medlemskapsdata som källa.

## Relationer

- En `Access grant` tillhör exakt en Organization.
- En `Access grant` pekar på antingen en `User` eller en extern `Role/Group`.
- En extern `Role/Group` kommer från identitets- eller auth-källan. Access använder den som grant-mottagare och kan slå upp visningsdata, men äger inte gruppens externa livscykel.
- Ett globalt feature grant refererar ett `Access claim`.
- Ett view grant refererar en view-identitet och en view permission.
- Accessbeslut ska respektera både direkta User grants och grants via externa Role/Group-medlemskap.
- Admin bypass/bootstrap ligger utanför lagrade Access grants men dess portalbetydelse ägs av Access.

## Invariants

- Ett `Access claim` value får bara ha en betydelse i systemet.
- Ett `Access grant` är organisationsbundet och får inte förväxlas med den globala claim-katalogen.
- Role betyder extern auth-grupp, inte en portalägd roll.
- Admin bypass/bootstrap sparas inte som ett `Access grant`.
- Access ska inte överbelasta globala feature claims för view-specifika rättigheter.
- Befintliga Mongo collection-namn och dataform för User- och Role-access ska bevaras tills en uttrycklig datamigrering beslutas.
- Befintliga claim values, inklusive Access administration-claimen, ska vara stabila.
- Featurekod ska konsumera sessions- och authorization-kontrakt i stället för att tolka `ClaimsPrincipal` direkt.

## Nuvarande Policyer

### Admin Bypass

Admin är ett externt claim-baserat bypass- och bootstrap-koncept. Det finns för att systemadministration inte ska kunna låsa ute alla tenant-administratörer.

Access äger tolkningen av adminclaimen och beslutet att den ger portalbehörighet. Admin bypass ska kunna ge åtkomst till Access-administration utan ett lagrat Access grant och ska inte materialiseras som en User- eller Role-grant.

### User Och Role Grants

Access hanterar både direkta User grants och grants till externa Role/Group-mottagare.

Role/Group-grants är beroende av extern gruppmedlemskapstolkning. Access äger authorization-beslutet och grantinformationen, men inte den externa gruppens livscykel eller medlemskap som källa.

### View Access

View access modelleras separat från global feature access.

En User kan få använda en view utan att få redigera view-definitionen eller administrera viewens access. `UseView`, `EditView` och `AdministerViewAccess` är därför skilda beslut.

## MVP-Begränsningar Och Tillfälliga Val

- Nuvarande User- och Role-access kapslas utan automatisk datamigrering.
- Befintliga routes ska vara fortsatt nåbara eller omdirigeras under migrationen.
- Portalägda roller ingår inte i nuvarande modell.
- Externa kataloguppslag kan ha enklare cache- och felhantering tills integrationsbehoven stabiliserats.

## Sannolika Framtida Variationer

- tydligare audit trail för beviljade och återkallade grants
- rikare view-access UI när Reports-modellen mognar
- extern gruppsynkning eller bättre visningsmetadata för Role/Group-mottagare
- fler view-permissions om Reports får fler ansvarsnivåer

## Relaterade ADR:er

- [../adr/global/0041-use-access-claim-registry.md](../adr/global/0041-use-access-claim-registry.md)
- [../adr/global/0042-remove-shared-and-establish-explicit-ownership.md](../adr/global/0042-remove-shared-and-establish-explicit-ownership.md)