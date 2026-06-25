# ADR - Architecture Decision Records

ADR:er används för att dokumentera långlivade tekniska beslut som är dyra att upptäcka i efterhand.

## När en ADR bör skrivas

Skriv en ADR bara när alla tre kriterier är uppfyllda:

- beslutet är svårt eller dyrt att backa ur
- beslutet är överraskande utan kontext
- beslutet är resultatet av en verklig trade-off

Exempel är arkitekturens riktning, data- eller auth-modell, integrationsmönster, gränser mellan moduler, persistensstrategi och avsiktliga avsteg från det uppenbara.

## Struktur

ADR:er ligger nära beslutets ägarskap:

- [global/](global/) - repoövergripande arkitektur, modulgränser och gemensamma riktlinjer
- [modules/bidcon/](modules/bidcon/) - Bidcon-import, snapshot-källa och Bidcon-nära modellbeslut
- [modules/projects/](modules/projects/) - projektversioner, snapshots i projekt, view-scoped change sets, ersättningspolicy och renderbar kalkylstate
- [modules/reports/](modules/reports/) - views, report definitions, layout, formler, PDF-export och vy-livscykel

Om ett beslut påverkar flera moduler läggs det där beslutet primärt ägs. Lägg det i `global/` när beslutet framför allt handlar om modulgränsen mellan två ägare.

## Namngivning

Behåll befintliga ADR-nummer när filer flyttas, så gamla hänvisningar fortsätter vara begripliga.

Nya ADR:er numreras lokalt i den mapp där de skapas:

- `global/0003-kort-beskrivning.md`
- `modules/projects/0025-kort-beskrivning.md`
- `modules/reports/0006-kort-beskrivning.md`

Använd ett kort format:

```md
# Kort titel

1-3 meningar: vilken kontext beslutet togs i, vad vi beslutade och varför.
```

## Valfria sektioner

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

### Bidcon

- [0005 - Store estimation snapshots as complete imports](modules/bidcon/0005-store-estimation-snapshots-as-complete-imports.md)

### Projects

- [0003 - Bind change sets to estimation snapshots](modules/projects/0003-bind-change-sets-to-estimation-snapshots.md)
- [0004 - Model project versions as snapshot selections](modules/projects/0004-model-project-versions-as-snapshot-selections.md)
- [0006 - Allow controlled estimation item replacements](modules/projects/0006-allow-controlled-estimation-item-replacements.md)
- [0007 - Use a modular replacement policy for tagged rows](modules/projects/0007-use-modular-replacement-policy-for-tagged-rows.md)
- [0008 - Store replacements as selections](modules/projects/0008-store-replacements-as-selections.md)
- [0009 - Keep change sets version-local](modules/projects/0009-keep-change-sets-version-local.md)
- [0010 - Allow one active replacement per original row](modules/projects/0010-allow-one-active-replacement-per-original-row.md)
- [0011 - Scope change sets to views](modules/projects/0011-scope-change-sets-to-views.md)
- [0013 - Share view-scoped change sets between users](modules/projects/0013-share-view-scoped-change-sets-between-users.md)
- [0014 - Use optimistic concurrency for shared replacements](modules/projects/0014-use-optimistic-concurrency-for-shared-replacements.md)
- [0016 - Defer undo and redo for view-scoped changes](modules/projects/0016-defer-undo-redo-for-view-scoped-changes.md)
- [0017 - Keep view-scoped change sets in Projects](modules/projects/0017-keep-view-scoped-change-sets-in-projects.md)
- [0020 - Keep existing view changes active when changes are disabled](modules/projects/0020-keep-existing-view-changes-active-when-disabled.md)
- [0024 - Use a general renderable snapshot state](modules/projects/0024-use-general-renderable-snapshot-state.md)

### Reports

- [0012 - Use View.EnableChanges for replacement permission](modules/reports/0012-use-view-enable-changes-for-replacement-permission.md)
- [0015 - Export active view state to PDF](modules/reports/0015-export-active-view-state-to-pdf.md)
- [0021 - Do not copy change sets when duplicating views](modules/reports/0021-do-not-copy-change-sets-when-duplicating-views.md)
- [0022 - Archive views with history](modules/reports/0022-archive-views-with-history.md)
- [0023 - Generate PDF exports on demand for MVP](modules/reports/0023-generate-pdf-exports-on-demand-for-mvp.md)
