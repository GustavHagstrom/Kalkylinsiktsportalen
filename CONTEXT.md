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
Minst två sheet-rader på samma nivå i Bidcon-trädet som delar samma giltiga `ReplacementTag` och samma `SheetRowKind` enligt berörd `SheetSection`. En rad kan ha flera giltiga taggar och därmed ingå i flera `ReplacementGroup`. Gruppen ger möjliga kandidater men skapar inga automatiska `ReplacementSelection`.
_Avoid_: Ersättningsgrupp

**ReplacementTag**:
Kanoniskt case-sensitive taggvärde utan klammer, extraherat från en klammer-tagg i Bidcons `Remark`-fält, som kan markera att en sheet-rad får ingå i replacement enligt berörd `SheetSection`. Whitespace innanför klammer trimmas. En `SheetSection` definierar vilka taggar som är giltiga i just den rapportsektionen; matchande giltiga taggar mellan syskonrader gör dem till replacement-kandidater.
_Avoid_: Ersättningstagg

**Valid Replacement Tags**:
Explicit lista av `ReplacementTag` som en change-enabled `SheetSection` tillåter för skapande av nya `ReplacementSelection`. Bara taggar i listan kan göra syskonrader till replacement-kandidater; tom lista betyder att inga replacements är möjliga i sektionen och har i praktiken samma användareffekt som att sektionen inte är change-enabled. Listan styr inte applicering av redan sparade selections.
_Avoid_: alla taggar i Remark när rapportmallen ska vara avsiktlig

**ReplacementPolicy**:
Systemdefault-regel för hur ett giltigt replacement utförs, inklusive hur mängd hanteras vid bytet. Initial policy visar ersättningsradens värden på originalradens plats men behåller originalradens `Quantity`. `SheetSection` definierar vilka `ReplacementTag` som kan göra syskonrader till replacement-kandidater; Projects utför och lagrar användarens `ReplacementSelection`.
_Avoid_: Ersättningspolicy

**ReplacementSelection**:
Sparat val i ett `ViewScopedChangeSet` som pekar ut berörd `SheetSection`, originalrad, ersättningsrad, matchande `ReplacementTag`, originalmängd vid valtillfället, använd policyversion, stabilt användar-ID, användarens visningsnamn/e-post vid ändringen och ändringstid. Det sparar användarens val för en specifik view och ett specifikt snapshotläge, inte ett färdigräknat kalkylresultat eller en regel för framtida importer. En rad kan ingå i flera kandidatgrupper via flera taggar, så selection måste bära vilken tagg valet gjordes genom.
Originalrad och ersättningsrad måste vara olika sheet-rader.
Vid rendering visas ersättningsradens värden på originalradens plats, med originalradens mängd enligt `ReplacementPolicy`, och originalraden ska kunna inspekteras för spårbarhet. Bidcon-backed displayvärden kommer från ersättningsraden om inte `ReplacementPolicy` uttryckligen håller fältet från originalraden. Replacement är en overlay på originalpositionen, inte en mutation av snapshotet och inte ett filter som tar bort ersättningsraden från dess egen plats.
Visibility och format för overlayn utvärderas mot originalradens `SheetRowKind`.
Ett `ViewScopedChangeSet` har högst ett aktivt `ReplacementSelection` per `SheetSection` och original `SheetRowPath`.
Lifecycle för sparade selections, inklusive borttag/ångring, ägs av Projects.
_Avoid_: Ersättningsval

**EstimationVersion**:
Namngiven sparad versionspunkt för en enskild `ProjectEstimation` som refererar ett bestämt `EstimationSnapshot` och kan vara aktivt läge för den estimationen.
_Avoid_: ProjectVersion, Projektversion

**Draft**:
Arbetsversion som ännu inte är en publicerad eller definitiv version.
_Avoid_: utkastversion

**View**:
Organisationsägd rapportmall som beskriver hur kalkyldata ska visas genom valt `PageFormat` och en ordnad samling identifierbara sektioner, initialt `DataSection` och `SheetSection`. En `View` äger rapportstrukturens invariants, inklusive vilka sektioner som ingår, sektionernas stabila identiteter och deras ordning. `View` muterar inte sektioners interna innehåll direkt, utan delegerar sådana förändringar till respektive sektion.
_Avoid_: mall, dokument

**SectionId**:
Stabil identitet för en sektion inom en `View`. En `SectionId` följer sektionen när den flyttas och används tillsammans med `ViewId` när andra modeller eller renderingsspårning behöver peka på samma sektion över tid.
_Avoid_: listposition när identiteten måste vara stabil

**Change-enabled View**:
Legacybegrepp för tidigare modell där `View.EnableChanges` styrde om nya delade `ReplacementSelection` fick göras i vyn. Målmodellen använder `Change-enabled SheetSection`.
_Avoid_: Ändringsbar view när ändringsrätten gäller en sheet section

**Change-enabled SheetSection**:
`SheetSection` där nya delade view-lokala `ReplacementSelection` får göras av användare som har rätt att använda vyn, när raderna matchar sektionens giltiga `ReplacementTag` och Projects kan utföra valet.
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
Identifierbar sektion i en `View` som presenterar exakt ett helt Bidcon-datablad, till exempel `TenderSheet` eller `SummarySheet`, från aktuell `RenderingContext` med konfigurerad synlighet för bladets kolumner samt synlighet och formatering per `SheetRowKind` och `SheetColumnKind`. `SheetSection` visar en standardiserad radindikator när en rad är ändrad. Identiteten är stabil inom sin `View` även om sektionen flyttas. En `SheetSection` äger sina egna interna fält-, synlighets- och formateringsinvariants.

**SheetKind**:
Explicit val av vilket Bidcon-datablad en `SheetSection` presenterar, till exempel `TenderSheet`, `SummarySheet` eller `NetSheet`.
_Avoid_: Content när värdet väljer datablad

**SheetColumnKind**:
Explicit kolumntyp som en `SheetSection` kan visa. Begreppet omfattar både Bidcon-backed kolumner och Reports-owned kolumner, bär kolumnens semantiska displaytyp och default `ValueFormat`, och används tillsammans med radtyp för synlighet och format.
_Avoid_: fria kolumnnamn, string keys när kolumnen är ett domänfält

**Bidcon-backed SheetColumnKind**:
`SheetColumnKind` vars displayvärde hämtas från en rad i Bidcon-snapshotet.
_Avoid_: Reports-owned column när värdet kommer från snapshotraden

**Reports-owned SheetColumnKind**:
`SheetColumnKind` vars displayvärde härleds av Reports från aktuell `RenderingContext`, till exempel `ChangeIndicator` från applicerade `ReplacementSelection`. En Reports-owned kolumn beskriver presentation, inte lagring av radbundna arbetsvärden.
_Avoid_: Bidcon-backed column när värdet inte finns på snapshotraden

**ChangeIndicator**:
Reports-owned `SheetColumnKind` som visar att en renderad sheet-rad är påverkad av en applicerad `ReplacementSelection`. Den är en vanlig kolumn i `SheetSection`ens ordnade kolumnlista, inte en separat fast radindikator.
_Avoid_: specialindikator utanför sheet-kolumnerna

**Sheet Row Identity**:
Stabil identitet för en rad inom ett valt `SheetKind`, baserad på `SheetRowPath`: sekvensen av syskonindex enligt den importerade `Children`-listans ordning från databladets root till raden. Den används tillsammans med `SectionId` och `SheetKind` för renderingsspårning och `ReplacementSelection`.
_Avoid_: ExternalId eller Code när du menar sheet-radens identitet

**SheetRowPath**:
Sekvens av syskonindex från ett valt `SheetKind`'s renderbara root-barn till en rad i ett Bidcon-datablad, baserad på den importerade `Children`-listans ordning, till exempel `0/3/2`. Pathen är deterministisk inom ett immutable `EstimationSnapshot` och används som stabil sheet row identity. Initialt härleds den av en snapshot-row adapter och sparas inte fysiskt på snapshotnoden.
_Avoid_: RowOrder ensam, ExternalId eller Code när raden behöver identifieras inom ett sheet

**SheetRowKind**:
Explicit radtyp som en `SheetSection` kan rendera från ett Bidcon-datablad. Begreppet täcker Bidcons radtyper som kan förekomma i snapshotet och används tillsammans med `SheetColumnKind` för synlighet och format.
_Avoid_: Group/Part/Other när den faktiska Bidcon-radtypen spelar roll

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
