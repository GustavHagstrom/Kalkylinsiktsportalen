# Produkt och domän

Det här dokumentet samlar produktbeskrivning, övergripande domänbild och centrala huvudflöden för Kalkylinsiktsportalen.

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

## Övergripande Domänkarta

Den detaljerade levande domänmodellen finns i [domain/README.md](domain/README.md) och de modulvisa dokumenten under `docs/domain/`.

På global nivå består domänbilden av följande huvuddelar:

- `Organization` som tenantgräns för data, views, access och credentials
- `Bidcon` som extern källa för estimationsdata
- `Projects` som portalens arbetsmodell för projekt, versioner, snapshots och delade ändringar
- `Reports` som portalens modell för views, rapportstruktur, rendering och export
- `Access` som portalens behörighetsmodell

Domänens huvudkedja:

1. en organisation konfigurerar åtkomst till Bidcon
2. användaren autentiseras och får portalrättigheter
3. estimat hämtas eller väljs från Bidcon
4. estimat kan på sikt kopplas till projekt
5. snapshots sparas som oföränderliga kompletta importer av kalkyllägen
6. projektversioner sparar vilka snapshots som ingår i ett projektläge
7. en view beskriver hur informationen visas
8. rapporten renderas från aktiv state som Projects bygger och Reports presenterar

Fördjupning:

- [domain/projects.md](domain/projects.md)
- [domain/reports.md](domain/reports.md)

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

Den kanoniska ordlistan finns i [../CONTEXT.md](../CONTEXT.md). Uppdatera `CONTEXT.md` när ett domänbegrepp byter namn, får en skarpare definition eller när ett nytt begrepp blir etablerat.

Aggregate-struktur, relationer, invariants och modulnära policyer ska dokumenteras i `docs/domain/*`, inte här.

## Antaganden Att Bekräfta

- Views är ett kärnbegrepp i produkten, inte bara en teknisk detalj.
- Organisation är huvudsaklig avgränsningsnivå.
- Rättighetsstyrning är en central produktförmåga.
- Projekt blir ett framtida kärnbegrepp.
- Versionshantering av kalkyler inom projekt är central i målbilden.
- Skillnaden mellan view, rapport, mall och dokument behöver beslutas tydligare.
- Skillnaden mellan projekt, kalkyl, snapshot, version och draft behöver förankras i användarspråket.
