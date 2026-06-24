# Produkt och domän

Det här dokumentet samlar produktbeskrivning, domänbild och centrala begrepp för Kalkylinsiktsportalen.

## Kort Beskrivning

Kalkylinsiktsportalen är en portal för att hämta, välja ut, strukturera och presentera kalkylinformation från Bidcon i anpassningsbara rapportvyer.

Produkten kombinerar:
- Bidcon som extern datakälla
- egna views som presentations- och rapportlager
- organisationsbunden konfiguration
- rättighetsstyrning för administration och användning

Produkten är inte ett nytt kalkylsystem och ska inte ersätta Bidcon. Den är ett verksamhetsnära presentations-, styrnings- och analyslager ovanpå kalkyldata.

## Syfte Och Värde

Produktens syfte är att göra kalkylinformation mer användbar än om den enbart konsumeras i källsystemet.

Det innebär att portalen ska stödja:
- att rätt information visas för rätt målgrupp
- att samma underliggande kalkyldata kan presenteras på olika sätt
- att rapportupplägg kan återanvändas
- att åtkomst och administration styrs i portalens egen kontext

Kärnvärdet är kombinationen av extern kalkyldata, anpassningsbara rapportvyer, organisationsspecifik konfiguration och rättighetsstyrd användning.

## Målbild

Den nuvarande kärnan är att hämta kalkylinformation från Bidcon och presentera den genom anpassningsbara views.

Den långsiktiga målbilden är att användare också ska kunna:
- skapa projekt i portalen
- koppla ett projekt till en eller flera kalkyler från Bidcon
- hämta nya versioner över tid
- skapa immutable snapshots av kalkyllägen
- dela projekt med relevanta personer i organisationen
- följa historik och jämföra versioner
- använda rapporter för att visa både nuläge och förändring

Det gör produkten till mer än en rapportportal på sikt: den ska kunna samla, versionshantera, dela och analysera kalkyler i projektform.

## Primära Användare

**Konsument av kalkylinformation** söker fram estimat, väljer relevanta objekt och använder rapporter som arbets-, besluts- eller säljunderlag.

**Administratör** hanterar rättigheter, roller, känsliga funktioner och organisationens konfiguration.

**Rapport-/viewansvarig** skapar och ändrar views, rapportstruktur, sektioner och formatering.

## Funktionella Områden

**Åtkomst och administration**
- rättigheter på användare och roller
- skyddade administrativa funktioner
- separata ansvarsnivåer för användning, redigering och administration

**Integration mot Bidcon**
- organisationsbunden konfiguration av Bidcon API-åtkomst
- autentisering mot Bidcon
- inläsning av estimatdata

**Kalkylutforskare**
- läsa in tillgängliga estimat
- söka och filtrera
- välja relevanta objekt för fortsatt arbete

**Rapporter och views**
- skapa och redigera views
- styra sektioner, layout, format och formler
- rendera rapporter från kalkyldata

**Projekt och versioner**
- samla en eller flera Bidcon-kalkyler i projekt
- skapa snapshots och versioner
- följa historik och jämföra lägen

## MVP

En rimlig MVP är att en organisation kan:

1. konfigurera åtkomst till Bidcon
2. logga in i portalen
3. läsa in, hitta och välja estimat
4. använda minst en view för att presentera relevant kalkylinformation
5. styra vilka användare som får administrera centrala funktioner

MVP:n bevisar att data kommer in från rätt källa, att rätt användare kommer åt rätt funktioner och att informationen kan presenteras i en domänanpassad vy.

## Domänmodell

```text
Organization
├─ Users, roles and access
├─ Bidcon API credentials
├─ Views
└─ Projects

Bidcon
└─ Estimations

Project
├─ ProjectEstimations
├─ EstimationSnapshots
├─ ChangeSets
└─ Versions/history

View
├─ DataSections
├─ SheetSections
└─ PageBreakSections

DataSection
└─ DataCells
   ├─ raw values
   ├─ formulas
   ├─ estimation references
   └─ formatting
```

Domänens huvudkedja:

1. en organisation konfigurerar åtkomst till Bidcon
2. användaren autentiseras och får portalrättigheter
3. estimat hämtas eller väljs från Bidcon
4. estimat kan på sikt kopplas till projekt
5. snapshots sparas som oföränderliga kalkyllägen
6. en view beskriver hur informationen visas
7. rapporten renderas från `Snapshot + ChangeSet + View`

## Viktiga Gränser

**Extern kalkyldata vs intern presentationsmodell**  
Bidcon äger kalkyldatan. Portalen äger hur den presenteras.

**Extern kalkyldata vs internt projektlager**  
Projekt, versioner, snapshots och historik är portalens egna arbetsobjekt ovanpå Bidcon-data.

**Identitet vs portalbehörighet**  
Identity/Graph beskriver vem användaren är. Portalens accessmodell beskriver vad användaren får göra.

**Organisation vs global nivå**  
Organisationen är tenant-gränsen för projekt, views, access, credentials och rapportkonfiguration.

## Begrepp

| Begrepp | Arbetsdefinition |
|---|---|
| Bidcon | Externt system som portalen hämtar kalkyl- och estimationsdata från. |
| Bidcon API | API för autentisering och läsning av Bidcon-data. |
| Organisation | Tenant-/avgränsningsnivå för data, rättigheter, views och credentials. |
| Användare | Identifierad person, typiskt via Microsoft Identity / Graph. |
| Roll | Gruppering av användare som används tillsammans med portalens accessregler. |
| Access claim | Funktionellt rättighetsvärde som styr vad användare eller roller får göra. |
| UserAccess | Direkta rättigheter för en användare inom en organisation. |
| RoleAccess | Rättigheter för en roll inom en organisation. |
| Bidcon API credentials | Organisationsbundna uppgifter för anslutning till Bidcon API. |
| Estimation / kalkyl / estimat | Centralt affärsobjekt från Bidcon med kalkylinformation och metadata. |
| Projekt | Portalägt arbetsobjekt som samlar en eller flera Bidcon-kalkyler. |
| ProjectEstimation | Koppling mellan ett projekt och en Bidcon-estimation-identitet. |
| EstimationSnapshot | Oföränderligt sparat kalkylläge från Bidcon. |
| SnapshotChangeSet | Ändringar eller kommentarer ovanpå ett snapshot, utan att mutera snapshotet. |
| Projektversion | Sparat läge för projektets kalkyler vid en viss tidpunkt. |
| Draft / utkastversion | Arbetsversion som ännu inte är en publicerad eller definitiv version. |
| View | Organisationsägd rapportmall som beskriver hur kalkyldata ska visas. |
| Rapportvy | Verksamhetsnära ord för en `View` använd som rapport. |
| DataSection | Sektion där data visas i cell- eller tabellnära format. |
| SheetSection | Sektion som representerar ett blad eller särskild dokumentdel. |
| PageBreakSection | Sektion som representerar sidbrytning eller utskriftsuppdelning. |
| Data cell | En cell med råvärde, formel, estimation-referens eller format. |
| Rapportanpassning | Förmågan att strukturera och formatera kalkylinformation för olika syften. |
| Kalkylutforskare | Yta där användaren söker, filtrerar och väljer estimat från Bidcon. |
| Jämförelse | Användningsfall där versioner ställs mot varandra för att visa skillnader. |
| Historik | Spårbar följd av tidigare versioner av projekt eller kalkyllägen. |

## Antaganden Att Bekräfta

- Views är ett kärnbegrepp i produkten, inte bara en teknisk detalj.
- Organisation är huvudsaklig avgränsningsnivå.
- Rättighetsstyrning är en central produktförmåga.
- Projekt blir ett framtida kärnbegrepp.
- Versionshantering av kalkyler inom projekt är central i målbilden.
- Skillnaden mellan view, rapport, mall och dokument behöver beslutas tydligare.
- Skillnaden mellan projekt, kalkyl, snapshot, version och draft behöver förankras i användarspråket.

