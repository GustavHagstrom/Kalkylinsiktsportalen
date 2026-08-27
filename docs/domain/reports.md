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
- bära valt `PageFormat` för rapportmallen
- äga strukturella ändringar av rapporten, till exempel att lägga till, ta bort och flytta sektioner
- bära livscykelstatus, inklusive aktiv och arkiverad
- vara workspace-gränsen som Projects change sets scope:as mot

`View` muterar inte sektioners interna innehåll direkt. `DataSection` och `SheetSection` äger sina egna interna invariants och defaults, medan `View` äger den ordnade sektionssamlingen och vilka sektionstyper som får ingå.

### `DataSection`

Identifierbar sektion i en `View` som består av en fri spreadsheet-like layout av `DataCell`.

Ansvar:

- bära en fri grid-layout med celler som kan innehålla text, formler, formatering och namngivna värdereferenser
- bära rad- och kolumnstorlekar som del av rapportlayouten
- bära cell- och textformat som del av rapportmallens layout
- skydda sin egen rad-, kolumn-, cell- och layoutkonsistens
- läsa metadata och namngivna värden via aktuell `RenderingContext`
- inte själv tolka celluttryck till displayvärden

### `SheetSection`

Identifierbar sektion i en `View` som visar exakt ett helt Bidcon-datablad, till exempel `TenderSheet` eller `SummarySheet`, från aktuell `RenderingContext`.

Ansvar:

- bära `SheetKind`, vilket explicit väljer vilket datablad som ska visas
- bära om nya `ReplacementSelection` får göras i just den sheet section och vilka `ReplacementTag` som är giltiga där
- bära ordning och synlighet för bladets kolumner genom explicit `SheetColumnKind`
- bära synlighet och formatering för kombinationen `SheetRowKind` och `SheetColumnKind`
- skydda sina egna fält-, synlighets- och formateringsregler
- visa standardiserad radindikator när en rad är ändrad

`Content` är legacy-/payloadspråk och ska inte användas som domänbegrepp för `SheetSection`. En eventuell rubrik eller beskrivande label för en sheet section är ett separat layoutval, inte samma sak som vilket datablad sektionen renderar.

Nuvarande implementation är en migrationsmodell mot den här målbilden. Den etablerar `SheetSection` som identifierbar sektion med explicit sheet-källa, sparse cellkonfiguration och renderingsstyrande kolumnkonfiguration. Under migration får koden använda förenklade `ColumnType`/`RowType`-namn så länge gränsen är lokal för Reports och inte sprids som kanoniskt domänspråk. Den förenklade `ColumnType`-konfigurationen är ändå nuvarande Reports-sanning för kolumnordning, synlighet och relativ bredd tills den ersätts av den fulla `SheetColumnKind`-modellen. Nuvarande implementation implementerar inte ännu hela `SheetColumnKind`/`SheetRowKind`-målmodellen, `ChangeIndicator` eller snapshotdriven sheet-row-rendering.

Målmodellen låter ändringsförutsättningarna ligga på `SheetSection`, inte på hela `View`. `ViewScopedChangeSet` är fortfarande scoped till `View`, estimation version och snapshot eftersom Projects utför och lagrar delade val för rapportmallen som helhet, men varje nytt `ReplacementSelection` måste vara giltigt enligt den `SheetSection` där ändringen görs. Dagens `View.EnableChanges` är legacy-/kompatibilitetsspråk eller en temporär default under migration, inte långsiktig domänsanning.

En change-enabled `SheetSection` definierar en explicit lista av giltiga `ReplacementTag` för sektionen. Taggarna matchas mot snapshotradens `Remark`-kolumn. Bara taggar i sektionens lista kan göra syskonrader till replacement-kandidater. Om två syskonrader i samma sheet tree har matchande giltiga `ReplacementTag` och samma `SheetRowKind` enligt sektionens konfiguration är de en giltig replacement-kandidat inom den `SheetSection`, förutsatt att övriga Projects-regler för att utföra och lagra valet uppfylls. Tom tagglista betyder att inga replacements är möjliga i sektionen även om `Remark` innehåller klammer-taggar; i praktiken är det samma användareffekt som att sektionen inte är change-enabled. `SheetSection` definierar alltså förutsättningarna för giltiga replacements i rapportmallen; Projects utför, validerar concurrency och lagrar `ReplacementSelection`.

`ReplacementTag` lagras som kanoniskt taggvärde utan klammer. Remark-parsern extraherar till exempel `{minTagg}` till `minTagg`, och matchning görs på det kanoniska värdet. UI kan visa taggar med eller utan klammer, men domänen lagrar inte `{}` som del av taggvärdet.

Remark-parsern trimmar whitespace innanför klammer, så `{ minTagg }` blir `minTagg`. Matchning av `ReplacementTag` är case-sensitive; `minTagg` och `Mintagg` är olika taggar.

En sheet-rad kan ha flera giltiga `ReplacementTag` i `Remark` och kan därför ingå i flera replacement-kandidatgrupper samtidigt. För varje giltig tagg bildas en separat kandidatgrupp bland syskonrader som delar samma tagg. När användaren gör ett `ReplacementSelection` sparas vilken matchande `ReplacementTag` valet gjordes genom.

En `ReplacementGroup` uppstår bara när minst två syskonrader delar samma giltiga `ReplacementTag`. En ensam rad med en giltig tagg är taggad men har inga replacement-kandidater och ska inte erbjuda replacement-UI.

Initialt kräver replacement-kandidater samma `SheetRowKind`. Syskonrader med matchande giltig `ReplacementTag` men olika `SheetRowKind` bildar inte en giltig replacement-kandidatgrupp. Cross-kind replacements är en framtida explicit policyvariation om behovet uppstår.

Replacement-kandidater måste vara renderbara i berörd `SheetSection`. Candidate discovery utgår från rader som passerar sektionens `SheetRowKind`-synlighet; rader med dold `SheetRowKind` blir inte replacement-kandidater i den sektionen. Om användaren ska kunna välja en rad som replacement måste dess radtyp vara synlig i samma `SheetSection`.

`Remark`-kolumnen behöver inte vara synlig för att replacement-taggar ska gälla. `ReplacementTag` läses från snapshotradens `Remark`-fält oavsett om `Remark` visas som `SheetColumnKind` i berörd `SheetSection`. Kolumnsynlighet är presentation; replacement-förutsättningar använder källdata i snapshotet.

En rad kan inte ersätta sig själv. Originalraden ingår i `ReplacementGroup` för att gruppen ska hittas, men den ska inte erbjudas som ersättningskandidat för sig själv. Ett `ReplacementSelection` måste peka på olika `SheetRowPath` för originalrad och ersättningsrad.

En `ReplacementGroup` skapar inga automatiska `ReplacementSelection`. Candidate discovery presenterar andra syskonrader i samma grupp som möjliga ersättningsrader, och användaren väljer explicit vilken ersättning som ska sparas.

Det finns högst ett aktivt `ReplacementSelection` per `SheetSection` och original `SheetRowPath`. Även om originalraden ingår i flera kandidatgrupper via flera `ReplacementTag` kan den bara vara ersatt av en rad åt gången i samma `SheetSection`. `ReplacementSelection` sparar vilken tagg valet gjordes genom.

Samma ersättningsrad får användas av flera olika originalrader samtidigt i första modellen. Unikhetsregeln gäller originalraden, inte replacement-raden. Varje selection är spårbar genom original `SheetRowPath`, replacement `SheetRowPath` och matchande `ReplacementTag`.

Vid rendering av ett aktivt `ReplacementSelection` visas ersättningsradens värden på originalradens plats. Initial `ReplacementPolicy` är systemdefault och inte konfigurerbar per `SheetSection`: visa ersättningsradens värden på originalradens plats men behåll originalradens `Quantity`. Det betyder att till exempel `Remark`, `Description`, `Code` och `Unit` normalt kommer från ersättningsraden. Replacement är en overlay på originalpositionen, inte en mutation av snapshotet och inte ett filter som tar bort ersättningsraden från dess egen plats. Om ersättningsraden också renderas som vanlig snapshotrad på sin ursprungliga plats fortsätter den göra det, om den inte filtreras bort av andra `SheetSection`-regler.

`ReplacementSelection` sparar använd `ReplacementPolicy`-version vid valtillfället även när policyn är systemdefault. Det gör historiken begriplig om systempolicyn ändras senare.

`SheetSection`ens giltiga `ReplacementTag` styr skapande av nya `ReplacementSelection`, inte applicering av redan sparade selections. En sparad `ReplacementSelection` fortsätter appliceras så länge den refererar existerande snapshotrader i rätt snapshot och inte aktivt tas bort eller ersätts. Att ändra tagglistan på rapportmallen ska alltså inte retroaktivt sluta visa historiska val.

Eftersom `ReplacementSelection` är scoped till ett immutable `EstimationSnapshot` ska en sparad `SheetRowPath` inte försvinna ur samma snapshot. Om ett sparat val någon gång inte kan lösas mot snapshotet är det ett data-/hydrationfel, inte ett normalt domänläge.

Borttag eller ångring av en `ReplacementSelection` är Projects-ägt. `SheetSection` definierar replacement-förutsättningar för rapportmallen men äger inte lifecycle för sparade selections.

Arkiverade views och deras `SheetSection` får inte ändra giltiga `ReplacementTag` eller andra ändringsförutsättningar via normala edit-flöden. Vyn måste återaktiveras eller dupliceras innan sådana ändringar görs.

När en `SheetSection` dupliceras kopieras dess presentation, change-enabled-läge och giltiga `ReplacementTag`, men den duplicerade sektionen får ny `SectionId`. Befintliga `ReplacementSelection` kopieras inte.

Initialt får replacement-kandidater bara skapas för leaf-rader i sheet-trädet. Rader med barn bildar inte replacement-kandidater även om de har matchande giltiga `ReplacementTag`. Replacement av hela subträd är en framtida explicit policyvariation.

Replacement overlay använder originalradens plats och `SheetRowKind` för visibility och format. Cell visibility och format nycklat på `SheetRowKind` och `SheetColumnKind` utvärderas alltså mot originalradens `SheetRowKind`, medan displayvärden hämtas från ersättningsraden med originalradens mängd enligt policy. `ChangeIndicator` visar att raden är ersatt.

`SheetSection` använder en explicit `SheetColumnKind` för kolumner i stället för fria kolumnnamn. `SheetColumnKind` omfattar både Bidcon-baserade kolumner, till exempel description, code, remark, active-status, quantity, unit och reference, och Reports-ägda kolumner, till exempel comment eller change-indikator om sådana behövs. Kolumnordning och kolumnsynlighet är konfiguration på `SheetSection`. Cellkonfiguration, värdesynlighet och format nycklas på kombinationen `SheetRowKind` och `SheetColumnKind`.

`SheetColumnKind` kan vara Bidcon-backed eller Reports-owned. Bidcon-backed kolumner hämtar sina värden från snapshotraden. Reports-owned kolumner härleder sina värden från aktuell `RenderingContext`, till exempel en change-indikator från applicerade `ReplacementSelection`. `ChangeIndicator` är en målmodellkolumn som ska bli en vanlig Reports-owned `SheetColumnKind` i den ordnade kolumnlistan när change-set rendering kopplas in. Den ingår inte i nuvarande förenklade `ColumnType`-katalog. När den införs kan den formateras, flyttas eller döljas som andra kolumner. Nya change-enabled `SheetSection` bör då defaulta till att inkludera `ChangeIndicator`, men domänen kräver inte att den alltid är synlig och dess synlighet är inte en förutsättning för att skapa `ReplacementSelection`. Spårbarhet för själva ändringen finns fortfarande i Projects/change-set även om indikatorn inte renderas. `Comment` är en framtida Reports-owned kolumn tills radkommentarers ägarskap, identitet, lifecycle och relation till `ViewScopedChangeSet` är beslutad. `SheetSection` lagrar inte radvärden för Reports-owned kolumner; om en framtida comment-kolumn kräver användarinmatade kommentarer per rad ska det vara separat Projects-ägt change/comment state, inte data i rapportmallen.

`SheetColumnKind` bär kolumnens semantiska displaytyp och default `ValueFormat`. En mängdkolumn är därför numeric och har number-format som default; textkolumner är text; eventuella datumkolumner har date/time-default. `SheetSection` lagrar bara overrides från kolumnens semantiska default, till exempel decimalantal, tusentalsseparator, textstil eller cell-layout. Detta skiljer `SheetSection` från `DataSection`, där `ValueFormat` måste väljas explicit eftersom cellinnehållet är användarinput och inte ska auto-tolkas.

Första `SheetSection`-formatmodellen har systemdefault, kolumnens semantiska default och cell override per `SheetRowKind` och `SheetColumnKind`. Den har inte separata cascading formatlager per hel `SheetRowKind` eller hel `SheetColumnKind` initialt. Radnivå- och kolumnnivå-format som egna lager är framtida variationer om behovet uppstår.

`SheetSection` lagrar `SheetCellConfiguration` sparse. En saknad cellkonfiguration för `SheetRowKind` och `SheetColumnKind` betyder systemdefault, kolumnens semantiska default och eventuell defaultpresentation från valt `SheetKind`. `SheetSection` lagrar bara avvikelser, till exempel `HideValue`, `CellLayout`, `TextStyle` eller `ValueFormat`-overrides. Helt default cellkonfigurationer prunas efter edit. Värdets grundtyp kommer från `SheetColumnKind`, inte från den sparse cellkonfigurationen.

Default för cellvärdesynlighet är att visa värdet. En defaultpresentation kan tekniskt ange dolda värden för specifika `SheetRowKind` och `SheetColumnKind`, men initial katalog bör inte använda sådana default-hidden-regler utan ett konkret behov. Normalt uttrycks dold cellvärdesynlighet som en explicit `SheetCellConfiguration`-override i den berörda `SheetSection`.

`HideValue` döljer bara cellens displayvärde. Cellen finns kvar i tabellen och behåller cell-layout, textstil, borders, bakgrund, padding och kolumnbredd. `HideValue` är ett presentationsval för värdet, inte ett layoutfilter.

`HideValue` på en Bidcon-backed cell påverkar inte framtida `ChangeIndicator`. `ChangeIndicator` är en egen Reports-owned `SheetColumnKind` i målmodellen och kan fortfarande visas även när andra kolumnvärden på samma rad är dolda. Om change-indikering ska döljas görs det genom att ta bort/dölja `ChangeIndicator`-kolumnen eller genom explicit cellvärdesynlighet för den kolumnen.

Kolumnsynlighet, kolumnordning och cellvärdesynlighet är olika val. `SheetSection` har en ordnad lista av synliga `SheetColumnKind`; listpositionen är kolumnordningen och varje `SheetColumnKind` får förekomma högst en gång i samma `SheetSection`. Kolumner som inte ingår renderas inte alls. Per kombination av `SheetRowKind` och `SheetColumnKind` kan cellens värde döljas medan kolumnplatsen, cell-layouten, formatet och radstrukturen fortfarande finns kvar.

Kolumnbredd är del av `SheetSection`ens rapportlayout. `SheetSection` bär relativ bredd/vikt per synlig `SheetColumnKind`; saknad explicit bredd betyder kolumnens systemdefault från `SheetColumnKind` eller valt `SheetKind`'s defaultpresentation. Tillgänglig totalbredd kommer från rendering/container och används för att räkna om relativa kolumnvikter.

`SheetSection` bär inte radhöjd per `SheetRowKind` i första modellen. Radhöjd härleds av rendering från cellinnehåll, textstil, padding och tillgänglig bredd. En renderad sheet-rad som inte ryms i aktuell sidbody blir ett blockerande pagineringsfel. Radhöjd per `SheetRowKind` är en sannolik framtida layoutvariation, särskilt för rubrik- eller grupprader, men ingår inte initialt.

`SheetSection` kan visa en header-rad med displaynamn för de synliga kolumnerna i aktuell kolumnordning. Header-raden är en del av sheet-tabellens rendering, inte en snapshotrad. Nuvarande implementation bär `ShowHeaders`, med default `true`, och rendering ska skapa ett header-block när `ShowHeaders` är true. Headertext får under migration komma från den förenklade `ColumnType`-katalogen tills den fulla `SheetColumnKind`-katalogen finns. Pagineringsmotorn är domänblind och ska inte känna till sheet headers eller automatiskt upprepa dem vid sidbrytning. Egna header-labels per kolumn och upprepade sheet headers vid pagination är framtida variationer.

Headern har ingen `SheetRowKind`. Cellkonfiguration som nycklas på `SheetRowKind` och `SheetColumnKind` gäller bara data rows från snapshotet. Headern använder systemdefault header style i första modellen; egna header-format, till exempel `HeaderTextStyle` eller `HeaderCellLayout`, är en framtida section-level variation.

Vid rendering översätter `ReportDocumentRenderer`/sheet-adaptern en `SheetSection` till en `PrintableContainer` med stabil identitet för sektionen. Containern innehåller en följd av mätbara `Printable Block`: ett header-block när `ShowHeaders` är true, följt av ett block per renderad sheet-rad. Hela sheet-tabellen renderas inte som ett enda odelbart block. Containern är inte odelbar, men ger gruppering, spårbarhet och en plats för framtida section-level page-break behavior. Pagineringsmotorn ser bara generiska blocks och containers och behöver inte känna till sheet-rader, tabeller eller headers.

Ett renderat sheet row-block identifieras stabilt genom `SectionId`, `SheetKind` och snapshotradens `SheetRowPath`. `SheetKind` identifierar databladets root, så `SheetRowPath` börjar vid rootens renderbara barn. Pathen är sekvensen av syskonindex enligt den importerade `Children`-listans ordning, till exempel `0` för första top-level-raden och `0/3` för dess fjärde barn. Pathen är deterministisk inom ett immutable `EstimationSnapshot` och beskriver radens plats i snapshotets importerade träd. `RowOrder` är inte ensam unik inom ett helt `SheetKind`, och `ExternalId` och `Code` är andra orelaterade fält; de ska inte användas som sheet row identity. `ReplacementSelection`, `ChangeIndicator` och renderingsspårning ska peka på samma snapshotrad genom `SheetRowPath` även när omkringliggande rader filtreras bort eller presentationen ändras.

I första implementationen sparas `SheetRowPath` inte fysiskt på snapshotpayloadens noder. Reports/Projects använder en snapshot-row adapter som traverserar snapshotets tree, tilldelar `SheetRowPath` från `Children`-index och producerar renderbara sheet rows. Om flera moduler senare behöver samma path som persisted data kan den materialiseras i en ny snapshotpayload-version.

`SheetSection` använder i målmodellen en explicit `SheetRowKind` för bladets radtyper. `SheetRowKind` täcker de Bidcon-radtyper som kan renderas från snapshotet, till exempel group, part, posting, layer-rader, quantity posting, locked stages, text och invoice. Nuvarande implementation får använda `RowType.Group/Part/Other` som lokalt migrationsspråk i Reports tills snapshot-row-adaptern och full `SheetRowKind` finns. `RowType` är inte kanoniskt domänspråk och ska ersättas eller mappas bort under migration.

Radtyp-synlighet och cellvärdesynlighet är också olika val. `SheetSection` kan konfigurera vilka `SheetRowKind` som renderas; en dold radtyp renderas inte alls. Om radtypen renderas kan enskilda cellvärden fortfarande döljas per kombination av `SheetRowKind` och `SheetColumnKind`.

Radtyp-synlighet filtrerar varje rad utifrån radens egen `SheetRowKind`; den döljer inte automatiskt radens hela subträd. Om en parent-radtyp är dold men ett barn har synlig `SheetRowKind` kan barnet fortfarande renderas. Hierarki och indentation härleds fortfarande från originalträdet. Att dölja hela subträd är ett separat innehållsfilter och ingår inte initialt.

När parent-rader filtreras bort komprimerar `SheetSection` inte hierarkin för synliga barn. Indentation och nivåvisning utgår från snapshotets originalträd, inte från den filtrerade synliga listan. `SheetSection` filtrerar presentationen men skriver inte om hierarkin.

`SheetSection` omordnar inte faktiska rader från databladet. Radordningen kommer från valt `SheetKind` i aktuell `RenderingContext`, inklusive Bidcons hierarki och order. `SheetSection` kan filtrera bort radtyper men är inte en egen sorterings- eller omstruktureringsmodell för Bidcon-rader.

`SheetSection` bevarar Bidcon-radernas hierarki som renderingsinformation men äger inte hierarkin. Snapshotets sheet-rader kan bära nivå eller parent-child-struktur, och renderingen kan visa den strukturen genom till exempel indentation i description-kolumnen eller annan standardlayout. `SheetSection` kan formatera per `SheetRowKind` och `SheetColumnKind`, men kan inte flytta rader mellan nivåer eller ändra trädstrukturen.

Hierarkivisning är initialt systemstandard, inte konfiguration på `SheetSection`. Renderingen visar snapshotets hierarki på ett standardiserat sätt, initialt i description-kolumnen. `SheetSection` bär inte egna inställningar för indent-size, expand/collapse eller separat nivåkolumn i första modellen.

Initialt har `SheetSection` inga egna innehållsfilter utöver synliga `SheetRowKind`. Den filtrerar inte på radinnehåll, taggar, aktiv-status eller hierarkisubträd. Sådana filter är framtida variationer och måste modelleras explicit när behovet finns, särskilt eftersom de påverkar radidentitet, ändringsindikatorer och hur totalsummor förstås.

`SheetSection` skapar inte egna syntetiska subtotal- eller totalrader i första modellen. Den renderar snapshotets sheet-rader, filtrerade på `SheetRowKind`, plus Reports-owned kolumnvärden på dessa rader. Om subtotaler eller totaler behövs ska de antingen komma från Bidcon-bladet som riktiga snapshotrader eller modelleras senare som explicita Reports-owned rows med egna identitets- och placeringsregler.

Varje `SheetKind` har en kuraterad defaultpresentation med synliga kolumner, kolumnordning och synliga radtyper. En ny `SheetSection` startar från valt `SheetKind`'s defaultpresentation, inte från alla möjliga kolumner/radtyper och inte från en tom tabell. Användaren kan sedan ändra kolumner, radtyper, cellvärdesynlighet och format.

`SheetKind` är immutable efter att en `SheetSection` har skapats. Om användaren vill byta från ett datablad till ett annat modelleras det som borttag av den gamla `SheetSection` och tillägg av en ny med ny `SectionId`. Det skyddar historiska `ViewScopedChangeSet`, formatkonfiguration, cellkonfiguration och sheet-specifika defaults från att byta betydelse under samma sektionsidentitet.

En `View` får innehålla flera `SheetSection` med samma `SheetKind`. De skiljs genom `SectionId` och egen konfiguration, till exempel en detaljerad change-enabled `TenderSheet` och en sammanfattad read-only `TenderSheet` med annat kolumnurval.

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
- använda Reports cell evaluation för att härleda `DataCell`-displayvärden från `CellExpression` och aktuell `RenderingContext`

## Relationer

- Reports renderar aktuell `RenderingContext`. `RenderableSnapshotState` är tills vidare ett preliminärt arbetsnamn för kontraktet mellan Projects och Reports, inte ett kanoniskt Reports-begrepp.
- `ReportDocumentRenderer` håller `View`- och `RenderingContext`-kunskap utanför `PaginatedDocumentRenderer`, som bara arbetar med `PrintableDocument`.
- En `View` kan användas mot olika snapshot- och versionslägen, men varje `ViewScopedChangeSet` ägs fortfarande av Projects.
- Ändringsförutsättningarna på en `SheetSection` styr om användare får skapa nya view-lokala ändringar i just den sektionen och vilka `ReplacementTag` som gör syskonrader till replacement-kandidater, medan Projects fortfarande äger utförande, lagring och concurrency för `ViewScopedChangeSet`.
- När en view dupliceras kopieras view-definitionen, men inte det change set-state som Projects äger för tidigare arbetslägen.

## Invariants

- `View` äger rapportdefinitionen men inte snapshotets sanning eller `ReplacementSelection`-persistens.
- En `View` skapas inom exakt en `Organization` och kan inte flyttas till en annan `Organization`.
- En `View` måste ha ett namn, men namnet behöver inte vara unikt inom en `Organization`.
- En `View` kan ha en valfri beskrivning som metadata för att skilja rapportmallar åt; beskrivningen har ingen renderingsbetydelse om den inte uttryckligen används i en sektion.
- En `View` bär valt `PageFormat` eftersom sidformatet är del av rapportmallens layout. Nya views använder A4 som default tills användaren väljer annat.
- En arkiverad `View` får inte redigeras i normala edit-flöden; den måste återaktiveras eller dupliceras till en aktiv view innan ändringar görs.
- En `View` får ha noll sektioner medan rapportmallen byggs.
- En `View` får innehålla flera sektioner av samma typ och flera `SheetSection` med samma `SheetKind`; varje sektion skiljs genom sin stabila identitet och egen konfiguration.
- När en sektion tas bort försvinner den ur den aktiva view-definitionen; `View` har ingen separat per-section-arkivering.
- Sektioners ordning kommer från deras position i `View`ens ordnade sektionssamling. `SectionIndex` är ett övergångs-/payloadfält, inte en separat domänsanning.
- En ny sektion får sin `SectionId` när sektionen skapas. `View` skyddar att alla sektioner i samma view har icke-tomma och unika `SectionId`.
- `SectionId` är unik inom sin `View`; externa referenser till en sektion behöver `ViewId` och `SectionId` tillsammans.
- En befintlig sektions typ får inte ändras. Typbyte modelleras som borttag av den gamla sektionen och tillägg av en ny sektion med ny `SectionId`.
- Strukturell redigering identifierar befintliga sektioner med `SectionId`; dubbletter eller saknade sektionsidentiteter är ogiltig edit-input. Under kompatibilitetsperioden får ett editor-payload introducera en ny sektion med ny `SectionId`, men bara om identiteten är unik, icke-tom, sektionstypen är känd och resultatets ordning är giltig.
- Hel-view update är ett legitimt bulk-save-use case för rapporteditorn när användaren arbetar med hela rapportmallen som ett dokument. Smalare workflows kan fortfarande uttrycka strukturella ändringar som intentioner, till exempel `AddSection`, `RemoveSection` och `MoveSection`, när det ger tydligare audit, behörighet, concurrency eller användarintention.
- Hel-view update får ersätta hela view-definitionen atomiskt, men måste validera att alla sektioner har icke-tom och unik `SectionId`, att existerande sektioner behåller sin `SectionId`, att en existerande `SectionId` inte byter sektionstyp och att payloadens sektionsordning bevaras. En saknad existerande sektion behandlas som delete bara när borttag är explicit bekräftat. Nya sektioner får introduceras med nya `SectionId`. `DataSection` och `SheetSection` validerar fortsatt sina egna interna invariants.
- Normalisering är en legacy-/hydration-mekanism. Nya feature-slices ska använda domänbeteenden som håller `View` giltig utan att callers behöver reparera modellen efteråt.
- I en `DataSection` är `RowDefinition`- och `DataColumnDefinition`-samlingarna sanningen för gridens storlek. Separata row/column count-fält är derived eller legacy/payload, inte egna domänsanningar.
- En existerande `DataSection` har minst en rad och minst en kolumn.
- En `DataSection` får inte ta bort sin sista rad eller sista kolumn; då ska sektionen tas bort från `View` i stället.
- En ny `DataSection` kan skapas med explicit storlek. En 3x3-grid är en editor-default, inte en affärsregel.
- `DataSection` trimmar inte automatiskt tomma rader eller kolumner. Gridens storlek är ett explicit layoutval.
- Radhöjd är del av rapportlayouten, inte bara editor-state, och uttrycks i millimeter i domänmodellen.
- Radhöjd kan vara decimal men måste vara större än noll.
- Kolumnbredd är del av rapportlayouten, inte bara editor-state, och uttrycks som relativ vikt inom `DataSection`ens tillgängliga bredd.
- Kolumnbreddens relativa vikt kan vara decimal men måste vara större än noll.
- `DataSection` bär inte sin totala renderade bredd. Tillgänglig bredd kommer från rendering/container och används för att omräkna kolumnvikter vid rendering.
- `DataSection` stödjer inte sammanslagna celler i nuläget.
- I en `DataSection` identifieras en `DataCell` av sin position `(rowIndex, columnIndex)` inom sektionen. Stringnycklar som `R0C0` är lookup-/persistence-detaljer, inte domänspråk.
- En `DataSection` får inte innehålla celler utanför aktuell gridstorlek. Nya cell edits utanför gridens bounds ska avvisas i stället för att tyst repareras.
- En `DataCell` lagrar användarens `CellExpression`. Displayvärdet härleds vid rendering; uttryck som börjar med `=` tolkas som formler, annars visas uttrycket som text. Format ligger bredvid celluttrycket.
- `DataCell` har systemdefault för textformat och cellformat. En cell behöver bara lagras när dess uttryck eller format avviker från systemdefault.
- Cell- och textformat i en `DataSection` är del av rapportmallens layout, inte bara editor-state.
- `CellFormat`, `TextFormat` och `BorderFormat` är Reports-domänvärden för rapportlayout. De uttrycker layoutintention; rendering översätter dem till CSS eller annan presentationsrepresentation.
- Formatmodeller är value objects. Målmodellen bör ersätta formatvärden via domänoperationer i stället för att behandla format som muterbara entities.
- Målmodellen skiljer `CellLayout`, `TextStyle` och `ValueFormat`. `CellLayout` äger bakgrund, padding och borders; `TextStyle` äger font, färg, styling och alignment; `ValueFormat` äger plain text/number/date-regler.
- En `DataCell` lagrar bara formatdelar som overrides från systemdefault. Saknade `CellLayout`, `TextStyle` eller `ValueFormat` betyder systemdefault för den formatdelen.
- Efter cell edits eller formatpatchar tas en cell bort ur `DataSection`ens lagrade cellsamling när den åter är helt default: tomt `CellExpression` och inga format-overrides.
- Systemdefault för cellformat är initialt en global Reports-default, inte ett `View`-specifikt tema.
- Dagens `TextFormat` blandar textstil och värdeformat och bör delas enligt målmodellen.
- Default `ValueFormat` är text. Tal- och datumformat måste vara explicit valda; programmet ska inte auto-tolka celler som tal eller datum.
- Number `ValueFormat` tillåter `DecimalCount` mellan 0 och 6.
- Number `ValueFormat` har en enkel `UseThousandsSeparator`-inställning. Själva separatorns tecken och kultur kommer från rendering/export-context, inte från cellen.
- Date/time `ValueFormat` använder en begränsad uppsättning stödda formatpresets, inte fria .NET-formatsträngar.
- Initiala date/time-presets är `Date` och `DateTimeMinute`.
- Bold, italic och underline är boolska text style-val med default `false`.
- Underline behålls som text style-val; andra text decorations, till exempel strike-through, ingår inte initialt.
- Färger i rapportformat uttrycks som validerade hex-färger utan alpha, till exempel `#FFFFFF`. UI kan använda color picker, men domänen ska inte acceptera fria CSS-färgsträngar.
- Hex-färger normaliseras till uppercase kanonisk form, till exempel `#FFFFFF`.
- Default cellbakgrund är ingen fill, inte explicit vit. Explicit vit bakgrund är ett användarvalt format.
- Default textfärg är systemdefault/auto, inte explicit svart. Explicit svart text är ett användarvalt format.
- Font väljs från en begränsad uppsättning stödda rapportfonter. Domänen ska inte acceptera godtyckliga CSS `font-family`-strängar.
- Default font är systemdefault/auto. Nuvarande systemdefault kan vara Calibri, men explicit font lagras bara när användaren väljer en font.
- Fontstorlek uttrycks i points i domänmodellen, inte CSS-pixels.
- Fontstorlek kan vara decimal men måste vara större än noll.
- Cellpadding uttrycks i millimeter i domänmodellen, inte CSS-pixels.
- Cellpadding får vara noll men inte negativ.
- Border width uttrycks i points i domänmodellen, inte CSS-pixels.
- Ingen border är ett separat border-tillstånd. När en border finns måste dess width vara större än noll.
- Border style är en begränsad enum, initialt `Solid`, `Dashed`, `Dotted` och `Double`. `None` är inte en border style utan det separata no-border-tillståndet.
- En cell kan ha olika border per sida: left, right, top och bottom.
- Horisontell och vertikal textplacering är separata formatbegrepp. Horisontell alignment använder `Left`, `Center`, `Right`; vertikal alignment använder `Top`, `Middle`, `Bottom`.
- Default horisontell textplacering härleds från displaytyp när ingen explicit alignment är satt: text vänsterställs, tal högerställs och datum/tid högerställs. Explicit alignment override vinner alltid.
- Default vertikal textplacering i en `DataCell` är bottom.
- Lagrade `DataCell`-format får inte vara null. Legacy-hydration kan reparera null-format till systemdefault, men nya domänoperationer ska inte skapa null-format.
- Cell evaluation är ett Reports-renderingsansvar utanför `DataCell`/`DataSection` entityn. Det ska vara testbart utan Blazor-komponentlivscykel.
- En formel som inte kan tolkas ska ge ett explicit renderingsfel eller felvärde, inte tyst falla tillbaka till att visa `CellExpression` som vanlig text.
- En `DataSection` behöver bara lagra celler som avviker från default. En saknad cell betyder en tom default-cell på den positionen.
- När en rad eller kolumn tas bort ur en `DataSection` flyttas efterföljande rader/kolumner och deras cellinnehåll upp respektive vänster. Grid-positioner är layoutkoordinater, inte stabila rad- eller kolumnidentiteter.
- Rader och kolumner i en `DataSection` identifieras av sin aktuella position inom sektionen, inte av stabila identities.
- En `DataSection` skapas med explicit initial storlek. Efter skapande ändras den genom intentionella operationer som add/remove row/column, resize och cell edits. Full reset/initialize är inte ett normalt edit-beteende och ska vara explicit om det behövs.
- `DataSection` kan applicera format på ett cellintervall som en domänoperation. Sektionen validerar att intervallet ligger inom gridens bounds.
- Formatändringar uttrycks som intentionella, partiella patches när användaren ändrar en del av formatet, till exempel bold, bakgrundsfärg eller top border. En patch ska inte oavsiktligt ersätta orelaterade formatval.
- Formatpatchar kan både sätta ett explicit formatvärde och återställa ett formatfält till systemdefault/auto.
- `DataSection` har ingen separat titel i nuläget. Rapportens rubriker bärs av cellinnehåll i gridden.
- Page-break-regler hör hemma på gemensam section-/rapportblocknivå, inte som interna cell- eller radregler i `DataSection`.
- En `DataSection` kan renderas som flera mätbara radblock för paginering. Den behöver inte placeras som ett enda odelbart `Printable Block`.
- En `SheetSection` kan renderas som flera mätbara blocks för paginering: eventuellt header-block och sedan ett block per renderad sheet-rad.
- `DataSection` validerar inte radhöjd mot en specifik sidbody. En renderad rad som inte ryms i aktuell sidbody blir ett blockerande pagineringsfel.
- Ändringsförutsättningarna på en `SheetSection` styr rätten att göra nya ändringar i den sektionen och vilka replacement-taggar som är giltiga, inte om redan sparade ändringar ska appliceras.
- En arkiverad `View` ska inte användas för nya normala arbetsflöden eller nya ändringar.
- Duplicering av en `View` skapar en ny view-definition, inte ett nytt workspace med gamla `ViewScopedChangeSet`.
- Ett `PrintableDocument` måste bära en change token så mätning och paginering kan knytas till samma dokumentversion.
- `PrintableContainer` ska flattenas före paginering och får inte placeras som ett odelbart block.
- Header och footer reserverar höjd på varje sida innan body-block placeras.
- Ett body-block som är högre än tillgänglig body-yta är ett blockerande pagineringsfel.

## Nuvarande Policyer

### Borttag Av Sektion

Borttag av en sektion ska föregås av en tydlig UI-varning, särskilt eftersom Projects kan ha historiska `ViewScopedChangeSet` som refererar sektionens `SectionId`.

### Ändringsförutsättningar På `SheetSection`

Om en användare får använda en view och berörd `SheetSection` är change-enabled får användaren göra de delade `ReplacementSelection` som är giltiga enligt sektionens replacement-taggar och som Projects kan utföra. Detta är en sheet-section-egenskap, inte en generell projektinställning.

Nya `SheetSection` har ändringsrätt avstängd som standard. Att tillåta nya delade `ReplacementSelection` och välja vilka `ReplacementTag` som gäller är avsiktliga rapportmallval per sheet section.

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

`PageFormat` är ett Reports-domänvärde för namngiven sidstorlek med mått i millimeter, till exempel A3, A4 eller A5. `View` äger valt `PageFormat`; `PrintableDocument` bär sidformatet som renderad input till pagineringsmotorn.

Motorn paginerar bara body-block. Header och footer repeteras som fasta regioner på varje sida och minskar den tillgängliga body-ytan. Om nästa block inte ryms på aktuell sida fortsätter motorn på nästa sida. Om ett enskilt block inte ryms i body-ytan returneras ett blockerande `Pagination Error`.

`PaginatedDocumentRenderer` äger den säkra print-ingången. Browser print öppnas bara när den paginerade preview-DOM:en är aktuell och saknar blockerande `Pagination Error`. Browserns print-to-PDF är den initiala PDF-vägen för renderern, så PDF-export i MVP använder samma paginerade DOM som användaren förhandsgranskar.

Cell evaluation errors visas i editor/preview som felvärden. Som nuvarande policy ska export/print kunna blockeras av cell evaluation errors, men detta kan omprövas när rapportarbetsflödet mognar.

## MVP-Begränsningar Och Tillfälliga Val

- första PDF-exporten utgår från aktivt öppnat view-läge snarare än bredare historikhantering
- lagrade exportartefakter är inte standard i MVP

## Sannolika Framtida Variationer

- tydligare skillnad mellan arbetsvyer, publicerade rapportvyer och arkiverade historikvyer
- rikare exporthistorik
- fler view- eller rapportnivåer för olika målgrupper eller distributionssätt
- cellformler i en `DataSection` som refererar andra celler inom samma sektion, om behovet uppstår
- konfigurerbara defaultformat på `DataSection`, rad eller kolumn, om användare behöver formatmallar utöver systemdefault för celler
- `View`-specifika rapportteman/defaultformat, om användare behöver konsekventa mallteman
- flytt av rader eller kolumner i en `DataSection`, om rapporteditorn behöver omordna gridinnehåll utan manuell kopiering
- gemensamma section-level page-break-regler för `DataSection` och `SheetSection`
- `Comment` som Reports-owned `SheetColumnKind`, om radkommentarers ägarskap, identitet, lifecycle och relation till `ViewScopedChangeSet` beslutas
- innehållsfilter i `SheetSection`, till exempel aktiv-status, taggar eller hierarkisubträd
- konfigurerbar hierarkivisning i `SheetSection`, till exempel indent-size, expand/collapse eller separat nivåkolumn
- radhöjd per `SheetRowKind` i `SheetSection`
- separata cascading formatlager per hel `SheetRowKind` eller hel `SheetColumnKind`
- egna header-labels per `SheetColumnKind` i en `SheetSection`
- egna header-format på `SheetSection`
- upprepade `SheetSection`-headers vid pagination utan att bryta pagineringsmotorns domänblinda ansvar
- Reports-owned subtotal- eller totalrader i `SheetSection`
- cross-kind replacements mellan rader med olika `SheetRowKind`
- `ReplacementPolicy` per `SheetSection`
- replacement av hela subträd eller icke-leaf-rader

## Relaterade ADR:er

- [../adr/modules/reports/0015-export-active-view-state-to-pdf.md](../adr/modules/reports/0015-export-active-view-state-to-pdf.md)
- [../adr/modules/reports/0022-archive-views-with-history.md](../adr/modules/reports/0022-archive-views-with-history.md)
- [../adr/modules/reports/0023-generate-pdf-exports-on-demand-for-mvp.md](../adr/modules/reports/0023-generate-pdf-exports-on-demand-for-mvp.md)
