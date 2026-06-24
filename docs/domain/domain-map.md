# Domänkarta

Det här dokumentet visar en förenklad karta över de viktigaste domänobjekten och deras relationer.

Det är en konceptuell beskrivning, inte ett exakt tekniskt klassdiagram.

## Översikt

```text
Organisation
├─ användare
├─ roller
├─ user access
├─ role access
├─ Bidcon API credentials
├─ projekt
├─ views
└─ åtkomst till Bidcon-data

Bidcon
└─ estimat / kalkyler

Projekt
├─ en eller flera kalkyler
├─ versioner
├─ delning
├─ historik
└─ jämförelser

View
├─ data sections
├─ sheet sections
└─ page break sections

DataSection
└─ data cells
    ├─ råvärden
    ├─ formler
    ├─ estimation-referenser
    └─ formattering
```

## Relationer i ord

### Organisation → View
En organisation verkar äga sina egna views.

Det innebär sannolikt att:
- views skapas inom en organisationskontext
- views inte är globala per default
- olika organisationer kan ha olika rapportbehov och mallar

### Organisation → Bidcon API credentials
Varje organisation verkar ha egna credentials för att ansluta till Bidcon API.

Det betyder att integrationen mot Bidcon är organisationsbunden.

### Organisation → Projekt
Projekt verkar i målbilden vara organisationsbundna arbetsobjekt.

Det innebär sannolikt att:
- projekt skapas inom en organisationskontext
- delning och åtkomst styrs inom samma kontext
- historik och versioner tillhör organisationens arbetsyta

### Organisation → UserAccess / RoleAccess
Rättigheter bedöms inom en organisationskontext.

Det innebär att systemet inte bara har globala användare, utan även organisationsspecifika rättigheter för funktioner i portalen.

### Bidcon → Estimation
Bidcon är källsystemet för estimat/kalkyler.

Portalen hämtar och presenterar dessa objekt, men verkar inte vara ursprungssystemet för själva kalkylinformationen.

### Projekt → Estimation
Ett projekt är tänkt att kunna samla en eller flera kalkyler från Bidcon.

Det gör projektet till ett arbetsobjekt ovanpå de enskilda kalkylerna, snarare än en ersättning för dem.

### Projekt → Versioner
Ett projekt är tänkt att ha en historik av versioner över tid.

Det innebär sannolikt att en användare kan:
- hämta ny version av projektets kalkyler
- skapa ny draft eller ny aktuell version
- följa tidigare lägen

### Projekt → Delning och jämförelse
Projekt verkar i målbilden också vara den naturliga platsen för:
- delning med relevanta personer
- jämförelser mellan versioner
- rapporter över förändringar

### View → Sectioner
En view verkar vara sammansatt av flera sektionstyper.

Det tyder på att rapporter byggs modulärt och kan innehålla:
- datasektioner
- bladsektioner
- sidbrytningar

### DataSection → Data cells
Datasektioner verkar bestå av celler som definierar både innehåll och presentation.

Detta tyder på att rapportmotorn i systemet är relativt flexibel.

### Data cell → Estimation
En datacell verkar kunna referera till ett estimat och dess egenskaper.

Det gör att samma rapportvy kan återanvändas för olika estimat, så länge cellerna vet hur värden ska hämtas och presenteras.

## Trolig huvudkedja i domänen

En förenklad huvudkedja verkar vara:

1. en organisation konfigurerar åtkomst till Bidcon
2. användaren autentiseras och får rättigheter i portalens kontext
3. användaren hämtar eller väljer estimat från Bidcon
4. användaren kan på sikt samla kalkyler i ett projekt
5. en view används för att strukturera hur informationen ska visas
6. rapporten presenteras utifrån sektioner, celler och formattering
7. projektets historik och versioner kan följas och jämföras

## Objekt som verkar komma utifrån respektive inifrån systemet

### Externa / importerade domänobjekt
- Bidcon-estimat
- viss användar- och rollinformation från identity/graph

### Interna / portalägda domänobjekt
- projekt
- projektversioner
- views
- sektioner
- cellkonfiguration
- access claims
- user access
- role access
- organisationsspecifika Bidcon-credentials

## Viktiga domängränser

### 1. Extern kalkyldata vs intern presentationsmodell
Systemet skiljer mellan:
- data som kommer från Bidcon
- modellen för hur data ska visas i portalen

Detta är sannolikt en av domänens viktigaste gränser.

### 1b. Extern kalkyldata vs internt projektlager
Systemet verkar också vara på väg att skilja mellan:
- enskilda kalkyler från Bidcon
- portalens egna projekt, versioner och historik

Detta är en viktig framtida domängräns.

### 2. Identitet vs portalbehörighet
Systemet skiljer också mellan:
- vem användaren är i identitetsplattformen
- vad användaren får göra i portalen

### 3. Organisation vs global nivå
Många centrala objekt verkar vara knutna till organisation snarare än hela systemet.

Det antyder att organisationsgränsen är central i domänmodellen.
