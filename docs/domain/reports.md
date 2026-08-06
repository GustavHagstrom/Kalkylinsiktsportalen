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
- bära sektionernas stabila identiteter inom vyn och deras ordning i rapporten
- bära `EnableChanges` som styr om nya delade `ReplacementSelection` får göras i vyn
- bära livscykelstatus, inklusive aktiv och arkiverad
- vara workspace-gränsen som Projects change sets scope:as mot

### `DataSection`

Identifierbar sektion i en `View` som består av en fri excel-liknande layout av `DataCell`.

Ansvar:

- bära fria celler med text, formler, formatering och namngivna värdereferenser
- läsa metadata och namngivna värden via aktuell `RenderingContext`

### `SheetSection`

Identifierbar sektion i en `View` som visar exakt ett datablad, till exempel `TenderSheet` eller `SummarySheet`, från aktuell `RenderingContext`.

Ansvar:

- bära vilket datablad som ska visas
- bära synlighet och formatering för bladets fält
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
- `SectionIndex` är ordning i rapporten, inte identitet för `DataSection` eller `SheetSection`.
- `EnableChanges` styr rätten att göra nya ändringar, inte om redan sparade ändringar ska appliceras.
- En arkiverad `View` ska inte användas för nya normala arbetsflöden eller nya ändringar.
- Duplicering av en `View` skapar en ny view-definition, inte ett nytt workspace med gamla `ViewScopedChangeSet`.
- Ett `PrintableDocument` måste bära en change token så mätning och paginering kan knytas till samma dokumentversion.
- `PrintableContainer` ska flattenas före paginering och får inte placeras som ett odelbart block.
- Header och footer reserverar höjd på varje sida innan body-block placeras.
- Ett body-block som är högre än tillgänglig body-yta är ett blockerande pagineringsfel.

## Nuvarande Policyer

### `EnableChanges`

Om en användare får använda en view och `EnableChanges` är aktiverat får användaren göra de delade `ReplacementSelection` som Projects tillåter. Detta är en view-egenskap, inte en generell projektinställning.

### Duplicering

Duplicering av en view kopierar:

- view-definition
- relevanta inställningar, inklusive `EnableChanges`

Duplicering kopierar inte:

- befintliga `ViewScopedChangeSet`
- gammalt arbetsstate från andra vyer

### Arkivering

Views med historik, exportrelevans eller `ViewScopedChangeSet`-bakgrund ska kunna arkiveras i stället för att tas bort ur all historik.

Arkivering betyder att vyn:

- döljs från normal ny användning
- bevaras för att förklara historiska rapporter och beslut
- kan användas i läsläge för historik och exportförståelse

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
