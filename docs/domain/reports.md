# Reports

## Syfte Och Modulgräns

Reports äger rapportvyer, view-definitioner, sektioner, layout, rendering och view-livscykel.

Modulen äger:

- `View`
- view-definition med identifierbara `DataSection` och `SheetSection`
- komponentnära rendering från `View` och aktuell `RenderingContext` till mätbart rapportinnehåll
- pagineringspolicy för renderingsmodellens `Printable Block` och `Printable Container`
- regler för när en view är ändringsbar eller arkiverad
- rendering från `RenderingContext` till rapportstruktur
- PDF-export och relaterade exportbeslut

Reports äger inte:

- validering eller lagring av `ViewScopedChangeSet`
- mutation av `EstimationSnapshot`
- snapshot- och versionsmodell i Projects

Reports avsiktliga publika kontrakt är smala. `ViewId` är publik eftersom Access och andra moduler behöver referera en Reports-ägd view utan att känna till view-definitionen. `View`, view-definitionens sektioner, routes, renderers, stores, `PrintableDocument` och komponentstate är Reports-interna modeller eller adapters.

## Huvudaggregate

### `View`

Organisationsägd rapportmall som beskriver hur kalkyldata ska visas genom identifierbara sektioner, initialt `DataSection` och `SheetSection`.

Ansvar:

- bära rapportdefinitionen
- bära en ordnad sektionssamling med sektionernas stabila identiteter inom vyn
- äga strukturella ändringar av rapporten, till exempel att lägga till, ta bort och flytta sektioner
- bära `EnableChanges` som styr om nya delade `ReplacementSelection` får göras i vyn
- bära livscykelstatus, inklusive aktiv och arkiverad
- vara workspace-gränsen som Projects change sets scope:as mot

`View` muterar inte sektioners interna innehåll direkt. `DataSection` och `SheetSection` äger sina egna interna invariants och defaults, medan `View` äger den ordnade sektionssamlingen och vilka sektionstyper som får ingå.

### `DataSection`

Identifierbar sektion i en `View` som består av en fri excel-liknande layout av `DataCell`.

Ansvar:

- bära fria celler med text, formler, formatering och namngivna värdereferenser
- skydda sin egen rad-, kolumn-, cell- och layoutkonsistens
- läsa metadata och namngivna värden via aktuell `RenderingContext`

### `SheetSection`

Identifierbar sektion i en `View` som visar exakt ett datablad, till exempel `TenderSheet` eller `SummarySheet`, från aktuell `RenderingContext`.

Ansvar:

- bära vilket datablad som ska visas
- bära synlighet och formatering för bladets fält
- skydda sina egna fält-, synlighets- och formateringsregler
- visa standardiserad radindikator när en rad är ändrad

## Renderingsmodeller Och Adapters

### `PrintableDocument`

Komponentnära modellkontrakt för ett renderbart rapportdokument. Dokumentet bär stabil identitet, obligatorisk change token, sidformat samt separata header-, body- och footer-items.

Ansvar:

- beskriva dokumentets renderbara struktur utan beroende på browsermätning, JS eller Blazor-renderingslivscykel
- låta header och footer behandlas som fasta upprepade regioner per sida
- låta body-items pagineras genom mätta `Printable Block`
- bära `PageBreakBehavior` som layoutintention på block och containers

### `PrintableContainer`

Logisk gruppering av `PrintableItem` i ett `PrintableDocument`. Containers är inte odelbara vid paginering.

Ansvar:

- samla relaterade items i dokumentmodellen
- flattenas till sina placement-descendants innan paginering
- föra vidare `PageBreakBehavior.After` till sin sista placement-descendant

### `Printable Block`

Odelbar placement-enhet i pagineringen. Ett block har stabil identitet, komponenttyp och parametrar, men pagineringsmotorn använder bara den flattenade blockidentiteten och dess mätta höjd.

Ansvar:

- vara minsta enhet som placeras i sidans body-yta
- kunna starta ny sida efter sig genom `PageBreakBehavior.After`
- ge spårbarhet från pagineringsresultat och fel till källblocket

### `ReportDocumentRenderer`

Adapter mellan rapportdefinitionen och det modellstyrda pagineringsflödet.

Ansvar:

- tolka `View` och aktuell `RenderingContext`, inklusive estimation version, snapshot och eventuellt view-scoped change set
- skapa ett `PrintableDocument` med stabil identitet, sidformat och change token
- översätta rapportsektioner till `PrintableBlock` och `PrintableContainer`
- låta lång eller internt brytbar sektiondata bli flera mätbara block utan att pagineringsmotorn behöver känna till rapportdomänen

## Relationer

- Reports renderar aktuell `RenderingContext`. `RenderableSnapshotState` är tills vidare ett preliminärt arbetsnamn för kontraktet mellan Projects och Reports, inte ett kanoniskt Reports-begrepp.
- `ReportDocumentRenderer` håller `View`- och `RenderingContext`-kunskap utanför `PaginatedDocumentRenderer`, som bara arbetar med `PrintableDocument`.
- En `View` kan användas mot olika snapshot- och versionslägen, men varje `ViewScopedChangeSet` ägs fortfarande av Projects.
- `EnableChanges` på `View` styr om användare får skapa nya view-lokala ändringar, medan Projects fortfarande äger validering, lagring och concurrency.
- När en view dupliceras kopieras view-definitionen, men inte det change set-state som Projects äger för tidigare arbetslägen.

## Invariants

- `View` äger rapportdefinitionen men inte snapshotets sanning eller `ReplacementSelection`-persistens.
- En `View` skapas inom exakt en `Organization` och kan inte flyttas till en annan `Organization`.
- En `View` måste ha ett namn, men namnet behöver inte vara unikt inom en `Organization`.
- En `View` kan ha en valfri beskrivning som metadata för att skilja rapportmallar åt; beskrivningen har ingen renderingsbetydelse om den inte uttryckligen används i en sektion.
- En arkiverad `View` får inte redigeras i normala edit-flöden; den måste återaktiveras eller dupliceras till en aktiv view innan ändringar görs.
- En `View` får ha noll sektioner medan rapportmallen byggs.
- En `View` får innehålla flera sektioner av samma typ och flera `SheetSection` som visar samma datablad; varje sektion skiljs genom sin stabila identitet och egen konfiguration.
- När en sektion tas bort försvinner den ur den aktiva view-definitionen; `View` har ingen separat per-section-arkivering.
- Sektioners ordning kommer från deras position i `View`ens ordnade sektionssamling. `SectionIndex` är ett övergångs-/payloadfält, inte en separat domänsanning.
- En ny sektion får sin `SectionId` när sektionen skapas. `View` skyddar att alla sektioner i samma view har icke-tomma och unika `SectionId`.
- `SectionId` är unik inom sin `View`; externa referenser till en sektion behöver `ViewId` och `SectionId` tillsammans.
- En befintlig sektions typ får inte ändras. Typbyte modelleras som borttag av den gamla sektionen och tillägg av en ny sektion med ny `SectionId`.
- Strukturell redigering identifierar befintliga sektioner med `SectionId`; dubbletter eller saknade sektionsidentiteter är ogiltig edit-input. Under kompatibilitetsperioden får ett editor-payload introducera en ny sektion med ny `SectionId`, men bara om identiteten är unik, icke-tom, sektionstypen är känd och resultatets ordning är giltig.
- Strukturella ändringar uttrycks som intentioner, till exempel `AddSection`, `RemoveSection` och `MoveSection`; whole-view saves är bara en kompatibilitetsbrygga.
- Normalisering är en legacy-/hydration-mekanism. Nya feature-slices ska använda domänbeteenden som håller `View` giltig utan att callers behöver reparera modellen efteråt.
- `EnableChanges` styr rätten att göra nya ändringar, inte om redan sparade ändringar ska appliceras.
- En arkiverad `View` ska inte användas för nya normala arbetsflöden eller nya ändringar.
- Duplicering av en `View` skapar en ny view-definition, inte ett nytt workspace med gamla `ViewScopedChangeSet`.
- Ett `PrintableDocument` måste bära en change token så mätning och paginering kan knytas till samma dokumentversion.
- `PrintableContainer` ska flattenas före paginering och får inte placeras som ett odelbart block.
- Header och footer reserverar höjd på varje sida innan body-block placeras.
- Ett body-block som är högre än tillgänglig body-yta är ett blockerande pagineringsfel.

## Nuvarande Policyer

### Borttag Av Sektion

Borttag av en sektion ska föregås av en tydlig UI-varning, särskilt eftersom Projects kan ha historiska `ViewScopedChangeSet` som refererar sektionens `SectionId`.

### `EnableChanges`

Om en användare får använda en view och `EnableChanges` är aktiverat får användaren göra de delade `ReplacementSelection` som Projects tillåter. Detta är en view-egenskap, inte en generell projektinställning.

Nya views har `EnableChanges` avstängt som standard. Att tillåta nya delade `ReplacementSelection` är ett avsiktligt rapportmallval.

### Duplicering

Duplicering av en view kopierar:

- view-definitionens ordnade sektionssamling
- relevanta inställningar, inklusive `EnableChanges`

Duplicering kopierar inte:

- befintliga `ViewScopedChangeSet`
- gammalt arbetsstate från andra vyer

Duplicerade sektioner får nya `SectionId`; kopian har motsvarande sektioner, inte samma sektioner.

### Arkivering

Views med historik, exportrelevans eller `ViewScopedChangeSet`-bakgrund ska kunna arkiveras i stället för att tas bort ur all historik.

View-livscykeln skiljer initialt bara mellan aktiv och arkiverad. `Draft`/`Published` införs inte förrän view-definitioner behöver publicerings- eller versionsregler.

Arkivering betyder att vyn:

- döljs från normal ny användning
- bevaras för att förklara historiska rapporter och beslut
- kan användas i läsläge för historik och exportförståelse

En arkiverad view kan återaktiveras genom en explicit åtgärd.

### Paginering

Reports pagineringsmotor är ren C#-logik. Den tar ett `PrintableDocument`, uppmätta header/footer-höjder och uppmätta body-block, flattenar containers och returnerar sidor, warnings och blockerande fel.

Motorn paginerar bara body-block. Header och footer repeteras som fasta regioner på varje sida och minskar den tillgängliga body-ytan. Om nästa block inte ryms på aktuell sida fortsätter motorn på nästa sida. Om ett enskilt block inte ryms i body-ytan returneras ett blockerande `Pagination Error`.

`PaginatedDocumentRenderer` äger den säkra print-ingången. Browser print öppnas bara när den paginerade preview-DOM:en är aktuell och saknar blockerande `Pagination Error`. Browserns print-to-PDF är den initiala PDF-vägen för renderern, så PDF-export i MVP använder samma paginerade DOM som användaren förhandsgranskar.

## MVP-Begränsningar Och Tillfälliga Val

- första PDF-exporten utgår från aktivt öppnat view-läge snarare än bredare historikhantering
- lagrade exportartefakter är inte standard i MVP

## Sannolika Framtida Variationer

- tydligare skillnad mellan arbetsvyer, publicerade rapportvyer och arkiverade historikvyer
- rikare exporthistorik
- fler view- eller rapportnivåer för olika målgrupper eller distributionssätt

## Relaterade ADR:er

- [../adr/modules/reports/0015-export-active-view-state-to-pdf.md](../adr/modules/reports/0015-export-active-view-state-to-pdf.md)
- [../adr/modules/reports/0022-archive-views-with-history.md](../adr/modules/reports/0022-archive-views-with-history.md)
- [../adr/modules/reports/0023-generate-pdf-exports-on-demand-for-mvp.md](../adr/modules/reports/0023-generate-pdf-exports-on-demand-for-mvp.md)
