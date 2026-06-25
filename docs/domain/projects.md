# Projects

## Syfte Och Modulgräns

Projects äger portalens arbetsmodell för projekt, versioner, snapshots och delade view-lokala ändringar ovanpå importerad Bidcon-data.

Modulen äger:

- `Project`
- `Projektversion`
- `EstimationSnapshot`
- `ViewScopedChangeSet`
- `Ersättningsval`
- `RenderableSnapshotState` som applikationskontrakt för aktiv renderbar kalkylstate

Projects äger inte:

- view-definition, layout eller PDF-rendering
- Bidcon API-integration och importmekanik
- accessregler utanför de kontrakt som krävs för use cases

## Huvudaggregates

### `Project`

Portalägt arbetsobjekt som samlar en eller flera Bidcon-estimations i en organisationskontext.

Ansvar:

- hålla ihop projektets identitet och övergripande livscykel
- koppla projektet till relevanta `ProjectEstimation`
- bära de `Projektversion` som beskriver sparade projektlägen

### `Projektversion`

Namngivet sparat projektläge som refererar ett bestämt urval av `EstimationSnapshot`.

Ansvar:

- beskriva vilka snapshots som ingår i ett visst projektläge
- vara jämförbar och spårbar över tid
- bära sammanhanget där `ViewScopedChangeSet` används

`Projektversion` äger inte kopior av snapshotdata eller färdigrenderad state.

### `EstimationSnapshot`

Oföränderlig komplett import av en Bidcon-estimation vid ett importtillfälle.

Ansvar:

- vara källsanning för importerad kalkyldata
- kunna tolkas av Projects use cases för att bygga aktiv state
- bevara den importerade strukturen utan portalmutationer

`EstimationSnapshot` är inte ett muterbart arbetsobjekt.

### `ViewScopedChangeSet`

Portalägd delad ändringsmängd för kombinationen `View`, `Projektversion` och `EstimationSnapshot`.

Ansvar:

- bära delade användarval utan att mutera snapshotet
- valideras mot snapshotets struktur och ersättningspolicy
- fungera som ett workspace för en specifik rapportvy i ett specifikt projektläge

Projects äger `ViewScopedChangeSet` även när scoping sker till en `View`.

### `Ersättningsval`

Sparat val i ett `ViewScopedChangeSet` som pekar ut vilken originalrad som ersätts av vilken ersättningsrad.

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
- En `Projektversion` refererar ett valt snapshot per relevant `ProjectEstimation`.
- Ett `EstimationSnapshot` representerar en specifik importerad estimation vid en viss tidpunkt.
- Ett `ViewScopedChangeSet` är unikt per kombinationen `View`, `Projektversion` och `EstimationSnapshot`.
- Ett `ViewScopedChangeSet` innehåller högst ett aktivt `Ersättningsval` per originalrad.
- Projects producerar `RenderableSnapshotState` från `EstimationSnapshot` plus `ViewScopedChangeSet` i kontexten av en `View` och en `Projektversion`.

## Invariants

- `EstimationSnapshot` är oföränderlig efter import.
- Användarförändringar sparas inte genom mutation av snapshotet.
- `ViewScopedChangeSet` är alltid bundet till exakt en `View`, exakt en `Projektversion` och exakt ett `EstimationSnapshot`.
- Ett `Ersättningsval` är alltid snapshot- och vy-lokalt, inte globalt eller automatiskt återanvänt mellan versioner.
- Ett `ViewScopedChangeSet` har högst ett aktivt ersättningsval per originalrad.
- `Projektversion` refererar snapshots, men äger inte kopierad snapshotdata eller kopierade change sets.
- `RenderableSnapshotState` är ett läskontrakt, inte en källa för framtida mutationer.

## Nuvarande Policyer

### Delad Ändringsyta

`ViewScopedChangeSet` delas mellan användare som arbetar i samma view, projektversion och snapshotläge. Senaste giltiga ändring blir den aktiva, och samtidiga uppdateringar ska hanteras med optimistic concurrency.

### Ersättningar Sparas Som Val

Ersättningar sparas som användarval, inte som färdiga omräknade resultat. Projects bygger den renderbara kalkylstaten genom att applicera valen mot snapshotet.

### Version-Lokala Val

Ersättningsval förs inte automatiskt vidare till senare importer eller andra projektversioner. Ett nytt snapshotläge kräver nya bekräftade val.

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

- föreslagna ersättningsval vid ny import, med explicit användarbekräftelse
- rikare audit trail för tidigare ersättningsval
- mer namngiven eller namespacad taggsyntax, till exempel `{replace:kyla}`
- tydligare stöd för mer komplexa Bidcon-formler och aggregatregler i renderbar state
- eventuell operationslogg om undo och redo blir viktigt

## Relaterade ADR:er

- [../adr/modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md](../adr/modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md)
