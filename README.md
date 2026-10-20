# Kalkylinsiktsportalen

En webbaserad applikation under utveckling för att omvandla kalkylinformation från Bidcon till anpassningsbara rapporter och kundunderlag.

Projektet utvecklas i **C# med .NET 10 och Blazor Server**, med MongoDB för lagring och integration mot Bidcons API.

## Bakgrund och verksamhetsnytta

Jag arbetar som kalkylansvarig och driver och utvecklar projektet själv utifrån behov jag möter i verksamheten.

Tidigare gjorde vi kalkylen i Bidcon och dokumenterade sedan ändringarna separat i en checklista eller leveransdeklaration till kunden. Jag byggde en applikation som skapar kundunderlaget från kalkylinformationen. Den har tagit bort dubbelarbete och sparar många timmar varje vecka.

Den tidigare lösningen byggdes i WPF med Bidcons SDK, fasta rapporter, Cosmos DB och PDF-export, kompletterat med ett Blazor Server-gränssnitt för vissa redigeringar.

När Bidcon rör sig mot API-baserad integration bygger jag en ny webbaserad lösning med större möjligheter att anpassa och återanvända rapporter.

## Status och målbild

**Projektet är under aktiv utveckling och har ännu inga färdiga användarflöden.** Fokus ligger just nu på rapporternas backendmodell och gränssnittet för rapportredigering.

Målet är att kunna:

- skapa anpassningsbara rapportvyer i stället för fasta, hårdkodade rapporter
- återanvända kalkylinformation för exempelvis säljunderlag och leveransdeklarationer
- arbeta med projekt, kalkylsnapshots och jämförelser mellan versioner
- använda lösningen på webben med Entra ID-inloggning och organisationsbunden åtkomst

Pushnotiser ingår också i den planerade vidareutvecklingen.

## Tekniskt fokus

Projektet visar hur jag omsätter verksamhetsbehov till domänmodell, modulgränser och testbar logik.

Några områden att utforska:

- **Rapportmodell och rendering:** struktur, sektioner, formatering och formler för anpassningsbara rapporter.
- **Modulär monolit:** tydliga ansvar för Bidcon-integration, projekt, rapporter och åtkomst, med avsiktliga publika kontrakt.
- **Tester:** domän- och användningsfallstester samt arkitekturtester som kontrollerar beroenden och modulgränser.

Arkitekturriktningen och avvägningarna finns i [målarkitekturen](docs/architecture/target-architecture.md) och [arkitekturbesluten](docs/adr/README.md).

## Hitta i repot

- `src/BlazorServer` — applikationen
- `src/Tests/BlazorServer.Tests` — tester
- [Lokal utveckling](docs/development.md) — förutsättningar, konfiguration och kommandon
- [Produkt och domän](docs/product-and-domain.md) — verksamhetsbehov och målbild
- [Dokumentationsindex](docs/README.md) — samlad fördjupning
