# Idéer

## Projektavgränsade moduler och presentationsadaptrar

### Status

Idé för framtida arkitekturriktning. Detta är inte ett beslutat mål eller en färdig migreringsplan.

### Bakgrund

Produktmodulerna ligger idag som mappar i Blazor Server-projektet och innehåller både verksamhetslogik, infrastruktur och Blazor-komponenter. Det gör modulgränserna svårare att upprätthålla och kopplar modulernas implementation till den nuvarande presentationsformen.

En framtida övergång till exempelvis ASP.NET Core API + React bör kunna göras utan att modulernas kärnbeteende behöver flyttas eller skrivas om.

### Grundidé

Dela projekt efter verkliga seams och utbytbara adapters, inte mekaniskt efter klassiska lager som Domain, Application och Infrastructure.

Varje produktmodul kan vid behov bestå av följande projekt:

```text
Modules/
└── Bidcon/
    ├── Kalkylinsiktsportalen.Bidcon/
    ├── Kalkylinsiktsportalen.Bidcon.Contracts/
    ├── Kalkylinsiktsportalen.Bidcon.Blazor/
    └── Kalkylinsiktsportalen.Bidcon.Api/
```

Alla projekt behöver inte skapas från början.

### Projekttyper och ansvar

#### `Kalkylinsiktsportalen.Bidcon`

Vanligt class library med `Microsoft.NET.Sdk`.

Innehåller modulens implementation, exempelvis:

- domänmodell
- use cases och handlers
- repositories och datalagring
- Bidcon-integration
- intern modulregistrering

Domain, Application och Infrastructure kan tills vidare vara mappar och interna namespaces i samma projekt. Separata projekt bör endast införas om en faktisk seam eller ett självständigt förändringsbehov uppstår.

Projektet ska normalt inte innehålla Razor-komponenter, endpoints eller AppShell-kod.

#### `Kalkylinsiktsportalen.Bidcon.Contracts`

Vanligt class library med `Microsoft.NET.Sdk`.

Innehåller modulens lilla externa interface mot andra moduler:

- publika kontrakt
- identifierare och resultattyper
- eventuella interfaces som andra moduler behöver anropa

Projektet skapas bara när andra moduler faktiskt behöver konsumera Bidcon. Det ska inte innehålla databasmodeller, SDK-typer eller intern implementation.

#### `Kalkylinsiktsportalen.Bidcon.Blazor`

Razor Class Library med `Microsoft.NET.Sdk.Razor` och vid behov:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

Innehåller:

- routbara Blazor-sidor
- modulens återanvändbara Blazor-komponenter
- UI-specifik registrering och metadata

Det publika UI-interfacet bör vara litet. Exempelvis bör `EstimationExplorer` kunna exponeras som en sammanhållen komponent medan interna tabeller, filter och dialoger förblir implementation.

#### `Kalkylinsiktsportalen.Bidcon.Api`

Vanligt class library med `Microsoft.NET.Sdk` och:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

Innehåller Minimal API-endpoints eller annan HTTP-adapter. Projektet behöver inte vara ett eget `Microsoft.NET.Sdk.Web`-projekt eftersom det monteras i den körbara hosten.

Detta projekt skapas först när HTTP-gränssnittet faktiskt behövs.

### Referensriktning

```text
Bidcon.Blazor ──→ Bidcon
Bidcon.Api ─────→ Bidcon
Bidcon ─────────→ Bidcon.Contracts

Projects ───────→ Bidcon.Contracts
Reports ────────→ Bidcon.Contracts
```

Följande beroenden ska undvikas:

```text
Projects ───────→ Bidcon
Projects.Blazor → Bidcon.Blazor
Bidcon ─────────→ AppShell
```

Andra moduler ska alltså bero på modulens kontrakt, inte på dess implementation eller presentationsadapter.

### Host och AppShell

Den körbara webbapplikationen förblir ett ASP.NET Core Web-projekt:

```text
App/
└── Kalkylinsiktsportalen.Web/
    ├── Program.cs
    └── AppShell/
```

AppShell äger konkreta skalytor som navigation, layout och Settings Hub. Produktmoduler bidrar endast med neutral metadata genom composition-kontrakt och ska inte referera AppShell.

Hosten komponerar valda adapters:

```csharp
builder.Services.AddBidconModule(registrationContext);
builder.Services.AddBidconBlazor(registrationContext);

var app = builder.Build();
```

Vid en framtida React-lösning kan hosten i stället eller parallellt montera API-adaptern:

```csharp
builder.Services.AddBidconModule(registrationContext);
builder.Services.AddBidconApi();

app.MapBidconEndpoints();
```

Modulens kärnbeteende behöver då inte bytas ut.

### Föreslagen första pilot

Testa upplägget på Bidcon i liten skala:

1. Skapa `Kalkylinsiktsportalen.Bidcon`.
2. Flytta modulens icke-UI-relaterade implementation dit.
3. Skapa `Kalkylinsiktsportalen.Bidcon.Blazor` som Razor Class Library.
4. Flytta exempelvis `EstimationExplorer` och Bidcon-inställningssidor dit.
5. Skapa `Bidcon.Contracts` endast för kontrakt som Projects eller Reports faktiskt behöver.
6. Vänta med `Bidcon.Api` tills ett HTTP-gränssnitt efterfrågas.

### Designprincip

> Dela projekt efter seams och utbytbara adapters, inte efter lager enbart för strukturens skull.

Målet är djupa moduler med ett litet interface, tydlig referensriktning och intern frihet att organisera implementationen utan att läcka den till andra moduler.