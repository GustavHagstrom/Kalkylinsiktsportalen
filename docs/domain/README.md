# Domänmodell

Den här katalogen innehåller repoets levande domändokumentation.

Syftet är att beskriva den faktiska modellen som modulerna äger:

- aggregates och ansvar
- relationer mellan modeller
- invariants
- nuvarande policyer
- MVP-begränsningar och tillfälliga val
- sannolika framtida variationer

## När `docs/domain/` Ska Användas

Beskriv domänmodellen här när dokumentationen handlar om:

- begreppens struktur i en modul
- aggregate-gränser och ansvar
- relationer mellan modulägda modeller
- invariants och livscykler
- nuvarande policyer som kan förfinas över tid

Produktbild, målbild och huvudflöden ägs av [../product-and-domain.md](../product-and-domain.md). Kanoniska termer ägs av [../../CONTEXT.md](../../CONTEXT.md).

Lägg inte sådant i ADR:er som standard. ADR:er används bara när ett modell- eller arkitekturval också är svårt att backa ur, överraskande utan historiken och resultatet av en verklig trade-off.

## Karta

- [Bidcon.md](Bidcon.md) - Bidcon-modulens integration, snapshots, importregler och normaliserade payload
- [projects.md](projects.md) - Projects-modulens aggregates, relationer, invariants och policyer
- [reports.md](reports.md) - Reports-modulens aggregate, livscykel, relationer och policyer

Senare kan fler moduldokument läggas till, till exempel för `Access` och `Organizations`.

## Gemensamma Modelleringsprinciper

Följande är levande repoövergripande modelleringsprinciper, inte ADR:er:

- modulernas domänmodeller ska skydda sina invariants genom beteende, inte genom fritt muterbar state
- aggregates ska normalt ha en tydlig aggregate root som äger mutationer och skyddar barnobjekt bakom gränsen
- aggregates och andra modeller med viktig initial state bör skapas genom tydliga fabriker eller beteendemetoder
- domänmodeller ska formas av verksamhetsregler och ansvar, inte av MongoDB- eller DTO-form
- centrala identiteter i domänkod bör vara starka ID-typer när det minskar risken att blanda ihop begrepp
- use case-gränser bör returnera valideringsresultat för förväntade fel, medan domänmodeller ska skydda invariants med explicita domänfel
- IDs, tidpunkter och liknande omgivningsdata ska normalt komma in utifrån use cases i stället för att skapas inuti domänmodellen

De här principerna får skärpas och justeras när modellen mognar. Om en princip blir ett verkligt långlivat, dyrt och överraskande trade-off-beslut kan den senare lyftas till en ADR.

## Mall För Moduldokument

Varje modulfil i `docs/domain/` bör normalt innehålla följande rubriker:

1. Syfte och modulgräns
2. Huvudaggregates
3. Relationer
4. Invariants
5. Nuvarande policyer
6. MVP-begränsningar och tillfälliga val
7. Sannolika framtida variationer
8. Relaterade ADR:er

Dokumenten ska vara levande och får förfinas när modellen skärps. Håll dem konkreta och modulägda, utan att göra dem till implementationstunga specifikationer.
