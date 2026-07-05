# Access

## Syfte Och Modulgräns

Access äger portalens behörighetsmodell för organisationsbundna rättigheter.

Modulen äger:

- access grants for global feature access
- access grants for view access
- accessbeslut som kombinerar direkta User grants och externa Role/Group grants
- use cases för att läsa, bevilja och återkalla access grants
- settingsytan där administratörer hanterar access

Access äger inte:

- den globala katalogen över access claim metadata
- portalägda roller
- extern identitet, inloggning eller gruppmedlemskap
- admin bypass eller bootstrap-rättigheter
- view-definition, rendering eller `EnableChanges`

`AccessClaimRegistry` är fortsatt den globala katalogen över access claim metadata. Access använder katalogen när den visar och beslutar om rättigheter, men katalogens claim values är stabila systemidentiteter och inte organisationsbundna grants.

## Huvudaggregates

### `Access grant`

Organisationsbunden tilldelning av en `Access claim` eller view-permission till en `User` eller extern `Role/Group`.

Ansvar:

- uttrycka vem som har fått en rättighet i en viss Organization-kontext
- referera en globalt stabil access claim eller en view-specifik permission
- skilja direkta User grants från externa Role/Group grants
- kunna beviljas och återkallas utan att andra claim-värden ändras
- bevara kompatibilitet med befintliga User och Role access-tilldelningar under första migreringsfasen

### `Access claim`

Globalt stabilt rättighetsvärde med metadata som registreras av features eller moduler via `AccessClaimRegistry`.

Ansvar:

- ha ett globalt unikt value
- beskriva vilken portalförmåga ett grant kan ge åtkomst till
- vara samma identitet oavsett Organization

`Access claim` är inte en tilldelning. När en User eller extern Role/Group får en claim i en Organization är det ett `Access grant`.

### View Access

View access är separata accessbeslut för en viss `View`.

Initiala permissions:

- `UseView`
- `EditView`
- `AdministerViewAccess`

Reports äger `View`, view-definition, rendering och `EnableChanges`. Access äger grants och beslut om vem som får använda, redigera eller administrera access för en view.

## Relationer

- En `Access grant` tillhör exakt en Organization.
- En `Access grant` pekar på antingen en `User` eller en extern `Role/Group`.
- En extern `Role/Group` kommer från identitets- eller auth-källan. Access använder gruppen som grant-mottagare men äger inte gruppkatalogen.
- Ett globalt feature grant refererar ett `Access claim`.
- Ett view grant refererar en view-identitet och en view permission.
- Accessbeslut ska respektera både direkta User grants och grants via externa Role/Group-medlemskap.
- Admin bypass/bootstrap ligger utanför lagrade Access grants.

## Invariants

- Ett `Access claim` value får bara ha en betydelse i systemet.
- Ett `Access grant` är organisationsbundet och får inte förväxlas med den globala claim-katalogen.
- Role betyder extern auth-grupp, inte en portalägd roll.
- Admin bypass/bootstrap sparas inte som ett `Access grant`.
- Access ska inte överbelasta globala feature claims för view-specifika rättigheter.
- Första migreringsfasen ska bevara befintliga Mongo collection-namn och dataform för User och Role access.
- Befintliga claim values, inklusive Access administration-claimen, ska vara stabila under första migreringsfasen.

## Nuvarande Policyer

### Admin Bypass

Admin är ett externt bypass- och bootstrap-koncept. Det finns för att systemadministration inte ska kunna låsa ute alla tenant-administratörer.

Admin bypass ska kunna ge åtkomst till Access-administration utan ett lagrat Access grant, och ska inte materialiseras som en User eller Role grant.

### User Och Role Grants

Access ska hantera både direkta User grants och grants till externa Role/Group-mottagare.

Role/Group-grants är beroende av extern gruppmedlemskapstolkning. Access äger beslutet att ett grant finns för gruppen, men inte gruppens livscykel eller medlemskap.

### View Access

View access ska modelleras separat från global feature access.

En User kan till exempel få använda en view utan att få redigera view-definitionen eller administrera viewens access. `UseView`, `EditView` och `AdministerViewAccess` ska därför vara skilda beslut.

## MVP-Begränsningar Och Tillfälliga Val

- Första migrationsfasen kapslar dagens User och Role access utan datamigrering.
- Befintliga routes ska vara fortsatt nåbara eller omdirigeras under migrationen.
- UI kan först flyttas till use cases utan bred settings-redesign.
- Portalägda roller ingår inte i denna PRD.

## Sannolika Framtida Variationer

- tydligare audit trail för beviljade och återkallade grants
- rikare view-access UI när Reports-modellen mognar
- extern gruppsynkning eller bättre visningsmetadata för Role/Group-mottagare
- fler view-permissions om Reports får fler ansvarsnivåer

## Relaterade ADR:er

- [../adr/global/0041-use-access-claim-registry.md](../adr/global/0041-use-access-claim-registry.md)
