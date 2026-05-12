# Kalkylinspektionsportalen

Kalkylinspektionsportalen är en **Blazor Server-applikation på .NET 10** för att presentera data från **Bidcon** i anpassningsbara rapporter.

## Syfte

Applikationens mål är att:
- presentera data från kalkylprogrammet Bidcon från Elecosoft i anpassningsbara rapporter
- koppla mot Bidcons webb-API
- distribuera rapporter utifrån rättigheter
- stödja både interaktiva och readonly-rapporter
- låta användare anpassa rapporter för internt bruk eller exempelvis säljunderlag

## Lösningens fokus

Huvudfokus i denna katalog är lösningen kring `BlazorServer`.

Nuvarande struktur bygger bland annat på:
- **Blazor Server** för UI
- **MudBlazor** för komponentbibliotek
- **MongoDB** för lagring av flera applikationsdata
- **feature-baserad struktur** via `IFeature`
- integration mot externt **Bidcon API**
- olika auth-flöden för development och production

## Projektöversikt

### `BlazorServer`
Huvudapplikationen i lösningen.

Viktiga egenskaper:
- bygger på **.NET 10** och **Blazor Server**
- använder **MudBlazor** för UI
- har en **feature-baserad struktur** där funktionalitet registreras via `IFeature`
- använder **MongoDB** för lagring av bland annat användarinställningar, åtkomst och vyer
- integrerar med ett externt **Bidcon API** via en namngiven `HttpClient`
- har olika auth-flöden för development respektive production:
  - development: mockad auth
  - production: Microsoft Identity / OpenID Connect

### `Tests/BlazorServer.Tests`
Testprojekt för BlazorServer.

Nuvarande fokus:
- NUnit används
- befintliga tester fokuserar just nu främst på `DataCellFormulaEvaluator`

## Komma igång

Se först:
- [docs/README.md](docs/README.md)
- [docs/development/local-development.md](docs/development/local-development.md)

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

## Viktiga backloggar och beslut

- [docs/backlog/improvement-areas.md](docs/backlog/improvement-areas.md)
- [docs/backlog/open-questions.md](docs/backlog/open-questions.md)
- [docs/adr/README.md](docs/adr/README.md)

## Nästa steg för projektbeskrivningen

1. Bekräfta projektets egentliga produktmål och målgrupp.
2. Beskriv domänbegrepp som exempelvis Bidcon, views, access claims och organisationskontext.
3. Fyll på med ADR:er när viktiga arkitekturbeslut tas.
4. Låt all ny funktionalitet uppdatera relevant dokumentation i `docs/`.
