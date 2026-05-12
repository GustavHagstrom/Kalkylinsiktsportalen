# Kalkylinspektionsportalen

Detta är en Blazor Server-applikation.

Applikationens mål och syfte:
- Presentera data från kalkylprogrammet Bidcon från Elecosoft i anpassningsbara rapporter.
- Kopplas visa Bidcons webb Api.
- Distribuera rapporterna utifrån rättigheter.
- Skapa både interagerbara och readonly rapporter.
- Rapporterna anpassas av användaren och kan sedan användas internt eller som exempelvis säljunderlag mot kunder.

> Detta dokument är ett första utkast baserat på en snabb repo-analys. Uppdatera gärna innehållet när domänspråk, målbild och ansvar blir tydligare.

## Projektöversikt

### `BlazorServer`
Huvudapplikationen i lösningen.

Nuvarande observationer:
- Bygger på **.NET 10** och **Blazor Server**.
- Använder **MudBlazor** för UI.
- Har en tydlig **feature-baserad struktur** där funktionalitet registreras via `IFeature`.
- Använder **MongoDB** för lagring av bland annat användarinställningar, åtkomst och vyer.
- Integrerar med ett externt **Bidcon API** via en namngiven `HttpClient`.
- Har olika auth-flöden för development respektive production:
  - development: mockad auth
  - production: Microsoft Identity / OpenID Connect

### `Tests/BlazorServer.Tests`
Testprojekt för BlazorServer.

Nuvarande observationer:
- NUnit används.
- Befintliga tester fokuserar just nu på `DataCellFormulaEvaluator`.

## Rekommenderad läsordning

- [docs/README.md](docs/README.md)
- [docs/architecture/overview.md](docs/architecture/overview.md)
- [docs/architecture/features-and-modules.md](docs/architecture/features-and-modules.md)
- [docs/development/local-development.md](docs/development/local-development.md)
- [docs/development/coding-guidelines.md](docs/development/coding-guidelines.md)
- [docs/agents/repository-context.md](docs/agents/repository-context.md)
- [docs/agents/task-template.md](docs/agents/task-template.md)
- [docs/backlog/open-questions.md](docs/backlog/open-questions.md)
- [docs/adr/README.md](docs/adr/README.md)

## Mål med dokumentationen

Syftet med dokumentationen är att ge både människor och agenter en stabil, repo-nära kontext för att kunna:
- förstå projektets mål och ansvar
- hitta rätt lager och filer snabbare
- göra mindre och mer korrekta ändringar
- hålla en konsekvent arkitektur över tid
- skriva bättre promter och arbetsbeskrivningar

## Förslag på nästa steg

1. Bekräfta projektets egentliga produktmål och målgrupp.
2. Beskriv domänbegrepp som exempelvis Bidcon, views, access claims och organisationskontext.
3. Besluta om `PwCracker` ska leva kvar i samma repo.
4. Fyll på med ADR:er när viktiga arkitekturbeslut tas.
5. Låt all ny funktionalitet uppdatera relevant dokumentation i `docs/`.
