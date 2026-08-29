# Reports ADRs

Här ligger bara de Reports-beslut som fortfarande är riktiga ADR:er: långlivade trade-offs kring export, historik, ansvar och tekniska vägval.

Den levande Reports-modellen finns i [../../../domain/reports.md](../../../domain/reports.md).

- [0024 - Edit views through intent commands](0024-edit-views-through-intent-commands.md)
- [0025 - Model SheetSection invariants explicitly](0025-model-sheet-section-invariants-explicitly.md)

Lägg normalt inte följande här:

- `View` som aggregate
- view-livscykel som levande modell
- `EnableChanges` som modulregel
- dupliceringsregler för views

Beslut om att validera, lagra eller applicera ersättningar mot snapshots hör normalt hemma i `docs/adr/modules/projects/`.
