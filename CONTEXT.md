# Kalkylinsiktsportalen

Det här dokumentet är repoets kanoniska ordlista för domänspråk.

## Language

**Bidcon**:
Externt system som portalen hämtar kalkyl- och estimationsdata från.

**Bidcon API**:
API för autentisering och läsning av Bidcon-data.

**Bidcon credentials**:
Organisationsägda API-uppgifter som portalen använder för att autentisera mot Bidcon API. Saknade Bidcon credentials är ett eget konfigurationsläge, inte samma sak som tomma uppgifter.
_Avoid_: BidconApiCredentials i domänspråk

**Organization**:
Tenant- och avgränsningsnivå för data, rättigheter, views och credentials.
_Avoid_: Organisation

**User**:
Identifierad person i portalen, typiskt via Microsoft Identity eller Graph.
_Avoid_: Användare

**Role**:
Extern auth-grupp som används tillsammans med portalens accessregler. Portalen äger inte en egen rollkatalog i den nuvarande modellen.
_Avoid_: Roll

**Access claim**:
Funktionellt rättighetsvärde som styr vad användare eller roller får göra. Claimens value är dess globala stabila identitet och får bara ha en betydelse i systemet.

**Access grant**:
Organisationsbunden tilldelning av en `Access claim` eller view-permission till en `User` eller extern `Role/Group`. Ett grant beskriver vad någon får göra i en Organization-kontext; själva access claimens globala betydelse ändras inte av grantet.
_Avoid_: Access claim när du menar den lagrade tilldelningen

**Access grant target**:
Mottagaren av ett `Access grant`, antingen en `User` eller en extern `Role/Group`. Direkta user-grants och role-grants är separata källor till access även när de ger samma effektiva `Access claim`.
_Avoid_: Principal när du behöver skilja på user och role/group

**Functional access grant**:
Ett `Access grant` som tilldelar en `Access claim` till en `User` eller extern `Role/Group` i en Organization-kontext. Används när grantet behöver skiljas från view-permissions.
_Avoid_: Access grant när skillnaden mot view-permission spelar roll

**Estimation**:
Centralt affärsobjekt från Bidcon med kalkylinformation och metadata.
_Avoid_: kalkyl, estimat

**Project**:
Portalägt arbetsobjekt som samlar en eller flera Bidcon-estimations.
_Avoid_: Projekt

**ProjectEstimation**:
Koppling mellan ett `Project` och en Bidcon-estimation-identitet.

**EstimationSnapshot**:
Oföränderlig komplett import av en Bidcon-estimation vid ett importtillfälle.

**ViewScopedChangeSet**:
Portalägda delade ändringar knutna till kombinationen `View`, `EstimationSnapshot` och en version i taget, utan att mutera snapshotet. Ändringarna appliceras när en `SheetSection` renderar snapshotets datablad, och varje enskild ändring pekar ut vilken `SheetSection` den gäller.
Ett `ViewScopedChangeSet` är unikt per `View`, `EstimationVersion` och `EstimationSnapshot`.
_Avoid_: SnapshotChangeSet när ändringen avser en viss view

**ReplacementGroup**:
Minst två leaf sheet-rader som är syskon på samma nivå i samma `SheetKind` och delar samma giltiga `ReplacementTag` samt kompatibel radtyp enligt Reports-domänen. Kompatibilitet kan initialt uttryckas genom samma `SheetRowKind`; om Bidcon-radtyper behöver vara mer detaljerade än replacement-kompatibilitet krävs en explicit global kompatibilitetsmodell, inte per-section konfiguration. En ensam taggad rad har inga replacement-kandidater och bildar ingen grupp i UI. En rad kan ha flera giltiga taggar och därmed ingå i flera `ReplacementGroup`. Gruppen ger möjliga kandidater men skapar inga automatiska `ReplacementSelection`. Replacement av parent-rader eller subträd är en framtida explicit policyvariation.
_Avoid_: Ersättningsgrupp

**ReplacementTag**:
Kanoniskt case-sensitive taggvärde utan klammer, extraherat från en klammer-tagg i Bidcons `Remark`-fält, som kan markera att en sheet-rad får ingå i replacement enligt berörd `SheetSection`. Whitespace innanför klammer trimmas. `Remark`-kolumnen behöver inte vara synlig för att taggen ska gälla; synlighet är presentation. En `SheetSection` definierar vilka taggar som är giltiga i just den rapportsektionen; matchande giltiga taggar mellan syskonrader gör dem till replacement-kandidater.
_Avoid_: Ersättningstagg

**Valid Replacement Tags**:
Explicit lista av `ReplacementTag` som en change-enabled `SheetSection` tillåter för skapande av nya `ReplacementSelection`. Bara taggar i listan kan göra syskonrader till replacement-kandidater; tom lista betyder att inga nya replacements är möjliga i sektionen och har i praktiken samma användareffekt som att sektionen inte är change-enabled. Listan styr inte applicering av redan sparade selections och en senare taggändring ska inte retroaktivt sluta visa historiska val.
_Avoid_: alla taggar i Remark när rapportmallen ska vara avsiktlig

**ReplacementPolicy**:
Systemdefault-regel för hur ett giltigt replacement appliceras vid rendering, inklusive hur mängd hanteras vid bytet. Initial policy visar ersättningsradens värden på originalradens plats men behåller originalradens `Quantity`; den är inte konfigurerbar per `SheetSection` i första modellen. `SheetSection` definierar vilka `ReplacementTag` som kan göra syskonrader till replacement-kandidater; Projects utför och lagrar användarens `ReplacementSelection`.
_Avoid_: Ersättningspolicy

**ReplacementSelection**:
Sparat val i ett `ViewScopedChangeSet` som pekar ut berörd `SheetSection`, originalradens och ersättningsradens `SheetRowPath`, matchande `ReplacementTag`, originalmängd vid valtillfället, använd `ReplacementPolicy`-version, stabilt användar-ID, användarens visningsnamn/e-post vid ändringen och ändringstid. Originalrad och ersättningsrad måste vara olika rader. Det sparar användarens val för en specifik view och ett specifikt snapshotläge, inte ett färdigräknat kalkylresultat eller en regel för framtida importer. En rad kan ingå i flera kandidatgrupper via flera taggar, så selection måste bära vilken tagg valet gjordes genom.
Vid rendering visas ersättningsradens värden på originalradens plats, med originalradens mängd enligt `ReplacementPolicy`, och originalraden ska kunna inspekteras för spårbarhet. Bidcon-backed displayvärden kommer från ersättningsraden om inte `ReplacementPolicy` uttryckligen håller fältet från originalraden. Replacement är en overlay på originalpositionen, inte en mutation av snapshotet och inte ett filter som tar bort ersättningsraden från dess egen plats; ersättningsraden fortsätter renderas där om den annars är renderbar.
Visibility, cellkonfiguration och format för overlayn utvärderas mot originalradens plats och `SheetRowKind`, medan displayvärden hämtas enligt `ReplacementPolicy`.
Ett `ViewScopedChangeSet` har högst ett aktivt `ReplacementSelection` per `SheetSection` och original `SheetRowPath`, även när originalraden ingår i flera `ReplacementGroup` genom flera giltiga taggar.
Lifecycle för sparade selections, inklusive borttag/ångring, ägs av Projects.
_Avoid_: Ersättningsval

**EstimationVersion**:
Namngiven sparad versionspunkt för en enskild `ProjectEstimation` som refererar ett bestämt `EstimationSnapshot` och kan vara aktivt läge för den estimationen.
_Avoid_: ProjectVersion, Projektversion

**Draft**:
Arbetsversion som ännu inte är en publicerad eller definitiv version.
_Avoid_: utkastversion

**View**:
Organisationsägd rapportmall som beskriver hur kalkyldata ska visas genom valt `PageFormat` och en ordnad samling identifierbara sektioner, initialt `DataSection` och `SheetSection`. En `View` äger rapportstrukturens invariants, inklusive vilka sektioner som ingår, att `SectionId` är unika inom vyn och sektionernas ordning. `View` muterar inte sektioners interna innehåll direkt, utan delegerar sådana förändringar till respektive sektion.
_Avoid_: mall, dokument

**SectionId**:
Stabil identitet för en sektion inom en `View`. En `SectionId` följer sektionen när den flyttas och används tillsammans med `ViewId` när andra modeller eller renderingsspårning behöver peka på samma sektion över tid.
_Avoid_: listposition när identiteten måste vara stabil

**Change-enabled View**:
Legacybegrepp för tidigare modell där `View.EnableChanges` styrde om nya delade `ReplacementSelection` fick göras i vyn. Målmodellen använder `Change-enabled SheetSection`.
_Avoid_: Ändringsbar view när ändringsrätten gäller en sheet section

**Change-enabled SheetSection**:
`SheetSection` där nya delade view-lokala `ReplacementSelection` får göras av användare som har rätt att använda vyn, när sektionen både är change-enabled och har matchande giltiga `ReplacementTag` för raderna samt Projects kan utföra valet. Nya `SheetSection` är inte change-enabled som default. Change-enabled utan giltiga taggar ger inga nya replacement-kandidater.
_Avoid_: Change-enabled View när bara en viss sheet section är ändringsbar

**Report View**:
Verksamhetsnära ord för en `View` använd som rapport.
_Avoid_: Rapportvy

**RenderingContext**:
Kombinationen av `View`, `EstimationVersion`, `EstimationSnapshot` och eventuellt `ViewScopedChangeSet` som används för att räkna fram vad en rapport visar vid rendering.
_Avoid_: Renderingskontext

**Paginated Document Renderer**:
Portalägd renderingsyta som placerar rapportsektioner, header och footer på ett fast sidformat så att den visade rapporten motsvarar utskrifts- eller PDF-resultatet.
_Avoid_: PrintableDocument när du menar förmågan, component när sidlayoutansvaret spelar roll

**Report Document Renderer**:
Adapter mellan en `View` med aktuell `RenderingContext` och `Paginated Document Renderer`. Den bygger renderbart rapportinnehåll från domänmodellen men äger inte själva sidpagineringen.
_Avoid_: Paginated Document Renderer när `View`-tolkning ingår

**Printable Block**:
Odelbar, stabil presentationsenhet som `Paginated Document Renderer` mäter och placerar på en sida. Ett `Printable Block` beskriver renderbart innehåll med parametrar och kan inte delas över sidor.
_Avoid_: komponent när sidbrytningsansvaret spelar roll

**Printable Container**:
Logisk samling av `Printable Block` och andra `Printable Container` som hör ihop i rapporten, till exempel flera rader från samma tabell. Containern kan bära layoutval som gäller efter hela innehållet utan att själv mätas eller placeras som ett odelbart block.
_Avoid_: printable block när innehållet ska kunna brytas internt

**Printable Block Identity**:
Stabil identitet för ett `Printable Block` inom ett paginerat dokument. Identiteten beskriver renderingsblocket, medan blockets källa beskriver vilken rapportsektion eller rad blocket kommer från.
_Avoid_: list index när blocket behöver spåras i fel, test eller omrendering

**Printable Block Source**:
Spårbar hänvisning från ett `Printable Block` till dess ursprung i rapporten, till exempel `SheetSection` och radidentitet.
_Avoid_: block identity när du menar rapportursprung

**Page Break Behavior**:
Layoutval på ett `Printable Block` som anger om renderingen ska fortsätta på samma sida eller bryta efter blocket. Stödda värden är `None` och `After`.
_Avoid_: page break before, DOM-attribut i blockkomponentens egen markup

**Document Header Region**:
Fast sidregion som upprepas överst på varje sida i ett paginerat dokument och reserverar höjd innan `Printable Block` placeras i sidans body-yta.
_Avoid_: header block

**Document Footer Region**:
Fast sidregion som upprepas nederst på varje sida i ett paginerat dokument och reserverar höjd innan `Printable Block` placeras i sidans body-yta.
_Avoid_: footer block

**Page Format**:
Reports-domänvärde för fast sidstorlek med namn och mått i millimeter, till exempel A3, A4 eller A5. En `View` bär valt `Page Format`, och renderingen för vidare sidformatet till `Paginated Document Renderer`.
_Avoid_: page size enum när måtten spelar roll

**Pagination Error**:
Blockerande renderingsproblem som innebär att `Paginated Document Renderer` inte kan placera innehållet korrekt på det valda sidformatet, till exempel när en `Printable Block` är högre än sidans body-yta.
_Avoid_: warning när utskrifts- eller PDF-resultatet inte kan garanteras

**Pagination Warning**:
Icke-blockerande observation från pagineringen där dokumentet fortfarande kan visas och skrivas ut korrekt.
_Avoid_: error när dokumentet fortfarande är korrekt renderbart

**DataSection**:
Identifierbar sektion i en `View` som består av en fri spreadsheet-like layout av `DataCell`, med text, formler, formatering och referenser till metadata eller namngivna värden från aktuell `RenderingContext`. Identiteten är stabil inom sin `View` även om sektionen flyttas. En `DataSection` äger sina egna interna layout- och cellinvariants.

**SheetSection**:
Identifierbar sektion i en `View` som presenterar exakt ett helt Bidcon-datablad valt genom `SheetKind`, till exempel `TenderSheet` eller `SummarySheet`, från aktuell `RenderingContext` med konfigurerad synlighet för bladets kolumner samt synlighet och formatering per `SheetRowKind` och `SheetColumnKind`. Identiteten och valt `SheetKind` är stabila inom sin `View`; `SectionId` får inte vara tomt eller bara whitespace. Om användaren vill visa ett annat datablad skapas en ny `SheetSection` med ny identitet. En `View` får innehålla flera `SheetSection` med samma `SheetKind`; de skiljs genom `SectionId` och egen konfiguration. En sektion kan rendera noll sheet-rader för ett tomt snapshot men måste ha minst en synlig kolumn. Den bevarar snapshotets radordning och hierarki, äger inte egna sheet-rader, skapar initialt inga syntetiska rader och har initialt inga innehållsfilter utöver framtida explicit `SheetRowKind`-synlighet.
Atomära lokala fält på `SheetSection` och dess value objects kan skyddas med validerande setters/backing fields. Ändringar som beror på flera fält, samlingar eller domänintentioner bör gå via intentionella metoder eller commands som kan validera hela invarianten.
Efter reconstitution ska en `SheetSection` vara normaliserad till effektivt komplett kolumnkatalog och cellmatris. Reconstitution får defaulta/materialisera saknade schema-evolution-kolumner och celler, men ska avvisa okända keys, okända kolumner och ogiltiga värden.

**Duplicated SheetSection**:
Ny `SheetSection` med ny `SectionId` och kopierade mallinställningar från en befintlig sheet section, till exempel `SheetKind`, kolumnkonfiguration, headers, page-break behavior, cellkonfiguration och ändringsförutsättningar. Befintliga `ReplacementSelection` kopieras inte.
_Avoid_: samma sektion, kopierad change history

**Archived View**:
Fryst rapportmall som inte får ändra sina `SheetSection`-invariants via normala edit-flöden, till exempel `SheetKind`, kolumnkatalog, headers, cellkonfiguration eller ändringsförutsättningar. Ändringar kräver återaktivering eller duplicering.
_Avoid_: arkiverad som bara visuell status

**SheetKind**:
Explicit val av vilket Bidcon-datablad en `SheetSection` presenterar, till exempel `TenderSheet`, `SummarySheet` eller `NetSheet`. `SheetKind` är immutable efter att sektionen har skapats och kan i nuvarande kod motsvaras av migrationsnamnet `SheetSourceType`. Varje `SheetKind` har en kuraterad defaultpresentation som nya `SheetSection` startar från och som styr default position, synlighet och layoutvikt per kolumn när nytillkomna `SheetColumnKind` materialiseras för äldre sections. UI får defaulta nya sheet sections till `TenderSheet`, men domänmodellen ska bära ett explicit `SheetKind`.
_Avoid_: Content när värdet väljer datablad, SheetSourceType i domänspråk

**SheetColumnKind**:
Explicit kolumntyp som en `SheetSection` kan konfigurera och visa. En `SheetColumnKind` bär kolumnens semantiska displaytyp och default `ValueFormat`, till exempel numeriskt format för mängd och textformat för beskrivning. En `SheetSection` har en effektivt komplett ordnad kolumnkatalog med exakt en immutable `SheetColumnConfiguration` per känd `SheetColumnKind`; varje konfiguration bär sin `SheetColumnKind`, synlighet och positiv layoutvikt, medan ordning kommer från listpositionen. Kolumnkatalogen exponeras som read-only lista; vanliga UI-mutationer sker via intentionella `SheetSection`-metoder som ändrar synlighet, bredd eller ordning under validering. Reconstitution, bulk-edit och DTO-save får ersätta hela katalogen genom en tydligt validerande bulk-väg. Persistence får sakna definitioner för nytillkomna kolumner som schema-evolution fallback; de defaultas eller materialiseras vid läsning/access så domänen uppför sig som komplett utan full produktionsdatabasmigration. Okända eller legacy-kolumner ska översättas före domänreconstitution eller avvisas; domänmodellen bär bara kända kolumner. Minst en kolumn måste vara synlig och `SheetSection` äger mutationer som kan påverka samlingsinvariants. Nuvarande C#-namn `ColumnDefinition` bör bytas till `SheetColumnConfiguration` när målmodellen implementeras.
_Avoid_: fria kolumnnamn, string keys när kolumnen är ett domänfält

**SheetCellConfiguration**:
Immutable value object som beskriver presentation för en cellposition i en `SheetSection`, till exempel dolt värde, cell-layout, textstil eller värdeformat. Rad- och kolumnidentitet ägs av `SheetSection`ens matrisnyckel `SheetRowKind` + `SheetColumnKind`, inte av cellkonfigurationen. En `SheetSection` har en effektivt komplett cellkonfigurationsmatris med exakt en `SheetCellConfiguration` per känd nyckel, inklusive default-celler. Persistence får sakna default-celler för nytillkomna row/column kinds som schema-evolution fallback; de ska då defaultas eller materialiseras när sektionen läses eller cellen används, inte kräva full produktionsdatabasmigration. Konfiguration får finnas för en dold kolumn eller framtida dold radtyp men påverkar bara rendering när både kolumnen och raden är synliga. Nuvarande C#-namn `SheetCell` bör bytas till `SheetCellConfiguration` när målmodellen implementeras.
`SheetSection` ska äga cellkonfigurationssamlingen som privat state; `GetCell` returnerar cellens immutable value object för läsning utan att mutera state och kastar vid okänd key. Eftersom modellen är effektivt komplett behövs inget `TryGetCell` i domän-API:t. Vanliga UI-mutationer sker via intentionella metoder som ersätter cellen med en ny value object-instans. Reconstitution, bulk-edit och DTO-save får ersätta eller bygga hela cellmatrisen genom en tydligt validerande bulk-väg som defaultar/materialiserar saknade schema-evolution-celler och avvisar okända keys. Persistence-migration får översätta legacy keys före reconstitution, men domänen ska inte tyst ignorera okända cellkonfigurationer. Lazy/defaulting är en bakåtkompatibilitetsmekanism för schema-evolution, inte en aktiv sparse-modell eller pruningstrategi.
_Avoid_: cellvärde när det bara är presentation, RowType/ColumnType inuti cellkonfigurationen som identitet, muterbar SheetCell som läcker ut ur SheetSection

**Sheet Format Model**:
Första formatmodellen för `SheetSection`: systemdefault, kolumnens semantiska default och effektivt komplett `SheetCellConfiguration` per `SheetRowKind` + `SheetColumnKind`. Till skillnad från `DataSection` har sheet-kolumner semantiskt default `ValueFormat`; varje sheet cell bär effektiv presentation för sin kombination. Separata formatlager per hel radtyp eller hel kolumn ingår inte i första modellen.
_Avoid_: implicit row-format eller column-format när cell override menas

**Sheet Header**:
Icke-data-rad som visar katalogens displaynamn för en `SheetSection`ens synliga kolumner i aktuell kolumnordning. Nya `SheetSection` visar headers som default; om `ShowHeaders` är av skapas inget header-block alls. Egna header-labels, header-format eller upprepade headers vid sidbrytning ingår inte i första modellen.
_Avoid_: snapshotrad, osynlig tom header

**SheetSection Label**:
Framtida separat layoutfält för en eventuell användarredigerbar titel på en `SheetSection`. Det ingår inte i första modellen och ska inte blandas ihop med `SheetKind`, `Content` eller sheet headers.
_Avoid_: Content när du menar titel, SheetKind när du menar rubrik

**Section Page Break Behavior**:
Generell section-level regel för hur en rapportsektion får brytas i paginering, gemensam för `DataSection` och `SheetSection`. Sheet-specifika regler som upprepade headers eller att hålla grupper med barn är framtida explicita variationer och ska inte vara implicit logik i pagineringsmotorn.
_Avoid_: sheet-specifik pagination som implicit del av SheetSection

**HideValue**:
Presentationsval på en `SheetCellConfiguration` som gör cellens text eller displayvärde osynligt medan cellens layout, plats, padding, borders, bakgrund, kolumnbredd och radstruktur är intakta.
_Avoid_: radfilter, kolumnfilter, borttagen cell

**Bidcon-backed SheetColumnKind**:
`SheetColumnKind` vars displayvärde hämtas från en rad i Bidcon-snapshotet.
_Avoid_: Reports-owned column när värdet kommer från snapshotraden

**Reports-owned SheetColumnKind**:
`SheetColumnKind` vars displayvärde härleds från aktuell `RenderingContext` eller från Projects change set, till exempel `ChangeIndicator` från applicerade `ReplacementSelection` eller en framtida `Comment`-kolumn. En Reports-owned kolumn i `SheetSection` beskriver presentation; radbundna användarvärden ägs av Projects change set.
_Avoid_: Bidcon-backed column när värdet inte finns på snapshotraden

**ChangeIndicator**:
Reports-owned `SheetColumnKind` som visar att en renderad sheet-rad är påverkad av en applicerad `ReplacementSelection`. Den är en vanlig kolumn i `SheetSection`ens kompletta ordnade kolumnkatalog och kan ordnas, döljas, breddas och formateras som andra kolumner, inte en separat fast radindikator. Nya change-enabled sections kan defaulta till synlig indikator, men domänen kräver inte att den alltid är synlig.
_Avoid_: specialindikator utanför sheet-kolumnerna

**Sheet Row Identity**:
Stabil identitet för en rad inom ett valt `SheetKind`, baserad på `SheetRowPath`: sekvensen av syskonindex enligt den importerade `Children`-listans ordning från databladets root till raden. Den används tillsammans med `SectionId` och `SheetKind` för renderingsspårning och `ReplacementSelection`.
_Avoid_: ExternalId eller Code när du menar sheet-radens identitet

**SheetRowPath**:
Sekvens av syskonindex från ett valt `SheetKind`'s renderbara root-barn till en rad i ett Bidcon-datablad, baserad på den importerade `Children`-listans ordning, till exempel `0/3/2`. Pathen är deterministisk inom ett immutable `EstimationSnapshot` och används som stabil sheet row identity. Initialt härleds den av en snapshot-row adapter och sparas inte fysiskt på snapshotnoden.
_Avoid_: RowOrder ensam, ExternalId eller Code när raden behöver identifieras inom ett sheet

**SheetRowKind**:
Explicit radtyp som en `SheetSection` kan rendera från ett Bidcon-datablad i målmodellen. Begreppet täcker Bidcons radtyper som kan förekomma i snapshotet och används tillsammans med `SheetColumnKind` för synlighet och format. Nuvarande kods `RowType` är en lokal formatnyckel, inte en full radtypssynlighetsmodell.
_Avoid_: Group/Part/Other när den faktiska Bidcon-radtypen spelar roll

**Sheet Row Height**:
Härledd renderingsstorlek för en sheet-rad, baserad på innehåll, textstil, padding och tillgänglig bredd. Radhöjd per `SheetRowKind` är inte `SheetSection`-state i första modellen.
_Avoid_: row-height config i SheetSection när höjden bara mäts vid rendering

**Sheet Hierarchy Display**:
Systemstandard för hur snapshotets sheet-hierarki visas, initialt genom renderingens standardlayout, till exempel indrag i description-kolumnen. `SheetSection` bevarar hierarkin men bär inte egna inställningar för indent-size, expand/collapse eller separat nivåkolumn i första modellen.
_Avoid_: egen trädmodell i SheetSection

**DataCell**:
Cell i en `DataSection` med användarens `CellExpression` samt format. Visningsvärdet sparas inte, utan beräknas vid rendering från cellens uttryck och aktuell `RenderingContext`.
_Avoid_: Data cell

**CellExpression**:
Användarens inmatade uttryck i en `DataCell`. Uttryck som börjar med `=` tolkas som formler vid rendering; annars visas uttrycket som text.
_Avoid_: RawValue i domänspråk

**CellLayout**:
Rapportlayoutvärde för en `DataCell` som beskriver bakgrund, padding och borders. En `CellLayout` uttrycker layoutintention och översätts av rendering till presentationsrepresentation.
_Avoid_: CellFormat när skillnaden mot text- och värdeformat spelar roll

**TextStyle**:
Rapportlayoutvärde för en `DataCell` som beskriver font, textfärg, bold, italic, underline och textplacering.
_Avoid_: TextFormat när värdeformat också ingår

**ValueFormat**:
Rapportlayoutvärde för en `DataCell` som beskriver hur cellens beräknade displayvärde formateras som text, tal eller datum/tid. Default är text, och tal/datum måste väljas explicit.
_Avoid_: TextFormat när du menar tal- eller datumformat

**Estimation Explorer**:
Yta där användaren söker, filtrerar och väljer estimations från Bidcon.
_Avoid_: Kalkylutforskare

**Comparison**:
Användningsfall där versioner ställs mot varandra för att visa skillnader.
_Avoid_: Jämförelse

**History**:
Spårbar följd av tidigare versioner av projekt eller kalkyllägen.
_Avoid_: Historik

**Settings Hub**:
Portalägd landningsyta för settings som samlar länkar till modulernas egna settings-sidor.
_Avoid_: settings landing page, första settings-sidan

**Settings Area**:
Modulägd settings-yta som är den enda settings-ingången en modul registrerar i portalens Settings Hub. En Settings Area har stabil identitet och egen route, och modulen äger själv eventuell undersidesnavigation inom sin area.
_Avoid_: settings section, settings component

**Feature registration**:
Kompatibilitetsväg för äldre `IFeature`-kod som registrerar tjänster, accessregler och äldre UI-bidrag utan att vara målformen för nya moduler.
_Avoid_: module registration, ny registrering

**Module registration**:
Målform för nya produktmoduler där modulen exponerar en statisk registreringsfunktion som bidrar med tjänster, UI-composition, access claims och policies till portalens `AppRegistrationContext`.
_Avoid_: feature registration, legacy registration, IModule registration
