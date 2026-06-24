# Produktbeskrivning — arbetsversion

Det här dokumentet är en första arbetsversion av produktbeskrivningen för Kalkylinspektionsportalen.

Syftet är att formulera produkten på ett sätt som fungerar för planering, utveckling och vidare domänarbete. Dokumentet bygger på nuvarande kod, nuvarande dokumentation och den struktur som redan finns i lösningen.

Det här är alltså inte en slutlig marknads- eller verksamhetsbeskrivning, men det är tänkt att vara mer styrande än en ren teknisk tolkning.

## Kort beskrivning

Kalkylinspektionsportalen är en portal för att hämta, välja ut, strukturera och presentera kalkylinformation från Bidcon i anpassningsbara rapportvyer.

Produkten kombinerar:
- Bidcon som extern datakälla
- egna vyer som presentations- och rapportlager
- organisationsbunden konfiguration
- rättighetsstyrning för administration och användning

## Vision och målbild

Den nuvarande kärnan i produkten är att hämta kalkylinformation från Bidcon och presentera den genom anpassningsbara views.

Den långsiktiga målbilden är bredare:
- att användare ska kunna skapa **projekt** i portalen
- att ett projekt ska kunna äga **en eller flera kalkyler** från Bidcon
- att behöriga användare ska kunna hämta in **nya versioner** av projektets kalkyler över tid
- att projekt ska kunna delas med relevanta personer i organisationen
- att historik ska bevaras så att utvecklingen över tid går att följa
- att användare på sikt ska kunna jämföra versioner sida vid sida eller via rapporter som visar skillnader

Det innebär att produkten på sikt inte bara är en vy- och rapportportal, utan också ett arbetsverktyg för att **samla, versionshantera, dela och analysera kalkyler i projektform**.

## Produktens syfte

Produktens syfte är att göra kalkylinformation mer användbar i verksamheten än om den enbart konsumeras i källsystemet.

Det innebär i praktiken att produkten ska stödja:
- att rätt information kan visas för rätt målgrupp
- att samma underliggande kalkyldata kan presenteras på olika sätt
- att rapportupplägg kan återanvändas
- att åtkomst och administration kan styras i portalens egen kontext

## Problembild

Bidcon innehåller kalkyl- och estimationsdata, men användarbehovet verkar inte vara begränsat till att läsa information direkt i källsystemet.

Den domänmässiga utmaningen ser i stället ut att vara att:
- göra informationen lättare att konsumera
- presentera olika utsnitt för olika målgrupper
- kunna forma rapporter och vyer utifrån verksamhetsbehov
- behålla kontroll över vilka användare som får konfigurera och administrera lösningen

Produkten kan därför beskrivas som ett lager ovanpå kalkyldatan, där fokus ligger på **presentation, urval, struktur och styrning**.

## Kärnvärde

Produktens kärnvärde är att göra kalkylinformation från Bidcon användbar i fler sammanhang än själva källsystemet.

Det sker genom att kombinera:
- **extern kalkyldata** från Bidcon
- **anpassningsbara rapportvyer**
- **organisationsspecifik konfiguration**
- **rättighetsstyrd användning och administration**

På sikt utökas detta värde även med:
- **projekt som samlar flera relaterade kalkyler**
- **versionshistorik över kalkyler inom ett projekt**
- **möjlighet att dela projekt i rätt målgrupp**
- **jämförelser mellan olika versioner eller kalkyllägen**

Produkten är därför inte bara en integration och inte bara en visningsyta. Den fungerar som ett verksamhetsnära presentations- och styrningslager för kalkylinformation.

## Primära användare

Följande användartyper framstår som mest relevanta utifrån nuvarande lösning:

### 1. Konsument av kalkylinformation
En användare som behöver:
- söka fram estimat
- välja relevanta objekt
- konsumera rapporter eller vyer
- använda informationen som arbets-, besluts- eller säljunderlag

### 2. Administratör
En användare som behöver:
- hantera rättigheter på användare och roller
- skydda och administrera känsliga funktioner
- styra vilka delar av produkten som är tillgängliga

### 3. Rapport-/viewansvarig
En användare som behöver:
- skapa eller ändra views
- definiera hur kalkylinformation ska presenteras
- justera struktur, sektioner och formatering

## Centrala funktionella områden

### 1. Åtkomst och administration
Produkten behöver stödja att:
- rättigheter kan sättas på användare och roller
- administrativa funktioner skyddas
- olika ansvarsnivåer kan särskiljas

### 2. Integration mot Bidcon
Produkten behöver stödja att:
- en organisation kan konfigurera åtkomst till Bidcon API
- portalen kan autentisera mot Bidcon
- estimatdata kan läsas in och användas i portalens vyer

### 3. Utforskning av kalkyler
Produkten behöver stödja att användare kan:
- läsa in tillgängliga estimat
- söka och filtrera bland dem
- välja relevanta objekt för fortsatt arbete

### 4. Rapport- och viewhantering
Produkten behöver stödja att behöriga användare kan:
- skapa views
- redigera rapportstruktur
- styra sektioner och layout
- formatera och uttrycka data på ett ändamålsenligt sätt

### 5. Projekt och versionshantering
Produkten ska på sikt stödja att behöriga användare kan:
- skapa projekt som samlar en eller flera Bidcon-kalkyler
- koppla kalkyler till ett projekt
- hämta hem en ny version av projektets kalkyler från Bidcon
- skapa en ny projektversion eller draft baserat på uppdaterad kalkyldata
- se vilken version som är den senaste
- följa historik för hur projektets kalkyler har förändrats över tid
- dela projektet med relevanta personer

### 6. Jämförelse och förändringsanalys
Produkten ska på sikt stödja att användare kan:
- jämföra versioner sida vid sida
- se skillnader mellan kalkylversioner
- ta fram rapporter som beskriver vad som har förändrats mellan versioner

## Produktens plats i systemlandskapet

Kalkylinspektionsportalen är inte ursprungssystem för kalkylinformationen.

Produkten är i stället:
- ett portalgränssnitt ovanpå extern kalkyldata
- ett presentationslager för estimationsinformation
- ett anpassningslager där rapportvyer definieras
- ett behörighetsstyrt arbetsverktyg i organisationskontext

Detta är viktigt, eftersom produktens ansvar inte primärt är att skapa kalkyldata, utan att **göra den användbar, styrbar och presenterbar**.

## Avgränsning

Utifrån nuvarande lösning bör produkten i första hand förstås som:
- en portal för rapportering och presentation
- ett verktyg för view-/mallhantering
- ett gränssnitt för rättighetsstyrd åtkomst till kalkylinformation

Produkten bör i nuläget inte beskrivas som:
- ett komplett kalkylsystem
- ett ersättningssystem för Bidcon
- en generell BI-plattform för alla typer av data

Samtidigt bör målbilden förstås som större än en ren visningsportal, eftersom produkten på sikt också ska kunna bära projekt, versionshistorik och jämförelser.

## Arbetsdefinition av MVP

En rimlig arbetsdefinition av MVP är att en organisation kan:
1. konfigurera åtkomst till Bidcon
2. logga in i portalen
3. läsa in, hitta och välja estimat
4. använda minst en view för att presentera relevant kalkylinformation
5. styra vilka användare som får administrera centrala funktioner

Denna MVP-definition fokuserar alltså på att bevisa kärnan i produkten:
- data kommer in från rätt källa
- rätt användare kommer åt rätt funktioner
- informationen kan presenteras i en domänanpassad vy

Det är samtidigt viktigt att se MVP:n som ett första steg mot den större målbilden där kalkyler inte bara presenteras, utan också organiseras i projekt, versionshanteras och jämförs över tid.

## Långsiktig målbild bortom MVP

Efter MVP bör produkten kunna utvecklas mot ett flöde där:
1. en användare skapar ett projekt
2. projektet kopplas till en eller flera kalkyler från Bidcon
3. behöriga användare kan hämta ny version eller skapa ny draft-version
4. tidigare versioner bevaras i historiken
5. projektet delas med relevanta användare
6. skillnader mellan versioner kan analyseras i vyer eller rapporter

## Antaganden som bör bekräftas

Följande antaganden ligger bakom den här arbetsversionen och bör bekräftas uttryckligt:
- att views är ett kärnbegrepp i produkten och inte bara en teknisk detalj
- att organisation är den huvudsakliga avgränsningsnivån
- att rättighetsstyrning är en central del av produkten, inte bara av infrastrukturen
- att rapporterna ska kunna användas både internt och i mer externa sammanhang
- att projekt blir ett framtida kärnbegrepp i produkten
- att versionshantering av kalkyler inom projekt är en central del av målbilden

## Punkter som behöver förtydligas vidare

För att göra detta till en mer officiell produktbeskrivning behöver följande förtydligas:
- exakt målgrupp och huvudpersona
- huvudsakligt användningsscenario
- skillnaden mellan view, rapport, mall och dokument
- skillnaden mellan projekt, kalkyl, version och draft
- hur kundnära rapportanvändningen faktiskt är
- vilka funktioner som hör till MVP respektive senare versioner

## Nästa steg

1. Bekräfta vilka användartyper som faktiskt finns
2. Definiera officiellt språk för view, rapport, mall och estimat
3. Beskriv ett eller två centrala användarflöden från början till slut
4. Bekräfta arbetsdefinitionen av MVP
5. Uppdatera dokumentet från arbetsversion till officiell produktbeskrivning
