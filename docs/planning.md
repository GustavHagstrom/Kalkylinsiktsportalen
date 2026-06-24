# Planering

Backlog och konkreta uppgifter bör i första hand hanteras som GitHub Issues.

Docs ska inte vara en parallell ärendelista. Den här filen används bara för planeringsprinciper och öppna frågor som ännu inte är tillräckligt konkreta för en issue eller ADR.

## Principer

- Skapa GitHub Issues för konkreta buggar, förbättringar och implementation tasks.
- Använd labels/milestones/project board i GitHub för prioritet och status.
- Skriv ADR när ett beslut är långlivat, svårt att backa ur eller påverkar arkitektur.
- Uppdatera docs när riktning, ansvar eller arbetssätt ändras.
- Ta bort planeringsanteckningar här när de har blivit issues, ADR:er eller beslut.

## Öppna Arkitekturfrågor

- När ska `Modules/*` skapas fysiskt i `src/BlazorServer`?
- Hur mycket av `IFeature`-mönstret ska behållas när modulerna blir tydligare?
- Ska WYSIWYG/PDF vara en intern del av `Reports` eller en separat renderingmodul?
- Hur ska `SnapshotChangeSet` modelleras?

## Öppna Produktfrågor

- Vilka användartyper är officiella?
- Vilket namn är officiellt för produkt, repo och solution?
- Vilka delar av [product-and-domain.md](product-and-domain.md) ska bli beslutad produktbeskrivning?

## Öppna Säkerhetsfrågor

- Vilken policy gäller för User Secrets, miljövariabler och annan hemlighetshantering?
- Hur ska Bidcon credentials lagras och roteras?

