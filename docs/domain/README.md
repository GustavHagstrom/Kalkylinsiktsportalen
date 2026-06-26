# Domanmodell

Den har katalogen innehaller repoets levande domandokumentation.

Syftet ar att beskriva den faktiska modellen som modulerna ager:

- aggregates och ansvar
- relationer mellan modeller
- invariants
- nuvarande policyer
- MVP-begransningar och tillfalliga val
- sannolika framtida variationer

## Nar `docs/domain/` Ska Anvandas

Beskriv domanmodellen har nar dokumentationen handlar om:

- begreppens struktur i en modul
- aggregate-granser och ansvar
- relationer mellan modulagda modeller
- invariants och livscykler
- nuvarande policyer som kan forfinas over tid

Produktbild, malbild och huvudfloden ags av [../product-and-domain.md](../product-and-domain.md). Kanoniska termer ags av [../../CONTEXT.md](../../CONTEXT.md).

Lagg inte sant i ADR:er som standard. ADR:er anvands bara nar ett modell- eller arkitekturval ocksa ar svart att backa ur, overraskande utan historiken och resultatet av en verklig trade-off.

## Karta

- [projects.md](projects.md) - Projects-modulens aggregates, relationer, invariants och policyer
- [reports.md](reports.md) - Reports-modulens aggregate, livscykel, relationer och policyer

Senare kan fler moduldokument laggas till, till exempel for `Bidcon`, `Access` och `Organizations`.

## Gemensamma Modelleringsprinciper

Foljande ar levande repoovergripande modelleringsprinciper, inte ADR:er:

- modulernas domanmodeller ska skydda sina invariants genom beteende, inte genom fritt muterbar state
- aggregates ska normalt ha en tydlig aggregate root som ager mutationer och skyddar barnobjekt bakom gransen
- aggregates och andra modeller med viktig initial state bor skapas genom tydliga fabriker eller beteendemetoder
- domanmodeller ska formas av verksamhetsregler och ansvar, inte av MongoDB- eller DTO-form
- centrala identiteter i domankod bor vara starka ID-typer nar det minskar risken att blanda ihop begrepp
- use case-granser bor returnera valideringsresultat for forvantade fel, medan domanmodeller ska skydda invariants med explicita domanfel
- IDs, tidpunkter och liknande omgivningsdata ska normalt komma in utifran use cases i stallet for att skapas inuti domanmodellen

De har principerna far skarpas och justeras nar modellen mognar. Om en princip blir ett verkligt langlivat, dyrt och overraskande trade-off-beslut kan den senare lyftas till en ADR.

## Mall For Moduldokument

Varje modulfil i `docs/domain/` bor normalt innehalla foljande rubriker:

1. Syfte och modulgrans
2. Huvudaggregates
3. Relationer
4. Invariants
5. Nuvarande policyer
6. MVP-begransningar och tillfalliga val
7. Sannolika framtida variationer
8. Relaterade ADR:er

Dokumenten ska vara levande och far forfinas nar modellen skarps. Hall dem konkreta och modulagda, utan att gora dem till implementationstunga specifikationer.
