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
Gruppering av användare som används tillsammans med portalens accessregler.
_Avoid_: Roll

**Access claim**:
Funktionellt rättighetsvärde som styr vad användare eller roller får göra. Claimens value är dess globala stabila identitet och får bara ha en betydelse i systemet.

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
Kalkylrader på samma nivå i Bidcon-trädet som delar minst ett taggvärde i `Remark`, till exempel `{minTagg}`. En rad kan ha flera klammer-taggar, och rader i samma `ReplacementGroup` får ersätta varandra enligt portalens `ReplacementPolicy`.
_Avoid_: Ersättningsgrupp

**ReplacementTag**:
Klammer-tagg i Bidcons `Remark`-fält som markerar att en kalkylrad kan ingå i en `ReplacementGroup`. Initialt räknas alla `{tagg}`-värden som `ReplacementTag` för kompatibilitet med den befintliga portalen.
_Avoid_: Ersättningstagg

**ReplacementPolicy**:
Modulär regel som avgör vilka kalkylrader som får ersätta varandra och hur mängd hanteras vid bytet. Initial policy behåller originalradens mängd och applicerar den på ersättningsraden.
_Avoid_: Ersättningspolicy

**ReplacementSelection**:
Sparat val i ett `ViewScopedChangeSet` som pekar ut berörd `SheetSection`, originalrad, ersättningsrad, matchande `ReplacementTag`, originalmängd vid valtillfället, använd policyversion, stabilt användar-ID, användarens visningsnamn/e-post vid ändringen och ändringstid. Det sparar användarens val för en specifik view och ett specifikt snapshotläge, inte ett färdigräknat kalkylresultat eller en regel för framtida importer.
Vid rendering visas ersättningsraden på originalradens plats, med originalradens mängd enligt `ReplacementPolicy`, och originalraden ska kunna inspekteras för spårbarhet.
Ett `ViewScopedChangeSet` har högst ett aktivt `ReplacementSelection` per `SheetSection` och originalrad.
_Avoid_: Ersättningsval

**EstimationVersion**:
Namngiven sparad versionspunkt för en enskild `ProjectEstimation` som refererar ett bestämt `EstimationSnapshot` och kan vara aktivt läge för den estimationen.
_Avoid_: ProjectVersion, Projektversion

**Draft**:
Arbetsversion som ännu inte är en publicerad eller definitiv version.
_Avoid_: utkastversion

**View**:
Organisationsägd rapportmall som beskriver hur kalkyldata ska visas genom identifierbara sektioner, initialt `DataSection` och `SheetSection`.
_Avoid_: mall, dokument

**Change-enabled View**:
`View` där `EnableChanges` är aktiverat och där användare med rätt att använda vyn får göra delade view-lokala `ReplacementSelection`.
_Avoid_: Ändringsbar view

**Report View**:
Verksamhetsnära ord för en `View` använd som rapport.
_Avoid_: Rapportvy

**RenderingContext**:
Kombinationen av `View`, `EstimationVersion`, `EstimationSnapshot` och eventuellt `ViewScopedChangeSet` som används för att räkna fram vad en rapport visar vid rendering.
_Avoid_: Renderingskontext

**DataSection**:
Identifierbar sektion i en `View` som består av en fri excel-liknande layout av `DataCell`, med text, formler, formatering och referenser till metadata eller namngivna värden från aktuell `RenderingContext`. Identiteten är stabil inom sin `View` även om sektionen flyttas, och sektionen kan ange sidbrytning efter sig.

**SheetSection**:
Identifierbar sektion i en `View` som presenterar exakt ett helt datablad, till exempel `TenderSheet` eller `SummarySheet`, från aktuell `RenderingContext` med konfigurerad synlighet och formatering för bladets fält. `SheetSection` visar en standardiserad radindikator när en rad är ändrad. Identiteten är stabil inom sin `View` även om sektionen flyttas, och sektionen kan ange sidbrytning efter sig.

**PageBreakAfter**:
Booleskt layoutval på en `DataSection` eller `SheetSection` som anger att rendering eller utskrift ska fortsätta på en ny sida efter sektionen.

**DataCell**:
Cell i en `DataSection` med råvärde, formel eller namngiven värdereferens samt format. Visningsvärdet sparas inte, utan beräknas vid rendering från cellens innehåll och aktuell `RenderingContext`.
_Avoid_: Data cell

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
