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

**Organisation**:
Tenant- och avgränsningsnivå för data, rättigheter, views och credentials.

**Användare**:
Identifierad person i portalen, typiskt via Microsoft Identity eller Graph.

**Roll**:
Gruppering av användare som används tillsammans med portalens accessregler.

**Access claim**:
Funktionellt rättighetsvärde som styr vad användare eller roller får göra.

**Estimation**:
Centralt affärsobjekt från Bidcon med kalkylinformation och metadata.
_Avoid_: kalkyl, estimat

**Projekt**:
Portalägt arbetsobjekt som samlar en eller flera Bidcon-estimations.

**ProjectEstimation**:
Koppling mellan ett projekt och en Bidcon-estimation-identitet.

**EstimationSnapshot**:
Oföränderlig komplett import av en Bidcon-estimation vid ett importtillfälle.

**ViewScopedChangeSet**:
Portalägda delade ändringar knutna till kombinationen `View`, `EstimationSnapshot` och en version i taget, utan att mutera snapshotet. Viktigaste ändringstypen är styrda byten av kalkylposter där Bidcon-data markerar vad som får bytas.
Ett `ViewScopedChangeSet` är unikt per `View`, `Projektversion` och `EstimationSnapshot`.
_Avoid_: SnapshotChangeSet när ändringen avser en viss view

**Ersättningsgrupp**:
Kalkylrader på samma nivå i Bidcon-trädet som delar minst ett taggvärde i `Remark`, till exempel `{minTagg}`. En rad kan ha flera klammer-taggar, och rader i samma ersättningsgrupp får ersätta varandra enligt portalens ersättningspolicy.

**Ersättningstagg**:
Klammer-tagg i Bidcons `Remark`-fält som markerar att en kalkylrad kan ingå i en ersättningsgrupp. Initialt räknas alla `{tagg}`-värden som ersättningstaggar för kompatibilitet med den befintliga portalen.

**Ersättningspolicy**:
Modulär regel som avgör vilka kalkylrader som får ersätta varandra och hur mängd hanteras vid bytet. Initial policy behåller originalradens mängd och applicerar den på ersättningsraden.

**Ersättningsval**:
Sparat val i ett `ViewScopedChangeSet` som pekar ut originalrad, ersättningsrad, matchande ersättningstagg, originalmängd vid valtillfället, använd policyversion, stabilt användar-ID, användarens visningsnamn/e-post vid ändringen och ändringstid. Det sparar användarens val för en specifik view och ett specifikt snapshotläge, inte ett färdigräknat kalkylresultat eller en regel för framtida importer.
Ett `ViewScopedChangeSet` har högst ett aktivt ersättningsval per originalrad.

**Projektversion**:
Namngivet sparat projektläge som refererar ett bestämt urval av `EstimationSnapshot`, typiskt ett snapshot per `ProjectEstimation` som ingår i versionen.

**Draft**:
Arbetsversion som ännu inte är en publicerad eller definitiv version.
_Avoid_: utkastversion

**View**:
Organisationsägd rapportmall som beskriver hur kalkyldata ska visas.
_Avoid_: mall, dokument

**Ändringsbar view**:
`View` där `EnableChanges` är aktiverat och där användare med rätt att använda vyn får göra delade view-lokala ersättningsval.

**Rapportvy**:
Verksamhetsnära ord för en `View` använd som rapport.

**DataSection**:
Sektion där data visas i cell- eller tabellnära format.

**SheetSection**:
Sektion som representerar ett blad eller särskild dokumentdel.

**PageBreakSection**:
Sektion som representerar sidbrytning eller utskriftsuppdelning.

**Data cell**:
Cell med råvärde, formel, estimation-referens eller format.

**Kalkylutforskare**:
Yta där användaren söker, filtrerar och väljer estimations från Bidcon.

**Jämförelse**:
Användningsfall där versioner ställs mot varandra för att visa skillnader.

**Historik**:
Spårbar följd av tidigare versioner av projekt eller kalkyllägen.
