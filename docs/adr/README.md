# ADR - Architecture Decision Records

ADR:er används för att dokumentera långlivade tekniska beslut som är dyra att upptäcka i efterhand.

## När En ADR Bör Skrivas

Skriv en ADR bara när alla tre kriterier är uppfyllda:

- beslutet är svårt eller dyrt att backa ur
- beslutet är överraskande utan kontext
- beslutet är resultatet av en verklig trade-off

Om något saknar någon av de tre egenskaperna ska det normalt inte vara en ADR.

Följande hör normalt inte hemma i ADR:er:

- domänordlista och begreppsdefinitioner
- domänmodellens löpande struktur
- aggregate, entiteter, relationer och invariants som del av den levande modellen
- modulnära modellbeskrivningar som förväntas förfinas iterativt

Sådant ska i stället beskrivas i levande domändokument, medan ADR:er ska förklara de få modellval som verkligen var svåra att välja, dyra att ändra och överraskande utan historiken.

Exempel är arkitekturens riktning, data- eller auth-modell, integrationsmönster, gränser mellan moduler, persistensstrategi och avsiktliga avsteg från det uppenbara.

## Struktur

ADR:er ligger nära beslutets ägarskap:

- [global/](global/) - repoövergripande arkitektur, modulgränser och gemensamma riktlinjer
- [modules/bidcon/](modules/bidcon/) - Bidcon-import, snapshot-källa och Bidcon-nära modellbeslut
- [modules/projects/](modules/projects/) - långlivade Projects-beslut som fortfarande är verkliga trade-offs
- [modules/reports/](modules/reports/) - långlivade Reports-beslut kring export, historik och ansvar

Om ett beslut påverkar flera moduler läggs det där beslutet primärt ägs. Lägg det i `global/` när beslutet framför allt handlar om modulgränsen mellan två ägare.

## Namngivning

Behåll befintliga ADR-nummer när filer flyttas, så gamla hänvisningar fortsatt är begripliga.

Nya ADR:er numreras lokalt i den mapp där de skapas:

- `global/0003-kort-beskrivning.md`
- `modules/projects/0025-kort-beskrivning.md`
- `modules/reports/0006-kort-beskrivning.md`

Använd ett kort format:

```md
# Kort titel

1-3 meningar: vilken kontext beslutet togs i, vad vi beslutade och varför.
```

## Valfria Sektioner

Lägg bara till extra sektioner när de faktiskt hjälper en framtida läsare:

- status: `proposed`, `accepted`, `deprecated` eller `superseded by ADR-NNNN`
- considered options
- consequences

## Befintliga ADR:er

### Global

- [0001 - Use feature slices with target modules](global/0001-use-feature-slices-with-target-modules.md)
- [0002 - Limit Shared to stable cross-cutting building blocks](global/0002-limit-shared-to-stable-cross-cutting-building-blocks.md)
- [0018 - Split active state building from report rendering](global/0018-split-active-state-building-from-report-rendering.md)
- [0019 - Pass view change settings to Projects](global/0019-pass-view-change-settings-to-projects.md)
- [0027 - Keep domain models persistence-independent](global/0027-keep-domain-models-persistence-independent.md)
- [0034 - Defer domain events during initial migration](global/0034-defer-domain-events-during-initial-migration.md)
- [0035 - Let Web orchestrate Projects and Reports initially](global/0035-let-web-orchestrate-projects-and-reports-initially.md)
- [0037 - Use pragmatic CQRS feature slices](global/0037-use-pragmatic-cqrs-feature-slices.md)

### Bidcon

- [0005 - Store estimation snapshots as complete imports](modules/bidcon/0005-store-estimation-snapshots-as-complete-imports.md)
- [0006 - Store Bidcon credentials in Mongo for MVP](modules/bidcon/0006-store-bidcon-credentials-in-mongo-for-mvp.md)

### Projects

- [0014 - Use optimistic concurrency for shared replacements](modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md)

### Reports

- [0015 - Export active view state to PDF](modules/reports/0015-export-active-view-state-to-pdf.md)
- [0022 - Archive views with history](modules/reports/0022-archive-views-with-history.md)
- [0023 - Generate PDF exports on demand for MVP](modules/reports/0023-generate-pdf-exports-on-demand-for-mvp.md)
