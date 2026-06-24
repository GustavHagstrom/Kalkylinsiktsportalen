# Kalkylinspektionsportalen

Kalkylinspektionsportalen är en **Blazor Server-applikation på .NET 10** för att presentera kalkylinformation från **Bidcon** i anpassningsbara rapportvyer.

På sikt är målet också att låta användare arbeta med **projekt** som äger en eller flera kalkyler från Bidcon, versionshantera dessa över tid, dela dem med relevanta personer och kunna följa historik och skillnader mellan versioner.

Det här dokumentet är en kort ingång till lösningen. För domän, arkitektur och arbetssätt finns fördjupning i `docs/`.

## Vad som finns här

### `BlazorServer`
Huvudapplikationen i lösningen.

### `Tests/BlazorServer.Tests`
Testprojekt för huvudapplikationen.

## Läs detta först

- [docs/domain/product-description-draft.md](docs/domain/product-description-draft.md)
- [docs/architecture/overview.md](docs/architecture/overview.md)
- [docs/development/local-development.md](docs/development/local-development.md)
- [docs/backlog/improvement-areas.md](docs/backlog/improvement-areas.md)

## Vad dokumentationen används till

Dokumentationen i `docs/` används för att hålla isär fyra olika typer av information:

- `docs/domain/` beskriver **vad produkten och domänen är**
- `docs/architecture/` beskriver **hur lösningen är uppbyggd**
- `docs/development/` beskriver **hur man arbetar i repoet**
- `docs/backlog/` samlar **förbättringsområden och öppna frågor**

## Dokumentationsprincip

Målet är att behålla strukturen men undvika dubbelinformation.

Det betyder att:
- `README.md` ska vara kort och fungera som ingång
- produkt- och domänbeskrivning ska ligga i `docs/domain/`
- tekniska strukturfrågor ska ligga i `docs/architecture/`
- arbetsregler och förbättringsbehov ska ligga i egna dokument

## Nästa steg

1. Håll `product-description-draft.md` som huvuddokument för produktens syfte.
2. Håll `architecture/overview.md` som huvuddokument för lösningens struktur.
3. Lägg nya strukturproblem i `backlog/improvement-areas.md` i stället för i README.
