# Dokumentationsindex

Den här katalogen är repoets långlivade kontextlager.

Målet är att dokumentationen ska vara kompakt och hjälpa utvecklare och agenter att svara på tre frågor:

1. Varför finns systemet?
2. Hur är det uppbyggt?
3. Hur gör vi ändringar utan att skapa onödig komplexitet?

## Läs Först

För de flesta ändringar räcker den här ordningen:

1. [../CONTEXT.md](../CONTEXT.md) - kanonisk ordlista för domänspråk
2. [product-and-domain.md](product-and-domain.md) - produktens syfte, övergripande domänkarta och huvudflöden
3. [domain/README.md](domain/README.md) - levande domänmodell och modulvisa domändokument
4. [architecture/target-architecture.md](architecture/target-architecture.md) - långsiktig målbild
5. [development.md](development.md) - lokal utveckling och kodriktlinjer

Om äldre kod och målarkitekturen säger olika saker gäller målarkitekturen som riktning för ny produktlogik. Små ändringar kan fortfarande göras nära befintlig feature enligt [development.md](development.md).

## Struktur

- [../CONTEXT.md](../CONTEXT.md) - kanonisk ordlista för domänspråk
- [product-and-domain.md](product-and-domain.md) - produktbeskrivning, övergripande domänkarta och huvudflöden
- [domain/README.md](domain/README.md) - levande domänmodell och mall för modulvisa domändokument
- [domain/access.md](domain/access.md) - Access-modulens grants, claim-gräns och externa role/group-ansvar
- [domain/projects.md](domain/projects.md) - Projects-modulens aggregates, invariants och policyer
- [domain/reports.md](domain/reports.md) - Reports-modulens aggregate, livscykel och policyer
- [architecture/target-architecture.md](architecture/target-architecture.md) - målarkitektur för modulär monolit, snapshots och utbytbar frontend
- [development.md](development.md) - lokal setup, arbetsflöde, kodriktlinjer och Definition of Done
- [planning.md](planning.md) - planeringsprinciper och öppna frågor som ännu inte blivit issues eller ADR:er
- [agent-guide.md](agent-guide.md) - agentinstruktioner och uppgiftsmall
- [adr/README.md](adr/README.md) - hur arkitekturbeslut dokumenteras

## Dokumentägarskap

För att undvika motsägelser ska varje typ av information ha en primär ägare:

- [../CONTEXT.md](../CONTEXT.md) äger kanoniska termer och endast termer.
- [product-and-domain.md](product-and-domain.md) äger produktsyfte, målbild, användartyper, huvudflöden och globala domängränser.
- [domain/README.md](domain/README.md) och `domain/*.md` äger modulernas levande domänmodell: aggregates, relationer, invariants och modulnära policyer.
- [architecture/target-architecture.md](architecture/target-architecture.md) äger arkitekturprinciper, modulgränser och beroenderiktning.
- [development.md](development.md) äger lokal setup, arbetsflöde och praktiska kodriktlinjer.
- [planning.md](planning.md) äger öppna frågor som ännu inte blivit issues eller ADR:er.
- [agent-guide.md](agent-guide.md) äger agentinstruktioner och uppgiftsmallar, inte allmän produkt- eller arkitekturbeskrivning.
- [adr/README.md](adr/README.md) och `adr/**/*.md` äger långlivade, svårbackade trade-offs.

Om samma information behöver nämnas i flera dokument ska sekundära dokument sammanfatta kort och länka till den primära källan i stället för att återberätta detaljerna.

## Dokumenttyper

### Primära styrdokument

Primära styrdokument ska vara korta, aktiva och hållas uppdaterade när riktningen ändras:

- kanoniskt domänspråk
- produkt och övergripande domänkarta
- levande modulägd domänmodell
- målarkitektur
- utvecklingsriktlinjer
- planeringsprinciper och öppna frågor

### Planering och Öppna Frågor

Konkreta uppgifter bör hanteras som GitHub Issues. Docs ska bara samla planeringsprinciper och frågor som ännu inte är issues eller ADR:er.

Beslut som är långlivade, svåra att backa ur och resultatet av en verklig trade-off ska dokumenteras som korta ADR:er via [adr/README.md](adr/README.md), med filen placerad under rätt global- eller modulmapp.

## Principer För Dokumentation

- Skriv kort, konkret och versionsnära.
- Dokumentera beslut och riktning, inte bara implementation.
- Låt ett dokument äga varje informationsslag.
- Uppdatera [../CONTEXT.md](../CONTEXT.md) när kanoniska domäntermer ändras eller etableras.
- Uppdatera `docs/domain/*.md` när aggregates, relationer, invariants eller modulnära policyer förfinas.
- Uppdatera docs när ansvar eller riktning ändras.
- Undvik parallella dokument som beskriver samma ansvar på olika sätt.
- Lägg hellre en kort länk till det dokument som äger frågan.

## Definition Of Done För Dokumentationsändringar

När du lägger till eller ändrar en feature bör du i normalfallet överväga om följande behöver uppdateras:

- kanoniskt domänspråk
- produktbild och övergripande domänkarta
- levande modulägd domänmodell
- målarkitektur
- utvecklingsriktlinjer
- planering eller öppna frågor
- ADR om beslutet är långlivat eller svårt att backa ur
