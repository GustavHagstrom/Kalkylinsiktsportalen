# Dokumentationsindex

Den här katalogen är repoets långlivade kontextlager.

Målet är att dokumentationen ska vara kompakt och hjälpa utvecklare och agenter att svara på tre frågor:

1. Varför finns systemet?
2. Hur är det uppbyggt?
3. Hur gör vi ändringar utan att skapa onödig komplexitet?

## Läs först

För de flesta ändringar räcker den här ordningen:

1. [product-and-domain.md](product-and-domain.md) — produktens syfte, domänmodell och begrepp
2. [architecture/target-architecture.md](architecture/target-architecture.md) — långsiktig målbild
3. [architecture/migration-plan.md](architecture/migration-plan.md) — stegvis väg dit
4. [architecture/next-phase-plan.md](architecture/next-phase-plan.md) — konkret plan för nästa migrationsfas
5. [architecture/current-state.md](architecture/current-state.md) — nuläge och mapping från features till målmoduler
6. [development.md](development.md) — lokal utveckling och kodriktlinjer

Om målarkitekturen och nuläget säger olika saker gäller målarkitekturen som riktning, medan nulägesdokumenten beskriver hur koden ser ut idag.

## Struktur

- [product-and-domain.md](product-and-domain.md) — produktbeskrivning, domänkarta, huvudflöden och begrepp
- [architecture/target-architecture.md](architecture/target-architecture.md) — målarkitektur för modulär monolit, snapshots och utbytbar frontend
- [architecture/migration-plan.md](architecture/migration-plan.md) — stegvis plan från nuvarande struktur till målarkitekturen
- [architecture/next-phase-plan.md](architecture/next-phase-plan.md) — aktuell plan för nästa migrationsfas
- [architecture/current-state.md](architecture/current-state.md) — nuläge, runtime-flöden, infrastruktur och feature-mapping
- [development.md](development.md) — lokal setup, arbetsflöde, kodriktlinjer och Definition of Done
- [planning.md](planning.md) — planeringsprinciper och öppna frågor som ännu inte blivit issues eller ADR:er
- [agent-guide.md](agent-guide.md) — kort agentkontext och uppgiftsmall
- [adr/README.md](adr/README.md) — hur arkitekturbeslut dokumenteras

## Dokumenttyper

### Primära styrdokument

Primära styrdokument ska vara korta, aktiva och hållas uppdaterade när riktningen ändras:

- produkt och domän
- målarkitektur och migrationsplan
- utvecklingsriktlinjer
- planeringsprinciper och öppna frågor

### Nulägesdokument

Nulägesdokument beskriver observerad kod. De får vara mer deskriptiva, men ska inte styra nya större vägval om de krockar med målarkitekturen.

### Planering och öppna frågor

Konkreta uppgifter bör hanteras som GitHub Issues. Docs ska bara samla planeringsprinciper och frågor som ännu inte är issues eller ADR:er.

## Principer för dokumentation

- Skriv kort, konkret och versionsnära.
- Dokumentera beslut och riktning, inte bara implementation.
- Uppdatera docs när ansvar eller riktning ändras.
- Undvik parallella dokument som beskriver samma ansvar på olika sätt.
- Lägg hellre en kort länk till det dokument som äger frågan.

## Definition of Done för dokumentationsändringar

När du lägger till eller ändrar en feature bör du i normalfallet överväga om följande behöver uppdateras:

- produkt/domän
- målarkitektur eller migrationsplan
- nuläge
- utvecklingsriktlinjer
- planering eller öppna frågor
- ADR om beslutet är långlivat eller svårt att backa ur
