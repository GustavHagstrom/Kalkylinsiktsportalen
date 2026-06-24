# Dokumentationsindex

Den här katalogen är repoets långlivade kontextlager.

Målet är att dokumentationen ska hjälpa både utvecklare och agenter att svara på tre frågor:

1. Varför finns systemet?
2. Hur är det uppbyggt?
3. Hur gör vi ändringar utan att skapa onödig komplexitet?

## Struktur

### Domän

- [domain/overview.md](domain/overview.md) — begrepp, affärssyfte och domänmodell på övergripande nivå
- [domain/glossary.md](domain/glossary.md) — begreppslista med arbetsdefinitioner
- [domain/domain-map.md](domain/domain-map.md) — relationer mellan centrala domänobjekt
- [domain/product-description-draft.md](domain/product-description-draft.md) — första utkast till mer officiell produktbeskrivning

### Arkitektur

- [architecture/overview.md](architecture/overview.md) — systembild, beroenden och huvudflöden i nuläget
- [architecture/features-and-modules.md](architecture/features-and-modules.md) — nuvarande feature- och modulindelning
- [architecture/target-architecture.md](architecture/target-architecture.md) — målarkitektur för modulär monolit, snapshots och utbytbar frontend
- [architecture/migration-plan.md](architecture/migration-plan.md) — stegvis plan från nuvarande struktur till målarkitekturen

### Utveckling

- [development/local-development.md](development/local-development.md) — lokal setup, körning och vanliga arbetsflöden
- [development/coding-guidelines.md](development/coding-guidelines.md) — kodriktlinjer och repo-specifika arbetssätt

### Agenter

- [agents/repository-context.md](agents/repository-context.md) — stabil kontext att återanvända i promptar
- [agents/task-template.md](agents/task-template.md) — mall för uppgiftsbeskrivningar till agenter

### Beslut och backlog

- [backlog/open-questions.md](backlog/open-questions.md) — öppna frågor och arkitekturpunkter att reda ut
- [backlog/improvement-areas.md](backlog/improvement-areas.md) — identifierade förbättringsområden
- [adr/README.md](adr/README.md) — hur arkitekturbeslut dokumenteras

## Principer för dokumentation

### Skriv kort, konkret och versionsnära

Föredra flera små dokument framför en stor README.

### Dokumentera beslut, inte bara implementation

Kod visar vad som gjorts. Dokumentationen bör också visa varför.

### Uppdatera när ansvar eller riktning ändras

Dokumentation som blir inaktuell är värre än ingen dokumentation alls.

## Definition of Done för dokumentationsändringar

När du lägger till eller ändrar en feature bör du i normalfallet överväga om följande behöver uppdateras:

- domänbeskrivning
- arkitekturöversikt
- featurebeskrivning
- lokal utvecklingsguide
- agentkontext
- ADR om beslutet är långlivat eller svårt att backa ur
