# Ideas

Den här filen är en lågfriktions-inbox för idéer, misstankar och saker som kan behöva utredas senare.

Den är inte backlog, beslutad riktning eller en ersättning för GitHub Issues.

## När Filen Ska Användas

- Skriv ner sådant som känns viktigt men ännu inte är redo som issue, ADR eller planeringsfråga.
- Råa bullets är tillåtna. Det är viktigare att fånga signalen än att formulera en färdig specifikation.
- Använd den frivilliga miniformen när det hjälper: `Signal`, `Varför det kan spela roll`, `Nästa rimliga steg`.
- Använd inte status, prioritet eller ägarskap här. När en idé behöver sådan hantering ska den flyttas till GitHub Issues eller `planning.md`.
- Kodnära tekniska signaler är okej när de är större än en lokal TODO-kommentar, till exempel möjlig duplicering av policylogik mellan moduler. Små lokala kodstädningar hör inte hemma här.
- Flytta vidare konkreta uppgifter till GitHub Issues.
- Flytta vidare långlivade beslut till ADR när de uppfyller ADR-kriterierna.
- Ta bort eller arkivera idéer som har blivit issues, ADR:er, beslut eller inte längre gäller.
- Vid planeringsarbete får agenter föreslå gallring av gamla idéer, men de ska inte ta bort idéer utan uttryckligt okej.

## Agentinstruktion

Agenter bör läsa den här filen vid planering, arkitekturarbete, produktfrågor eller när användaren explicit nämner idéer.

Agenter ska inte behandla idéer här som beslutad riktning. En idé är bara en signal om något som kan behöva undersökas.

Agenter får restriktivt lägga till idéer när de upptäcker något relevant men utanför aktuell scope. Anteckningen ska vara kort och får inte störa huvuduppgiften.

## Idéer

- 

## Frivillig Miniform

```text
### Kort iderubrik

Signal:

Varför det kan spela roll:

Nästa rimliga steg:
```

### EstimationExplorer
Bicon bör äga en Estimationbrowser. Exponera komponent via public/components. Andra moduler kan använda denna. 
Denna komponent vill nog project använda vid val av kalkyl som ska tillhöra projektet.
Bidconmodujle vill kanske också ha en sida som använder denna kompoent men ge som alternativ att skapa projekt utifrån valda kalkyler. Eller så tillhör även denna funktion project modulen.
Man kanske bara vill bläddra runt bland kalklyler lite. Var ska en sådan kompenent ligga? Om man bara bläddrar runt lite kan det ju hända att man också vill titta på en vald kalkyl eller skapa projekt från den eller bara en engångrapportl.
Var ska alla dessa alternativ ägas av?

### Projektavgränsade moduler och presentationsadaptrar

**Status:** Idé för framtida arkitekturriktning. Detta är inte ett beslutat mål eller en färdig migreringsplan.

**Signal:** Produktmodulerna ligger idag som mappar i Blazor Server-projektet och innehåller både verksamhetslogik, infrastruktur och Blazor-komponenter. Det försvårar upprätthållandet av modulgränser och kopplar modulernas implementation till nuvarande presentationsteknik.

**Varför det kan spela roll:** En framtida övergång till exempelvis ASP.NET Core API och React bör kunna göras utan att modulernas kärnbeteende behöver flyttas eller skrivas om.

Grundidén är att dela projekt efter verkliga seams och utbytbara adapters, inte mekaniskt efter klassiska lager som Domain, Application och Infrastructure.

En modul kan vid behov bestå av:

```text
Modules/
└── Bidcon/
    ├── Kalkylinsiktsportalen.Bidcon/
    ├── Kalkylinsiktsportalen.Bidcon.Contracts/
    ├── Kalkylinsiktsportalen.Bidcon.Blazor/
    └── Kalkylinsiktsportalen.Bidcon.Api/
```

Alla projekt behöver inte skapas från början.

#### Föreslagna projekttyper och ansvar

- `Kalkylinsiktsportalen.Bidcon`: vanligt class library med `Microsoft.NET.Sdk`. Innehåller modulens domänmodell, use cases, handlers, datalagring, Bidcon-integration och intern registrering. Domain, Application och Infrastructure kan tills vidare vara mappar och interna namespaces i samma projekt.
- `Kalkylinsiktsportalen.Bidcon.Contracts`: vanligt class library med modulens lilla externa interface mot andra moduler. Skapas bara när andra moduler faktiskt behöver konsumera Bidcon.
- `Kalkylinsiktsportalen.Bidcon.Blazor`: Razor Class Library med `Microsoft.NET.Sdk.Razor`. Innehåller routbara sidor, återanvändbara Blazor-komponenter och UI-specifik registrering.
- `Kalkylinsiktsportalen.Bidcon.Api`: vanligt class library med `FrameworkReference` till `Microsoft.AspNetCore.App`. Innehåller Minimal API-endpoints eller annan HTTP-adapter och skapas först när HTTP-gränssnittet faktiskt behövs.

Det publika UI-interfacet bör vara litet. Exempelvis kan `EstimationExplorer` exponeras som en sammanhållen komponent medan interna tabeller, filter och dialoger förblir implementation.

#### Referensriktning

```text
Bidcon.Blazor ──→ Bidcon
Bidcon.Api ─────→ Bidcon
Bidcon ─────────→ Bidcon.Contracts

Projects ───────→ Bidcon.Contracts
Reports ────────→ Bidcon.Contracts
```

Följande beroenden bör undvikas:

```text
Projects ───────→ Bidcon
Projects.Blazor → Bidcon.Blazor
Bidcon ─────────→ AppShell
```

AppShell ligger kvar i den körbara ASP.NET Core-hosten och äger navigation, layout och Settings Hub. Produktmoduler bidrar med neutral metadata via composition-kontrakt och refererar inte AppShell.

Blazor-hosten kan komponera:

```csharp
builder.Services.AddBidconModule(registrationContext);
builder.Services.AddBidconBlazor(registrationContext);
```

En framtida API-host kan i stället eller parallellt komponera:

```csharp
builder.Services.AddBidconModule(registrationContext);
builder.Services.AddBidconApi();
app.MapBidconEndpoints();
```

**Nästa rimliga steg:** Testa upplägget på Bidcon i liten skala:

1. Skapa `Kalkylinsiktsportalen.Bidcon` och flytta icke-UI-relaterad implementation dit.
2. Skapa `Kalkylinsiktsportalen.Bidcon.Blazor` som Razor Class Library och flytta exempelvis `EstimationExplorer` och Bidcon-inställningssidor dit.
3. Skapa `Bidcon.Contracts` endast för kontrakt som Projects eller Reports faktiskt behöver.
4. Vänta med `Bidcon.Api` tills ett HTTP-gränssnitt efterfrågas.

> Dela projekt efter seams och utbytbara adapters, inte efter lager enbart för strukturens skull.

Målet är djupa moduler med ett litet interface, tydlig referensriktning och intern frihet att organisera implementationen utan att läcka den till andra moduler.
