# Projects

## Syfte Och Modulgräns

Projects äger portalens arbetsmodell för projekt, projektkopplade estimations, versionsval och delade view-lokala ändringar ovanpå importerad Bidcon-data.

Modulen äger:

- `Project`
- `ProjectEstimation`
- `EstimationVersion`
- `ViewScopedChangeSet`
- `Ersättningsval`
- `RenderableSnapshotState` som applikationskontrakt för aktiv renderbar kalkylstate

Projects äger inte:

- view-definition, layout eller PDF-rendering
- Bidcon API-integration, importmekanik eller snapshot-lagring
- `EstimationSnapshot` utöver att referera snapshot via `EstimationSnapshotId`
- accessregler utanför de kontrakt som krävs för use cases

## Huvudaggregates

### `Project`

Portalägt arbetsobjekt som samlar en eller flera Bidcon-estimations i en organisationskontext.

Ansvar:

- hålla ihop projektets identitet och övergripande livscykel
- koppla projektet till relevanta `ProjectEstimation`
- vara referenspunkt för fristående `ProjectEstimation`-aggregat i samma organisationskontext

### `ProjectEstimation`

Portalägd koppling mellan ett `Project` och en Bidcon-estimation.

Ansvar:

- ha en egen stabil identitet inom Projects-domänen
- vara eget aggregate root för estimationens livscykel i projektet
- representera att en viss estimation tillhör ett visst projekt
- referera extern estimation-identitet utan att vara identifierad av den
- bära sin egen versionskedja över tid
- peka ut exakt en aktiv `EstimationVersion` som standardläge för UI och use cases
- kunna mjukavslutas utan att tappa versionshistorik eller sparade ändringar
- kunna återaktiveras i samma `Project` utan att få ny domänidentitet
- återfå sin senast aktiva `EstimationVersion` vid återaktivering
- återfå den återaktiverade versionens bundna `ViewScopedChangeSet` som aktivt arbetsläge
- avgränsa versionsansvar så att andra estimations i samma projekt inte uppdateras automatiskt

### `EstimationVersion`

Namngiven sparad versionspunkt för en enskild `ProjectEstimation` som refererar ett bestämt `EstimationSnapshot`.

Ansvar:

- vara en intern entity i `ProjectEstimation`-aggregatet
- ha en egen stabil identitet som inte beror på versionsordning inom ett projekt eller en estimation
- bära ett eget versionsnummer som kan visas och användas i UI
- använda sekventiell versionsnumrering inom sin `ProjectEstimation`
- beskriva vilket snapshot som gäller för en viss `ProjectEstimation` i en viss sparad version
- uppstå endast när en ny import skapar ett nytt `EstimationSnapshot`
- vara jämförbar och spårbar över tid
- bära sammanhanget där `ViewScopedChangeSet` används

`EstimationVersion` äger inte kopior av snapshotdata eller färdigrenderad state.

### `ViewScopedChangeSet`

Portalägd delad ändringsmängd för kombinationen `View`, `EstimationVersion` och `EstimationSnapshot`.

Ansvar:

- vara eget aggregate för delat versionsbundet arbetsläge
- ha egen stabil identitet utan att tappa den domänunika kombinationen av view, version och snapshot
- bära delade användarval utan att mutera snapshotet
- valideras mot snapshotets struktur och ersättningspolicy
- fungera som ett workspace för en specifik rapportvy i en specifik sparad estimation-version

Projects äger `ViewScopedChangeSet` även när scoping sker till en `View`.

### `Ersättningsval`

Sparat val i ett `ViewScopedChangeSet` som pekar ut vilken originalrad som ersatts av vilken ersättningsrad.

Ansvar:

- uttrycka ett användarvalt byte, inte ett materialiserat kalkylresultat
- bära metadata som behövs för spårbarhet och tolkning
- vara entydigt inom ett givet `ViewScopedChangeSet`

### `RenderableSnapshotState`

Applikationskontrakt som Projects producerar för att beskriva den aktiva kalkylstaten som Reports kan rendera.

Ansvar:

- representera aktivt snapshotläge efter att ersättningsval applicerats
- visa både originaldata och aktivt ersatt läge där det behövs för förklaring eller rendering
- ge Reports ett läsbart kontrakt utan att göra Reports beroende av Projects interna persistensmodell

`RenderableSnapshotState` är inte källsanning och inte ett persisterat aggregate.

## Relationer

- Ett `Project` samlar en eller flera `ProjectEstimation`.
- En `ProjectEstimation` kopplar en Bidcon-estimation till ett `Project` och bär sin egen versionskedja.
- En `ProjectEstimation` har en egen stabil identitet i Projects-domänen.
- En `ProjectEstimation` är ett eget aggregate root och ägs inte transaktionellt av `Project`.
- En `ProjectEstimation` refererar en extern `BidconEstimationId`, men identifieras inte av den.
- Samma `BidconEstimationId` får förekomma högst en gång inom ett givet `Project`.
- En `ProjectEstimation` har exakt en aktiv `EstimationVersion`.
- En `EstimationVersion` lever inom `ProjectEstimation`-aggregatet och är inte eget aggregate root.
- En `EstimationVersion` refererar exakt ett valt snapshot för exakt en `ProjectEstimation`.
- En `EstimationVersion` har en egen stabil identitet och tillhör exakt en `ProjectEstimation`.
- En `EstimationVersion` har också ett eget versionsnummer som är meningsfullt inom sin `ProjectEstimation`.
- Versionsnumret för `EstimationVersion` är sekventiellt inom en `ProjectEstimation`.
- Ett `EstimationSnapshot` ligger utanför `ProjectEstimation`-aggregatet och refereras via `EstimationSnapshotId`.
- Ett `ViewScopedChangeSet` är eget aggregate och refererar en `EstimationVersion` i stället för att ligga inne i `ProjectEstimation`.
- Ett `ViewScopedChangeSet` har ett eget internt id.
- Ett `ViewScopedChangeSet` är unikt per kombinationen `View`, `EstimationVersion` och `EstimationSnapshot`.
- Ett `ViewScopedChangeSet` innehåller högst ett aktivt `Ersättningsval` per originalrad.
- Projects producerar `RenderableSnapshotState` från `EstimationSnapshot` plus `ViewScopedChangeSet` i kontexten av en `View` och en `EstimationVersion`.

## Invariants

- Användarförändringar sparas inte genom mutation av snapshotet.
- `ProjectEstimation` identifieras inte bara av extern estimation-identitet eller sin plats i ett projekt.
- `ProjectEstimation` har ett eget internt id i Projects-domänen.
- `Project` och `ProjectEstimation` har separata aggregate-gränser.
- Ett `Project` kan inte innehålla flera `ProjectEstimation` med samma `BidconEstimationId`.
- `EstimationVersion` har ingen egen aggregate-gräns utanför sin `ProjectEstimation`.
- `EstimationSnapshot` ingår inte som intern del av `ProjectEstimation`-aggregatet.
- `ViewScopedChangeSet` har egen aggregate-gräns och egen samtidighetskontroll skild från `ProjectEstimation`.
- `ViewScopedChangeSet` identifieras med eget internt id, men den domänunika kombinationen `View` + `EstimationVersion` + `EstimationSnapshot` får bara förekomma en gång.
- En borttagen estimation mjukavslutas i `Project`, i stället för att hårdraderas med sin historik.
- En mjukavslutad `ProjectEstimation` kan senare återaktiveras i samma `Project`.
- En återaktiverad `ProjectEstimation` får tillbaka sin senast aktiva `EstimationVersion`.
- En återaktiverad `ProjectEstimation` får också tillbaka den versionens bundna `ViewScopedChangeSet` som aktivt arbetsläge.
- `EstimationVersion` identifieras inte av sin plats eller ordning inom ett `Project` eller inom en `ProjectEstimation`.
- `EstimationVersion` har ett versionsnummer som kan användas av användare och support för att prata om versioner.
- `EstimationVersion`-nummer återanvänds inte inom samma `ProjectEstimation`.
- En ny `EstimationVersion` motsvarar alltid en ny import; versioner skapas inte genom manuell versionsmärkning.
- En ny version i en `ProjectEstimation` skapar inte automatiskt en ny version för andra estimations i samma `Project`.
- Varje `ProjectEstimation` har alltid exakt en aktiv `EstimationVersion`.
- När en ny `EstimationVersion` skapas genom import blir den automatiskt aktiv för sin `ProjectEstimation`.
- Aktiv `EstimationVersion` kan ändras manuellt till en äldre version genom ett explicit användarval.
- När aktiv version byts till en äldre `EstimationVersion` blir den versionens bundna `ViewScopedChangeSet` åter de aktiva ändringarna.
- Om en aktiv `EstimationVersion` saknar `ViewScopedChangeSet` för en viss `View` betyder det att den viewn inte har några sparade ändringar i den versionen.
- `ViewScopedChangeSet` är alltid bundet till exakt en `View`, exakt en `EstimationVersion` och exakt ett `EstimationSnapshot`.
- Ett `Ersättningsval` är alltid snapshot- och vy-lokalt, inte globalt eller automatiskt återanvänt mellan versioner.
- Ett `ViewScopedChangeSet` har högst ett aktivt ersättningsval per originalrad.
- `EstimationVersion` refererar snapshots, men äger inte kopierad snapshotdata eller kopierade change sets.
- `ViewScopedChangeSet` flyttas inte automatiskt till en ny `EstimationVersion`, utan ligger kvar bundet till den version där valet skapades.
- `RenderableSnapshotState` är ett läskontrakt, inte en källa för framtida mutationer.

## Nuvarande Policyer

### Delad Ändringsyta

`ViewScopedChangeSet` delas mellan användare som arbetar i samma view, estimation-version och snapshotläge. Senaste giltiga ändring blir den aktiva, och samtidiga uppdateringar ska hanteras med optimistic concurrency.

### Ersättningar Sparas Som Val

Ersättningar sparas som användarval, inte som färdiga omräknade resultat. Projects bygger den renderbara kalkylstaten genom att applicera valen mot snapshotet.

### Version-Lokala Val

Ersättningsval förs inte automatiskt vidare till senare importer eller andra estimation-versioner. Ett nytt snapshotläge kräver nya bekräftade val.

### Gamla Ändringar Ligger Kvar På Gammal Version

När en ny `EstimationVersion` importeras ligger befintliga `ViewScopedChangeSet` kvar på den tidigare versionen. Systemet försöker inte automatiskt flytta eller återanvända ändringar på den nya versionen.

### Versionsansvar Per Estimation

Varje `ProjectEstimation` ansvarar för sin egen versionskedja. Nya importer eller sparade versioner för en estimation skapar inte automatiskt nya versioner för andra estimations i samma projekt. Ett framtida use case kan erbjuda ett bekvämlighetsflöde för att skapa nya versioner för flera estimations samtidigt, men det ändrar inte ägarskapet för versionshistoriken.

`ProjectEstimation` behandlas som ett eget domänobjekt, inte bara som en lös koppling mellan `Project` och extern estimation.

`ProjectEstimation` är också ett eget aggregate root. `Project` bär inte estimationens versionshistorik, aktiva version eller mjukavslutningsregler inom samma transaktionsgräns.

`EstimationVersion` lever inne i detta aggregate som en entity under `ProjectEstimation`-reglerna, i stället för som eget aggregate root.

`EstimationVersion` pekar på snapshotet i stället för att bära snapshotinnehåll inne i aggregatet.

`ViewScopedChangeSet` är ett separat aggregate. Det delas mellan användare, har egen samtidighetsmodell och refererar den `EstimationVersion` som ändringarna tillhör.

I kod och referenser får `ViewScopedChangeSet` ett eget id, medan domänregeln om unik kombination av `View`, `EstimationVersion` och `EstimationSnapshot` fortfarande upprätthålls som invariant.

### Borttagning Bevarar Historik

När en estimation tas bort från ett projekt mjukavslutas dess `ProjectEstimation`. Versionshistorik, aktiv version-historik och bundna `ViewScopedChangeSet` bevaras för spårbarhet och eventuell senare analys.

En mjukavslutad `ProjectEstimation` kan senare återaktiveras i samma projekt i stället för att skapas om som ett nytt domänobjekt.

Vid återaktivering blir den senast aktiva `EstimationVersion` aktiv igen.

Den versionens egna `ViewScopedChangeSet` blir samtidigt det aktiva arbetslaget igen.

### Aktiv Version Per Estimation

Varje `ProjectEstimation` har exakt en aktiv `EstimationVersion` som UI och standard-use-cases arbetar mot. Andra versioner finns kvar för historik, jämförelse eller manuell omvaljning, men är inte aktiva samtidigt.

När en ny import skapar en ny `EstimationVersion` blir den automatiskt den aktiva versionen för estimationen.

Användaren kan också explicit byta tillbaka aktiv version till en äldre `EstimationVersion`.

När en äldre version åter blir aktiv följer dess egna `ViewScopedChangeSet` med tillbaka som aktiva ändringar för den versionen.

Om den aktiva versionen saknar `ViewScopedChangeSet` för en viss `View` visas det som att den viewn inte har några ändringar i den versionen. Systemet försöker inte ärva ändringar från andra versioner.

### Ny Version Kräver Ny Import

I modellen betyder ny version alltid ny import. Användaren skapar inte fristående manuella versioner utan nytt snapshotunderlag.

### Ersättningspolicy För Taggade Rader

Den nuvarande policyn matchar befintligt portalbeteende:

- Bidcon-rader på samma trädnivå får ersätta varandra när de delar minst en klammertagg i `Remark`
- originalradens mängd appliceras på ersättningsraden
- endast kandidater med tillräcklig data för säker rendering ska vara valbara i första UI:t
- alla `{tagg}` behandlas initialt som ersättningstaggar av kompatibilitetsskäl

Policyn ska hållas modulär så att syntax och regler senare kan förfinas utan att ändra snapshotets grundmodell.

### Befintliga Ändringar Fortsätter Gälla

Om `View.EnableChanges` stängs av i Reports betyder det att nya ändringar inte får göras. Befintliga `ViewScopedChangeSet` fortsätter ändå att påverka rendering tills de rensas eller ersätts.

## MVP-Begränsningar Och Tillfälliga Val

- Undo och redo för `ViewScopedChangeSet` stöds inte i MVP.
- `ClearViewScopedChanges` behandlas som en bekräftad permanent åtgärd.
- Bidcon-regler som ännu inte är fullt förstådda kan behöva exponeras snarare än räknas om fullt ut i den aktiva staten.

## Sannolika Framtida Variationer

- bekvämlighetsflöde för att skapa nya versioner för flera estimations samtidigt
- föreslagna ersättningsval vid ny import, med explicit användarbekräftelse
- rikare audit trail för tidigare ersättningsval
- mer namngiven eller namespacad taggsyntax, till exempel `{replace:kyla}`
- tydligare stöd för mer komplexa Bidcon-formler och aggregatregler i renderbar state
- eventuell operationslogg om undo och redo blir viktigt

## Relaterade ADR:er

- [../adr/modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md](../adr/modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md)
