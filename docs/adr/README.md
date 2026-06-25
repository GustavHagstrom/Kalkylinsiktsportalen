# ADR - Architecture Decision Records

ADR:er anvands for att dokumentera langlivade tekniska beslut som ar dyra att upptacka i efterhand.

## Nar En ADR Bor Skrivas

Skriv en ADR bara nar alla tre kriterier ar uppfyllda:

- beslutet ar svart eller dyrt att backa ur
- beslutet ar overraskande utan kontext
- beslutet ar resultatet av en verklig trade-off

Om nagot saknar nagon av de tre egenskaperna ska det normalt inte vara en ADR.

Folande hor normalt inte hemma i ADR:er:

- domanordlista och begreppsdefinitioner
- domanmodellens lopande struktur
- aggregate, entiteter, relationer och invariants som del av den levande modellen
- modulnara modellbeskrivningar som forvantas forfinas iterativt

Sant ska i stallet beskrivas i levande domandokument, medan ADR:er ska forklara de fa modellval som verkligen var svara att valja, dyra att andra och overraskande utan historiken.

Exempel ar arkitekturens riktning, data- eller auth-modell, integrationsmonster, granser mellan moduler, persistensstrategi och avsiktliga avsteg fran det uppenbara.

## Struktur

ADR:er ligger nara beslutets agarskap:

- [global/](global/) - repoovergripande arkitektur, modulgranser och gemensamma riktlinjer
- [modules/bidcon/](modules/bidcon/) - Bidcon-import, snapshot-kalla och Bidcon-nara modellbeslut
- [modules/projects/](modules/projects/) - langlivade Projects-beslut som fortfarande ar verkliga trade-offs
- [modules/reports/](modules/reports/) - langlivade Reports-beslut kring export, historik och ansvar

Om ett beslut paverkar flera moduler laggs det dar beslutet primart ags. Lagg det i `global/` nar beslutet framfor allt handlar om modulgransen mellan tva agare.

## Namngivning

Behall befintliga ADR-nummer nar filer flyttas, sa gamla hanvisningar fortsattar vara begripliga.

Nya ADR:er numreras lokalt i den mapp dar de skapas:

- `global/0003-kort-beskrivning.md`
- `modules/projects/0025-kort-beskrivning.md`
- `modules/reports/0006-kort-beskrivning.md`

Anvand ett kort format:

```md
# Kort titel

1-3 meningar: vilken kontext beslutet togs i, vad vi beslutade och varfor.
```

## Valfria Sektioner

Lagg bara till extra sektioner nar de faktiskt hjalper en framtida lasare:

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

### Projects

- [0014 - Use optimistic concurrency for shared replacements](modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md)

### Reports

- [0015 - Export active view state to PDF](modules/reports/0015-export-active-view-state-to-pdf.md)
- [0022 - Archive views with history](modules/reports/0022-archive-views-with-history.md)
- [0023 - Generate PDF exports on demand for MVP](modules/reports/0023-generate-pdf-exports-on-demand-for-mvp.md)
