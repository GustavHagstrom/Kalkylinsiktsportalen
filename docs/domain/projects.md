# Projects

## Syfte Och Modulgrans

Projects ager portalens arbetsmodell for projekt, estimations, snapshots och delade view-lokala andringar ovanpa importerad Bidcon-data.

Modulen ager:

- `Project`
- `ProjectEstimation`
- `EstimationVersion`
- `EstimationSnapshot`
- `ViewScopedChangeSet`
- `Ersattningsval`
- `RenderableSnapshotState` som applikationskontrakt for aktiv renderbar kalkylstate

Projects ager inte:

- view-definition, layout eller PDF-rendering
- Bidcon API-integration och importmekanik
- accessregler utanfor de kontrakt som kravs for use cases

## Huvudaggregates

### `Project`

Portalagt arbetsobjekt som samlar en eller flera Bidcon-estimations i en organisationskontext.

Ansvar:

- halla ihop projektets identitet och overgripande livscykel
- koppla projektet till relevanta `ProjectEstimation`

### `ProjectEstimation`

Portalagd koppling mellan ett `Project` och en Bidcon-estimation.

Ansvar:

- ha en egen stabil identitet inom Projects-domänen
- representera att en viss estimation tillhor ett visst projekt
- referera extern estimation-identitet utan att vara identifierad av den
- bara sin egen versionskedja over tid
- peka ut exakt en aktiv `EstimationVersion` som standardlage for UI och use cases
- kunna mjukavslutas utan att tappa versionshistorik eller sparade andringar
- kunna ateraktiveras i samma `Project` utan att fa ny domanidentitet
- aterfa sin senast aktiva `EstimationVersion` vid ateraktivering
- aterfa den ateraktiverade versionens bundna `ViewScopedChangeSet` som aktivt arbetslage
- avgransa versionsansvar sa att andra estimations i samma projekt inte uppdateras automatiskt

### `EstimationVersion`

Namngiven sparad versionspunkt for en enskild `ProjectEstimation` som refererar ett bestamt `EstimationSnapshot`.

Ansvar:

- ha en egen stabil identitet som inte beror pa versionsordning inom ett projekt eller en estimation
- beskriva vilket snapshot som galler for en viss `ProjectEstimation` i en viss sparad version
- uppsta endast nar en ny import skapar ett nytt `EstimationSnapshot`
- vara jamforbar och sparbar over tid
- bara sammanhanget dar `ViewScopedChangeSet` anvands

`EstimationVersion` ager inte kopior av snapshotdata eller fardigrenderad state.

### `EstimationSnapshot`

Oforanderlig komplett import av en Bidcon-estimation vid ett importtillfalle.

Ansvar:

- vara kallsanning for importerad kalkyldata
- kunna tolkas av Projects use cases for att bygga aktiv state
- bevara den importerade strukturen utan portalmutationer

`EstimationSnapshot` ar inte ett muterbart arbetsobjekt.

### `ViewScopedChangeSet`

Portalagd delad andringsmangd for kombinationen `View`, `EstimationVersion` och `EstimationSnapshot`.

Ansvar:

- bara delade anvandarval utan att mutera snapshotet
- valideras mot snapshotets struktur och ersattningspolicy
- fungera som ett workspace for en specifik rapportvy i en specifik sparad estimation-version

Projects ager `ViewScopedChangeSet` aven nar scoping sker till en `View`.

### `Ersattningsval`

Sparat val i ett `ViewScopedChangeSet` som pekar ut vilken originalrad som ersatts av vilken ersattningsrad.

Ansvar:

- uttrycka ett anvandarvalt byte, inte ett materialiserat kalkylresultat
- bara metadata som behovs for sparbarhet och tolkning
- vara entydigt inom ett givet `ViewScopedChangeSet`

### `RenderableSnapshotState`

Applikationskontrakt som Projects producerar for att beskriva den aktiva kalkylstaten som Reports kan rendera.

Ansvar:

- representera aktivt snapshotlage efter att ersattningsval applicerats
- visa bade originaldata och aktivt ersatt lage dar det behovs for forklaring eller rendering
- ge Reports ett lasbart kontrakt utan att gora Reports beroende av Projects interna persistensmodell

`RenderableSnapshotState` ar inte kallsanning och inte ett persisterat aggregate.

## Relationer

- Ett `Project` samlar en eller flera `ProjectEstimation`.
- En `ProjectEstimation` kopplar en Bidcon-estimation till ett `Project` och bar sin egen versionskedja.
- En `ProjectEstimation` har en egen stabil identitet i Projects-domänen.
- En `ProjectEstimation` refererar en extern `BidconEstimationId`, men identifieras inte av den.
- Samma `BidconEstimationId` far forekomma hogst en gang inom ett givet `Project`.
- En `ProjectEstimation` har exakt en aktiv `EstimationVersion`.
- En `EstimationVersion` refererar exakt ett valt snapshot for exakt en `ProjectEstimation`.
- En `EstimationVersion` har en egen stabil identitet och tillhor exakt en `ProjectEstimation`.
- Ett `EstimationSnapshot` representerar en specifik importerad estimation vid en viss tidpunkt.
- Ett `ViewScopedChangeSet` ar unikt per kombinationen `View`, `EstimationVersion` och `EstimationSnapshot`.
- Ett `ViewScopedChangeSet` innehaller hogst ett aktivt `Ersattningsval` per originalrad.
- Projects producerar `RenderableSnapshotState` fran `EstimationSnapshot` plus `ViewScopedChangeSet` i kontexten av en `View` och en `EstimationVersion`.

## Invariants

- `EstimationSnapshot` ar oforanderlig efter import.
- Anvandarforandringar sparas inte genom mutation av snapshotet.
- `ProjectEstimation` identifieras inte bara av extern estimation-identitet eller sin plats i ett projekt.
- `ProjectEstimation` har ett eget internt id i Projects-domänen.
- Ett `Project` kan inte innehalla flera `ProjectEstimation` med samma `BidconEstimationId`.
- En borttagen estimation mjukavslutas i `Project`, i stallet for att hardraderas med sin historik.
- En mjukavslutad `ProjectEstimation` kan senare ateraktiveras i samma `Project`.
- En ateraktiverad `ProjectEstimation` far tillbaka sin senast aktiva `EstimationVersion`.
- En ateraktiverad `ProjectEstimation` far ocksa tillbaka den versionens bundna `ViewScopedChangeSet` som aktivt arbetslage.
- `EstimationVersion` identifieras inte av sin plats eller ordning inom ett `Project` eller inom en `ProjectEstimation`.
- En ny `EstimationVersion` motsvarar alltid en ny import; versioner skapas inte genom manuell versionsmarkning.
- En ny version i en `ProjectEstimation` skapar inte automatiskt en ny version for andra estimations i samma `Project`.
- Varje `ProjectEstimation` har alltid exakt en aktiv `EstimationVersion`.
- Nar en ny `EstimationVersion` skapas genom import blir den automatiskt aktiv for sin `ProjectEstimation`.
- Aktiv `EstimationVersion` kan andras manuellt till en aldre version genom ett explicit anvandarval.
- Nar aktiv version byts till en aldre `EstimationVersion` blir den versionens bundna `ViewScopedChangeSet` ater de aktiva andringarna.
- Om en aktiv `EstimationVersion` saknar `ViewScopedChangeSet` for en viss `View` betyder det att den viewn inte har några sparade andringar i den versionen.
- `ViewScopedChangeSet` ar alltid bundet till exakt en `View`, exakt en `EstimationVersion` och exakt ett `EstimationSnapshot`.
- Ett `Ersattningsval` ar alltid snapshot- och vy-lokalt, inte globalt eller automatiskt ateranvant mellan versioner.
- Ett `ViewScopedChangeSet` har hogst ett aktivt ersattningsval per originalrad.
- `EstimationVersion` refererar snapshots, men ager inte kopierad snapshotdata eller kopierade change sets.
- `ViewScopedChangeSet` flyttas inte automatiskt till en ny `EstimationVersion`, utan ligger kvar bundet till den version dar valet skapades.
- `RenderableSnapshotState` ar ett laskontrakt, inte en kalla for framtida mutationer.

## Nuvarande Policyer

### Delad Andringsyta

`ViewScopedChangeSet` delas mellan anvandare som arbetar i samma view, estimation-version och snapshotlage. Senaste giltiga andring blir den aktiva, och samtidiga uppdateringar ska hanteras med optimistic concurrency.

### Ersattningar Sparas Som Val

Ersattningar sparas som anvandarval, inte som fardiga omraknade resultat. Projects bygger den renderbara kalkylstaten genom att applicera valen mot snapshotet.

### Version-Lokala Val

Ersattningsval fors inte automatiskt vidare till senare importer eller andra estimation-versioner. Ett nytt snapshotlage kraver nya bekräftade val.

### Gamla Andringar Ligger Kvar Pa Gammal Version

Nar en ny `EstimationVersion` importeras ligger befintliga `ViewScopedChangeSet` kvar pa den tidigare versionen. Systemet forsoker inte automatiskt flytta eller ateranvanda andringar pa den nya versionen.

### Versionsansvar Per Estimation

Varje `ProjectEstimation` ansvarar for sin egen versionskedja. Nya importer eller sparade versioner for en estimation skapar inte automatiskt nya versioner for andra estimations i samma projekt. Ett framtida use case kan erbjuda ett bekvamlighetsflode for att skapa nya versioner for flera estimations samtidigt, men det andrar inte agarskapet for versionshistoriken.

`ProjectEstimation` behandlas som ett eget domänobjekt, inte bara som en los koppling mellan `Project` och extern estimation.

### Borttagning Bevarar Historik

Nar en estimation tas bort fran ett projekt mjukavslutas dess `ProjectEstimation`. Versionshistorik, aktiv version-historik och bundna `ViewScopedChangeSet` bevaras for sparbarhet och eventuell senare analys.

En mjukavslutad `ProjectEstimation` kan senare ateraktiveras i samma projekt i stallet for att skapats om som ett nytt domanobjekt.

Vid ateraktivering blir den senast aktiva `EstimationVersion` aktiv igen.

Den versionens egna `ViewScopedChangeSet` blir samtidigt det aktiva arbetslaget igen.

### Aktiv Version Per Estimation

Varje `ProjectEstimation` har exakt en aktiv `EstimationVersion` som UI och standard-use-cases arbetar mot. Andra versioner finns kvar for historik, jamforelse eller manuell omvaljning, men ar inte aktiva samtidigt.

Nar en ny import skapar en ny `EstimationVersion` blir den automatiskt den aktiva versionen for estimationen.

Anvandaren kan ocksa explicit byta tillbaka aktiv version till en aldre `EstimationVersion`.

Nar en aldre version ater blir aktiv foljer dess egna `ViewScopedChangeSet` med tillbaka som aktiva andringar for den versionen.

Om den aktiva versionen saknar `ViewScopedChangeSet` for en viss `View` visas det som att den viewn inte har några andringar i den versionen. Systemet forsoker inte arva andringar fran andra versioner.

### Ny Version Kraver Ny Import

I modellen betyder ny version alltid ny import. Anvandaren skapar inte fristaende manuella versioner utan nytt snapshotunderlag.

### Ersattningspolicy For Taggade Rader

Den nuvarande policyn matchar befintligt portalbeteende:

- Bidcon-rader pa samma tradniva far ersatta varandra nar de delar minst en klammertagg i `Remark`
- originalradens mangd appliceras pa ersattningsraden
- endast kandidater med tillracklig data for saker rendering ska vara valbara i forsta UI:t
- alla `{tagg}` behandlas initialt som ersattningstaggar av kompatibilitetsskal

Policyn ska hallas modular sa att syntax och regler senare kan forfinas utan att andra snapshotets grundmodell.

### Befintliga Andringar Fortsatter Galla

Om `View.EnableChanges` stangs av i Reports betyder det att nya andringar inte far goras. Befintliga `ViewScopedChangeSet` fortsatter anda att paverka rendering tills de rensas eller ersatts.

## MVP-Begransningar Och Tillfalliga Val

- Undo och redo for `ViewScopedChangeSet` stods inte i MVP.
- `ClearViewScopedChanges` behandlas som en bekräftad permanent atgard.
- Bidcon-regler som annu inte ar fullt forstadda kan behova exponeras snarare an raknas om fullt ut i den aktiva staten.

## Sannolika Framtida Variationer

- bekvamlighetsflode for att skapa nya versioner for flera estimations samtidigt
- foreslagna ersattningsval vid ny import, med explicit anvandarbekraftelse
- rikare audit trail for tidigare ersattningsval
- mer namngiven eller namespacad taggsyntax, till exempel `{replace:kyla}`
- tydligare stod for mer komplexa Bidcon-formler och aggregatregler i renderbar state
- eventuell operationslogg om undo och redo blir viktigt

## Relaterade ADR:er

- [../adr/modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md](../adr/modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md)
