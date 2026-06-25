# Agentguide

Det här dokumentet samlar stabil kontext och en enkel uppgiftsmall för AI-agenter.

## Kort Kontext

Repoet innehåller en Blazor Server-applikation på .NET 10 med MudBlazor, MongoDB, feature-baserad struktur och integration mot Bidcon API.

Arkitekturriktningen är en modulär monolit enligt [architecture/target-architecture.md](architecture/target-architecture.md), med stegvis migrering enligt [architecture/migration-plan.md](architecture/migration-plan.md).

Kanoniskt domänspråk finns i [../CONTEXT.md](../CONTEXT.md). Produktbild och huvudflöden finns i [product-and-domain.md](product-and-domain.md). Levande modulägd domänmodell finns i [domain/README.md](domain/README.md).

## Viktigt Att Veta

- Huvudprojektet är `src/BlazorServer`.
- UI byggs med Blazor Server.
- Nuvarande struktur använder `Features/*` och `IFeature`.
- Mycket tvärgående kod ligger idag i `Shared/*`, men `Shared` ska krympas.
- Data lagras bland annat i MongoDB.
- Auth skiljer sig mellan development och production.
- Tester finns i `src/Tests/BlazorServer.Tests`.
- Målmodulerna är `Projects`, `Reports`, `Bidcon`, `Access` och `Organizations`.

## Arbetssätt För Agenter

- läs relevanta filer innan ändring
- läs målarkitektur och migrationsplan om ändringen påverkar struktur eller ansvar
- gör små, fokuserade ändringar
- håll små ändringar nära befintlig feature
- lägg ny produktkod i `Modules/*` när det passar migrationsplanen
- lägg inte saker i `Shared` i onödan
- undvik nya paket om de inte behövs
- uppdatera [../CONTEXT.md](../CONTEXT.md) när ett kanoniskt domänbegrepp ändras eller etableras
- uppdatera `docs/domain/*.md` när aggregates, relationer, invariants eller modulnära policyer förfinas
- skriv en kort ADR enligt [adr/README.md](adr/README.md) när ett beslut är långlivat, svårt att backa ur och resultatet av en verklig trade-off
- använd inte ADR:er för löpande domänmodellering; aggregate, relationer, invariants och modulnära modellbeskrivningar hör normalt hemma i levande domändokument
- använd [planning.md](planning.md) för öppna frågor, inte för beslut som redan är fattade
- uppdatera docs när ansvar, regler eller arkitektur påverkas
- verifiera med relevanta tester eller annan rimlig kontroll

## Skills-Arbetsflöde Från Idé Till Handoff

Använd det här flödet när en ny idé behöver förfinas, brytas ned, byggas och lämnas vidare på ett spårbart sätt:

1. Ny idé
2. `/grill-with-docs`
3. `/to-prd`
4. `/to-issues`
5. Välj ett issue
6. `/tdd`
7. Implementera
8. Tester gröna
9. `/improve-codebase-architecture` vid behov
10. Commit
11. `/handoff` om du byter session

## Kontrollfrågor Innan Ändring

1. Vilken feature äger problemet idag?
2. Vilken målmodul bör äga ny produktlogik?
3. Är ändringen UI, state, use case, service, store eller auth?
4. Finns redan ett etablerat mönster i repoet?
5. Kräver ändringen ny policy, claim eller registrering?
6. Behöver tester eller docs uppdateras?

## Kort Uppgiftsmall

```text
Mål:
Beskriv exakt vad som ska åstadkommas.

Kontext:
Ange berörd feature, modul eller filområde.

Begränsningar:
Lista sådant som inte ska ändras, till exempel routes, auth eller paket.

Acceptanskriterier:
Beskriv när uppgiften är klar.

Verifiering:
Ange vilka tester eller kontroller som ska köras.
```

## Exempelprompt

```text
Läs först relevanta filer och sammanfatta kort hur berörd feature fungerar idag.
Läs målarkitektur och migrationsplan om ändringen påverkar struktur eller ansvar.
Implementera minsta rimliga ändring.
Följ befintlig feature-struktur för små ändringar och målmodulerna för ny produktkod.
Lägg inte till nya paket.
Uppdatera dokumentation om ansvar, regler eller arkitektur påverkas.
Verifiera med relevanta tester eller annan lämplig validering.
```

