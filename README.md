# Kalkylinspektionsportalen

Detta repo innehåller i nuläget främst en **Blazor Server-applikation på .NET 10** för att presentera och distribuera data från **Bidcon** i anpassningsbara rapporter.

Repoet innehåller också dokumentation för arkitektur, utvecklingsprinciper och agentarbete, så att både människor och AI-agenter kan arbeta mer konsekvent i lösningen.

## Syfte

Applikationens mål är i nuläget att:
- presentera data från kalkylprogrammet Bidcon från Elecosoft i anpassningsbara rapporter
- koppla mot Bidcons webb-API
- distribuera rapporter utifrån rättigheter
- stödja både interaktiva och readonly-rapporter
- låta användare anpassa rapporter för internt bruk eller exempelvis säljunderlag

## Projektöversikt

### `src/BlazorServer`
Huvudapplikationen i lösningen.

Nuvarande observationer:
- bygger på **Blazor Server** och **.NET 10**
- använder **MudBlazor** för UI
- använder en **feature-baserad struktur** via `IFeature`
- använder **MongoDB** för bland annat användarinställningar, åtkomst och vyer
- integrerar mot ett externt **Bidcon API** via `HttpClient`
- har olika auth-flöden för development och production

### `src/Tests/BlazorServer.Tests`
Testprojekt för huvudapplikationen.

## Komma igång

Se dokumentationen i `src/docs/` för mer detaljer.

Rekommenderad läsordning:
- [src/README.md](src/README.md)
- [src/docs/README.md](src/docs/README.md)
- [src/docs/architecture/overview.md](src/docs/architecture/overview.md)
- [src/docs/development/local-development.md](src/docs/development/local-development.md)
- [src/docs/agents/repository-context.md](src/docs/agents/repository-context.md)

## Dokumentation

Dokumentationen i `src/docs/` är tänkt att vara repoets långlivade kontextlager.

Den används för att beskriva:
- mål och domän
- arkitektur och ansvarsfördelning
- kodriktlinjer
- förbättringsområden
- hur agenter bör få kontext och arbeta i repoet

## Förbättringar och backlog

Identifierade förbättringsområden finns dokumenterade här:
- [src/docs/backlog/improvement-areas.md](src/docs/backlog/improvement-areas.md)

## Status

Repoet är under aktiv utveckling och dokumentationen är fortfarande under uppbyggnad.

Målet är att successivt göra både kodstruktur och dokumentation mer konsekvent, utan att stoppa leveransen av MVP-funktionalitet.
