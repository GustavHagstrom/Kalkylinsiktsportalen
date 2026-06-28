# Projects ADRs

Här ligger bara de Projects-beslut som fortfarande är riktiga ADR:er: långlivade trade-offs kring Projects-arkitektur, ansvar och tekniska vägval.

Den levande Projects-modellen finns i [../../../domain/projects.md](../../../domain/projects.md).

Lägg normalt inte följande här:

- aggregate-beskrivningar
- relationer mellan `Project`, `Projektversion`, `EstimationSnapshot` och `ViewScopedChangeSet`
- invariants
- ersättningspolicy som förväntas förfinas över tid
- `RenderableSnapshotState` som modulens levande kontraktsbeskrivning

Beslut om vydefinitioner, layout, formler och PDF-rendering hör normalt hemma i `docs/adr/modules/reports/`.
