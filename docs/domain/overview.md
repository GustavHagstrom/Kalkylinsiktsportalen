# Domänöversikt

Det här dokumentet beskriver domänen på övergripande nivå.

Målet med filen är att ge en snabb helhetsbild, inte att upprepa alla definitioner eller alla produktformuleringar.

Fördjupning finns även i:
- [glossary.md](glossary.md)
- [domain-map.md](domain-map.md)
- [product-description-draft.md](product-description-draft.md)

## Domänen i korthet

Kalkylinspektionsportalen handlar om att göra kalkylinformation från **Bidcon** användbar i en egen portal, där informationen kan:
- hämtas från extern källa
- presenteras i anpassade vyer och rapporter
- styras med rättigheter och organisationskontext
- användas i både interna och mer externa sammanhang

## Domänens kärna i en mening

> **att omvandla kalkylinformation från Bidcon till styrda, anpassningsbara och återanvändbara rapportvyer för olika användare och organisationer.**

## Domänens fyra huvuddelar

### 1. Extern kalkyldata
Bidcon är källsystemet för estimat och relaterad metadata.

### 2. Intern presentationsmodell
Portalen har egna views och sektioner som styr hur kalkylinformationen ska visas.

### 3. Organisationskontext
Många centrala objekt verkar tillhöra en organisation. Det gäller bland annat views, rättigheter och externa credentials.

### 4. Behörighetsstyrning
Portalen skiljer mellan identitet, roller och funktionella rättigheter i själva produkten.

## Viktigaste begreppen

- **Estimation** är det centrala informationsobjektet från Bidcon.
- **View** är portalens egen modell för hur information ska presenteras.
- **DataSection**, **SheetSection** och **PageBreakSection** beskriver hur en view struktureras.
- **UserAccess**, **RoleAccess** och **AccessClaim** beskriver vem som får använda vilka funktioner.
- **Bidcon API credentials** gör det möjligt för en organisation att läsa sin kalkyldata.

För definitioner av enskilda begrepp, se [glossary.md](glossary.md).

## Domänens huvudsakliga flöde

På hög nivå verkar domänen fungera så här:

1. en organisation konfigurerar åtkomst till Bidcon
2. användaren autentiseras och får rättigheter i portalens kontext
3. estimat hämtas eller väljs från Bidcon
4. en view används för att strukturera hur information ska visas
5. rapporten visas utifrån sektioner, celler och formattering

För relationer mellan objekten, se [domain-map.md](domain-map.md).

## Domänens värdeerbjudande

Det värde som domänen försöker skapa verkar ligga i kombinationen av:
- kalkylinformation
- rapportering och presentation
- behörighetsstyrning
- organisationsspecifik konfiguration

Produkten skapar alltså värde genom att göra Bidcon-data enklare att konsumera och enklare att anpassa för olika målgrupper och användningsfall.

## Vad som fortfarande behöver förtydligas

Det finns fortfarande flera frågor som inte fullt kan besvaras från koden ensam:
- Är views avsedda som generella mallar, sparade rapporter eller båda?
- Hur ser relationen ut mellan ett view och ett eller flera estimat i faktisk användning?
- Ska rapporter delas mellan användare inom samma organisation?
- Finns versionshantering av views eller mallar?
- Vilka delar av Bidcon-domänen är viktigast på sikt: kalkylöversikt, rapportering, export eller beslutsstöd?
- Hur långt ska rättighetsmodellen dras: funktionsnivå, rapportnivå, organisationsnivå eller objekt-nivå?
