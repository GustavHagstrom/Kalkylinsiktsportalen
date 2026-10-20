# Kalkylinsiktsportalen

En webbaserad applikation under utveckling för att omvandla kalkylinformation från Bidcon till anpassningsbara rapporter och kundunderlag.

Projektet utvecklas i **C# med .NET 10 och Blazor Server**, med MongoDB för lagring och integration mot Bidcons API.

## Bakgrund och verksamhetsnytta

Jag arbetar som kalkylansvarig och driver och utvecklar projektet själv utifrån behov jag möter i verksamheten.

Tidigare gjorde vi kalkylen i Bidcon och dokumenterade sedan ändringarna separat i en checklista eller leveransdeklaration till kunden. Jag byggde en applikation som skapar kundunderlaget från kalkylinformationen. Den har tagit bort dubbelarbete och sparar många timmar varje vecka.

Den tidigare lösningen byggdes i WPF med Bidcons SDK, fasta rapporter, Cosmos DB och PDF-export, kompletterat med ett Blazor Server-gränssnitt för vissa redigeringar.

När Bidcon rör sig mot API-baserad integration bygger jag en ny webbaserad lösning med större möjligheter att anpassa och återanvända rapporter.

## Status och målbild

**Projektet är under aktiv utveckling och har ännu inga färdiga användarflöden.** Appen går att köra lokalt med mockad inloggning för att utforska det som finns. Fokus ligger just nu på rapporternas backendmodell och gränssnittet för rapportredigering.

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

## Provkör lokalt

Du behöver .NET SDK 10 och en lokal MongoDB-instans. Ingen Entra ID-konfiguration behövs när du använder mockad inloggning.

Kör kommandona från repots rot. Om du har Docker kan du starta MongoDB med repots befintliga Compose-konfiguration:

```powershell
docker compose -f .devcontainer/docker-compose.yml up -d mongodb
```

Har du redan MongoDB på `localhost:27017` kan du hoppa över det steget. Appen använder som standard databasen `BidconCompanion`.

Lita på det lokala HTTPS-utvecklingscertifikatet och starta appen med profilen för mockad inloggning:

```powershell
dotnet dev-certs https --trust
dotnet run --project src/BlazorServer --launch-profile https_MockedAuth
```

Öppna [https://localhost:7184](https://localhost:7184), klicka på **Logga in** och välj **Admin** för att utforska appen med administratörsbehörighet. Det finns även mockanvändarna **User** och **Guest**.

Mockningen gäller inloggningen. MongoDB används fortfarande för lagring, och hämtning av verkliga kalkyler kräver egna Bidcon API-uppgifter. En ny lokal databas innehåller inga förberedda kalkyler eller rapportvyer. Provkörningen visar pågående funktionalitet, inte en färdig produktdemo.

För fler konfigurationsalternativ och testkommandon, se [lokal utveckling](docs/development.md).

## Hitta i repot

- `src/BlazorServer` — applikationen
- `src/Tests/BlazorServer.Tests` — tester
- [Lokal utveckling](docs/development.md) — förutsättningar, konfiguration och kommandon
- [Produkt och domän](docs/product-and-domain.md) — verksamhetsbehov och målbild
- [Dokumentationsindex](docs/README.md) — samlad fördjupning

## Läsguide

### För rekryterare och tekniska intervjuare

1. Läs bakgrunden och projektstatusen ovan. [Produkt och domän](docs/product-and-domain.md) ger mer sammanhang om verksamhetsbehov och målbild.
2. Läs inledningen och arkitekturprinciperna i [målarkitekturen](docs/architecture/target-architecture.md) för att förstå modulindelning, ansvar och beroenderiktning.
3. Välj ett exempel att följa från dokumentation till kod och tester:

| Område | Dokumentation | Kod och tester |
| --- | --- | --- |
| Rapportmodell | [Reports](docs/domain/reports.md) | [View](src/BlazorServer/Modules/Reports/Domain/Views/View.cs) och [ViewTests](src/Tests/BlazorServer.Tests/Modules/Reports/ViewTests.cs) |
| Modulgränser | [Målarkitektur](docs/architecture/target-architecture.md) | [FinalArchitectureBoundaryTests](src/Tests/BlazorServer.Tests/Architecture/FinalArchitectureBoundaryTests.cs) |
| Kalkylversioner | [Projects](docs/domain/projects.md) | [ProjectEstimation](src/BlazorServer/Modules/Projects/Domain/ProjectEstimation.cs) och [ProjectEstimationTests](src/Tests/BlazorServer.Tests/Modules/Projects/ProjectEstimationTests.cs) |

### För utvecklare

1. Följ instruktionerna under **Provkör lokalt** ovan och läs [utvecklingsguiden](docs/development.md) för konfiguration, testkommandon och kodriktlinjer.
2. Läs [produkt och domän](docs/product-and-domain.md), slå upp relevanta begrepp i [ordlistan](CONTEXT.md) och välj berörd modul via [domänindexet](docs/domain/README.md).
3. Läs relevanta delar av [målarkitekturen](docs/architecture/target-architecture.md) och berörda [arkitekturbeslut](docs/adr/README.md) innan du ändrar ansvar eller modulgränser.
4. Följ användningsfallet i applikationen och dess tester. Vid UI-arbete, läs även [UI/UX-principerna](docs/ui-ux.md).

### Så tolkar du dokumentationen

- **Implementation:** koden och testerna visar vad som är implementerat. En testad domänmodell innebär inte att ett komplett användarflöde är färdigt.
- **Produkt, domänmodell och målarkitektur:** beskriver överenskomna begrepp, regler och avsedd riktning. De är inte en lista över färdiga funktioner.
- **Arkitekturbeslut (ADR):** förklarar val och avvägningar. Kontrollera eventuell status och senare beslut; ett beslut innebär inte att implementationen är färdig.
- **Planering och idéer:** innehåller öppna frågor och möjliga framtida spår. Idéer är inte utlovad funktionalitet.

Använd ordlistan som uppslagsverk. Du behöver inte läsa hela dokumentationen för att komma igång.
