# Plan för nästa migrationsfas

## Var vi är nu

Implementationens migrationsläge är mellan fas 2 och fas 3 i [migration-plan.md](migration-plan.md).

Genomfört:
- Fas 1 är klar: målarkitektur, migrationsplan och dokumentationsindex finns.
- Fas 2 är klar: `src/BlazorServer/Modules/*` och nya `Shared/UI`, `Shared/Kernel`, `Shared/Infrastructure` finns.

Inte genomfört ännu:
- Modulmapparna innehåller bara platsmarkörer.
- Produktlogik ligger fortfarande huvudsakligen i `Shared` och `Features`.
- Blazor-komponenter anropar fortfarande stores och `BidconApiAccess` direkt.
- Det finns inget etablerat use case-lager för kärnflödet.

Nästa fas bör därför vara **fas 3A: etablera första use case-slicen**.

## Fas 3A: Etablera första use case-slicen

### Mål

Skapa ett konkret mönster för use cases i `Modules/*` utan att samtidigt göra en stor omflyttning av befintlig kod.

Fasen ska bevisa att:
- UI kan anropa ett use case i stället för att orkestrera flödet själv.
- use caset kan testas utan Blazor-rendering.
- modulgränserna kan användas i praktiken innan vi flyttar större mängder kod.

### Rekommenderat första flöde

Använd `Bidcon/FetchCompleteEstimation` som första vertikala slice.

Motivering:
- Den är redan identifierad i migrationsplanen.
- Befintligt ansvar finns tydligt samlat runt `Shared/Services/BidconApiAccess.cs`.
- Flödet har tydliga inputs och outputs.
- Det är en bra förberedelse för kommande Projects-snapshotflöde.

### Föreslagen struktur

```text
src/BlazorServer/Modules/Bidcon/
  FetchCompleteEstimation/
    FetchCompleteEstimationRequest.cs
    FetchCompleteEstimationResponse.cs
    FetchCompleteEstimationHandler.cs
  Contracts/
    IBidconEstimationReader.cs
  Infrastructure/
    BidconEstimationReader.cs
```

Om strukturen känns för tung i första steget kan `Contracts` och `Infrastructure` vänta, men request/response och handler bör finnas från början.

### Arbetsordning

1. Lägg till request/response-modeller för `FetchCompleteEstimation`.
2. Lägg till en handler som kapslar anropet till befintlig Bidcon-kod.
3. Introducera ett smalt kontrakt för att läsa estimatdata, så handlern inte behöver känna till HTTP-detaljer.
4. Registrera handler och kontrakt i composition root.
5. Uppdatera en befintlig UI-yta som läser ett estimat till att gå via handlern.
6. Lägg till fokuserade tester för handlern med fakead estimatläsare.
7. Dokumentera mönstret så nästa use case kan följa samma form.

### Definition of done

- Minst ett verkligt kärnflöde går via `Modules/*`.
- Berörd UI-komponent anropar ett use case i stället för `BidconApiAccess` direkt.
- Use caset har request/response-modeller utan Blazor-specifika typer.
- Use caset har testtäckning utan Blazor-rendering.
- Ingen bred flytt av `Shared` har gjorts som del av fasen.

## Nästa fas efter 3A

När `FetchCompleteEstimation` är på plats bör fas 3B bygga vidare med Projects-flödet:

```text
Projects/CreateProject
Projects/AddEstimationToProject
Projects/CreateSnapshotForProjectEstimation
```

Först då bör snapshotmodellering och immutable sparning börja konkretiseras.

