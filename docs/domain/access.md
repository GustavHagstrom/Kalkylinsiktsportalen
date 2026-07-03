# Access

## Syfte Och Modulgrans

Access ager portalens behorighetsmodell for organisationsbundna rattigheter.

Modulen ager:

- access grants for global feature access
- access grants for view access
- accessbeslut som kombinerar direkta User grants och externa Role/Group grants
- use cases for att lasa, bevilja och aterkalla access grants
- settingsytan dar administratorer hanterar access

Access ager inte:

- den globala katalogen over access claim metadata
- portalagda roller
- extern identitet, inloggning eller gruppmedlemskap
- admin bypass eller bootstrap-rattigheter
- view-definition, rendering eller `EnableChanges`

`AccessClaimRegistry` ar fortsatt den globala katalogen over access claim metadata. Access anvander katalogen nar den visar och beslutar om rattigheter, men katalogens claim values ar stabila systemidentiteter och inte organisationsbundna grants.

## Huvudaggregates

### `Access grant`

Organisationsbunden tilldelning av en `Access claim` eller view-permission till en `User` eller extern `Role/Group`.

Ansvar:

- uttrycka vem som har fatt en rattighet i en viss Organization-kontext
- referera en globalt stabil access claim eller en view-specifik permission
- skilja direkta User grants fran externa Role/Group grants
- kunna beviljas och aterkallas utan att andra claim-varden andras
- bevara kompatibilitet med befintliga User och Role access-tilldelningar under forsta migreringsfasen

### `Access claim`

Globalt stabilt rattighetsvarde med metadata som registreras av features eller moduler via `AccessClaimRegistry`.

Ansvar:

- ha ett globalt unikt value
- beskriva vilken portalformaga ett grant kan ge atkomst till
- vara samma identitet oavsett Organization

`Access claim` ar inte en tilldelning. Nar en User eller extern Role/Group far en claim i en Organization ar det ett `Access grant`.

### View Access

View access ar separata accessbeslut for en viss `View`.

Initiala permissions:

- `UseView`
- `EditView`
- `AdministerViewAccess`

Reports ager `View`, view-definition, rendering och `EnableChanges`. Access ager grants och beslut om vem som far anvanda, redigera eller administrera access for en view.

## Relationer

- En `Access grant` tillhor exakt en Organization.
- En `Access grant` pekar pa antingen en `User` eller en extern `Role/Group`.
- En extern `Role/Group` kommer fran identitets- eller auth-kallan. Access anvander gruppen som grant-mottagare men ager inte gruppkatalogen.
- Ett globalt feature grant refererar ett `Access claim`.
- Ett view grant refererar en view-identitet och en view permission.
- Accessbeslut ska respektera bade direkta User grants och grants via externa Role/Group-medlemskap.
- Admin bypass/bootstrap ligger utanfor lagrade Access grants.

## Invariants

- Ett `Access claim` value far bara ha en betydelse i systemet.
- Ett `Access grant` ar organisationsbundet och far inte forvaxlas med den globala claim-katalogen.
- Role betyder extern auth-grupp, inte en portalagd roll.
- Admin bypass/bootstrap sparas inte som ett `Access grant`.
- Access ska inte overbelasta globala feature claims for view-specifika rattigheter.
- Forsta migreringsfasen ska bevara befintliga Mongo collection-namn och dataform for User och Role access.
- Befintliga claim values, inklusive Access administration-claimen, ska vara stabila under forsta migreringsfasen.

## Nuvarande Policyer

### Admin Bypass

Admin ar ett externt bypass- och bootstrap-koncept. Det finns for att systemadministration inte ska kunna lasa ute alla tenant-administratorer.

Admin bypass ska kunna ge atkomst till Access-administration utan ett lagrat Access grant, och ska inte materialiseras som en User eller Role grant.

### User Och Role Grants

Access ska hantera bade direkta User grants och grants till externa Role/Group-mottagare.

Role/Group-grants ar beroende av extern gruppmedlemskapstolkning. Access ager beslutet att ett grant finns for gruppen, men inte gruppens livscykel eller medlemskap.

### View Access

View access ska modelleras separat fran global feature access.

En User kan till exempel fa anvanda en view utan att fa redigera view-definitionen eller administrera viewens access. `UseView`, `EditView` och `AdministerViewAccess` ska darfor vara skilda beslut.

## MVP-Begransningar Och Tillfalliga Val

- Forsta migrationsfasen kapslar dagens User och Role access utan datamigrering.
- Befintliga routes ska vara fortsatt nattbara eller omdirigeras under migrationen.
- UI kan forst flyttas till use cases utan bred settings-redesign.
- Portalagda roller ingar inte i denna PRD.

## Sannolika Framtida Variationer

- tydligare audit trail for beviljade och aterkallade grants
- rikare view-access UI nar Reports-modellen mognar
- extern gruppsynkning eller battre visningsmetadata for Role/Group-mottagare
- fler view-permissions om Reports far fler ansvarsnivaer

## Relaterade ADR:er

- [../adr/global/0041-use-access-claim-registry.md](../adr/global/0041-use-access-claim-registry.md)
