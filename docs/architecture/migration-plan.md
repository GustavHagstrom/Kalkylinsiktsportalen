# Migrationsplan mot målarkitektur

## Syfte

Det här dokumentet beskriver hur repoet stegvis kan flyttas från nuvarande struktur till målarkitekturen i [target-architecture.md](target-architecture.md).

Målet är att hitta rätt plats för kod, minska `Shared` som default-katalog och göra systemet redo för en framtida frontend utanför Blazor Server.

Det här dokumentet äger migreringssteg, sequencing och definition of done för migreringen. Det äger inte den detaljerade domänmodellen eller modulpolicyer. Sådant ska beskrivas i [../domain/README.md](../domain/README.md), [../domain/projects.md](../domain/projects.md), [../domain/reports.md](../domain/reports.md) och relevanta ADR:er.

## Grundstrategi

Migreringen bör göras i små, verifierbara steg.

Prioriteten är:

1. etablera modulgränser
2. flytta ny kod till rätt modul från början
3. flytta befintlig kod först när den ändå berörs eller blockerar
4. införa use cases mellan UI och produktlogik
5. senare dela upp i fler projekt eller API när gränserna visat sig stabila

## Fas 1: Dokumentera Och Namnge Målstrukturen

Status: genomförd.

Åtgärder:

- lägg till målarkitekturdokument
- lägg till migrationsplan
- uppdatera dokumentationsindex
- komplettera feature-/moduldokument med den nya målbilden

Definition of done:

- utvecklare och agenter kan se vilken riktning repoet ska röra sig mot
- nya ändringar kan bedömas mot målmodulerna

## Fas 2: Inför Modulmappar Utan Att Flytta Allt

Status: genomförd.

Rekommenderad struktur i Blazor-projektet:

```text
src/BlazorServer/
  Modules/
    Projects/
    Reports/
    Bidcon/
    Access/
    Organizations/
  Shared/
    UI/
    Kernel/
    Infrastructure/
```

Detta kan först göras inom samma projekt för att undvika ett stort lösningsbyte.

Regel:

- ny produktkod ska läggas i `Modules/*`
- ny generisk UI-kod kan läggas i `Shared/UI`
- nya små gemensamma värdeobjekt kan läggas i `Shared/Kernel`
- ny teknisk integration som inte är modulunik kan läggas i `Shared/Infrastructure`

Definition of done:

- det finns en fysisk plats för målmodulerna
- ny kod behöver inte hamna i gamla `Shared`
- befintlig kod har inte flyttats som del av fasen

## Fas 3: Etablera Use Cases För Kärnflödet

Status: påbörjad.

Det första modulära registreringsmönstret finns nu på plats. Nya målmoduler ska registreras via `IModule` och `ModuleRegistration`, medan äldre `Features/*` kan fortsätta använda `IFeature` under migrationen.

Exempel:

```csharp
public sealed class ProjectsModule : IModule
{
    public void Register(ModuleRegistration module)
    {
        module.Services.AddSingleton<IProjectStore, InMemoryProjectStore>();
        module.Services.AddTransient<CreateProjectCommandHandler>();
    }
}
```

`ConfigureFeatures(...)` och `ConfigureModules(...)` körs bredvid varandra men bidrar till samma runtime-context för UI, access claims och authorization policies.

Första use cases bör spegla kärnresan:

```text
Projects/CreateProject
Projects/AddEstimationToProject
Projects/CreateSnapshotForProjectEstimation
Reports/RenderViewForSnapshot
Reports/UpdateViewDefinition
Access/GrantViewAccess
Bidcon/FetchCompleteEstimation
Projects/BuildRenderableSnapshotState
```

Blazor-komponenter ska anropa dessa use cases i stället för att själva orkestrera stores, Bidcon-klienter och mappning.

Definition of done:

- minst ett kärnflöde går via use case-lager
- berörd målmodul registreras via `IModule`
- UI-komponenten är tunnare
- flödet kan testas utan Blazor-rendering

## Fas 4: Ge Reports-Modulen Ägarskap Över Views Och Rendering

Status: hög arkitektonisk nytta.

Kandidater att flytta eller kapsla in:

- `Shared/Components/Views`
- `Shared/Components/PrintPreview`
- view-DTO:er
- view-store-kontrakt
- formelutvärdering
- WYSIWYG/PDF-flöde

Målet är inte bara att flytta filer, utan att göra Reports ansvar tydligt.

Ansvarsgränser, invariants och policyer för `View`, `EnableChanges`, duplicering, arkivering och export ska beskrivas i [../domain/reports.md](../domain/reports.md) och relevanta Reports-ADR:er, inte i denna plan.

Definition of done:

- view-/rapportlogik ligger inte längre som oägd shared-kod
- Reports har tydliga use cases för redigering och rendering
- Reports ansvar är tydligt avgränsat mot Projects
- befintliga tester för formelutvärdering pekar mot Reports

## Fas 5: Ge Bidcon-Modulen Ägarskap Över Integration Och Import

Status: bör göras innan snapshotflödet byggs ut.

Kandidater att flytta eller ersätta:

- `Shared/Services/BidconApiAccess.cs`
- `Shared/DTOs/Bidcon`
- `Shared/Models/Bidcon`
- `IBidconApiCredentialsStore`
- `MongoBidconApiCredentialsStore`

Mål:

- Bidcon-modulen hämtar komplett estimatdata
- Projects-modulen använder Bidcon via kontrakt
- credentials är organisationsbundna
- Bidcon-modulen känner inte till Reports eller UI

Definition of done:

- Bidcon-anrop sker via modulens klient/importtjänst
- tokenhantering, credentials och felhantering är samlade
- snapshotflödet kan använda Bidcon utan att känna till HTTP-detaljer

## Fas 6: Inför Projects-Modulen Som Ny Produktkärna

Status: central för MVP-riktningen.

Denna fas ska etablera Projects som ägare av projekt, versioner, snapshots och aktiv renderbar kalkylstate.

Kärnflöden som bör bevisas:

```text
1. användaren väljer ett Bidcon-estimat
2. användaren kopplar estimatet till ett projekt
3. användaren skapar snapshot för estimatet
4. snapshot sparas immutable
5. användaren väljer view för att inspektera snapshotet
```

Första vertikala slice som bör bevisa samarbetet mellan Projects och Reports:

```text
1. användaren öppnar en projektversion
2. användaren öppnar ett snapshot i projektversionen
3. användaren väljer en view
4. systemet bygger aktiv renderbar state från snapshot och view-scope
5. användaren gör en tillåten ändring i arbetsläget
6. Reports renderar vyn med Projects aktiva state
7. användaren kan exportera det aktiva läget
```

Första slicen får gärna börja med ett sparat eller seedat `EstimationSnapshot` för att bevisa modulgränserna utan att samtidigt lösa hela kedjan för extern import, autentisering och API-fel.

Första slicen bör återanvända befintlig view-rendering och formelutvärdering så långt det går. Ny kod bör framför allt lägga `Projects/BuildRenderableSnapshotState` framför renderingen, så att befintlig Reports/view-motor kan konsumera ett aktivt snapshotläge i stället för att en parallell renderingsmotor byggs.

Designen för aktiv renderbar state och ansvarsfördelningen mellan Projects och Reports ska följa:

- [../domain/projects.md](../domain/projects.md)
- [../domain/reports.md](../domain/reports.md)
- [../adr/global/0018-split-active-state-building-from-report-rendering.md](../adr/global/0018-split-active-state-building-from-report-rendering.md)
- [../adr/global/0019-pass-view-change-settings-to-projects.md](../adr/global/0019-pass-view-change-settings-to-projects.md)

Policyer för snapshots, ersättningsval, `ViewScopedChangeSet`, optimistic concurrency, `EnableChanges`, duplicering, arkivering, PDF-export och MVP-begränsningar ska inte upprepas här. De ägs av domändokumenten och ADR:erna ovan.

Definition of done:

- snapshot är separat från live-Bidcon-data
- snapshot kan sparas och hämtas
- Projects äger aktiv renderbar kalkylstate för ett valt arbetsläge
- Projects och Reports har en tydlig kontraktsgräns
- minst ett snapshot kan renderas via en view
- första slicen bevisar modulgränsen utan att bygga en parallell renderingsmotor

## Fas 7: Förtydliga Access

Status: bör gå parallellt med Reports och Projects.

Access ska hantera:

- funktionsrättigheter
- view-rättigheter
- admin-bootstrap
- organisationsisolering

Rättigheter för views bör vara separata:

- läsa/använda view
- redigera view
- administrera view-rättigheter

Definition of done:

- view-access kan uttryckas separat från generell feature-access
- policies och claims är dokumenterade per modul
- adminrollen har definierad roll i tenant-bootstrap

## Fas 8: Krymp Gamla Shared

Status: kontinuerligt arbete.

Varje gång en fil i gamla `Shared` rörs bör den bedömas:

```text
Är detta produktlogik?
  Ja -> flytta mot relevant modul.

Är detta generisk UI?
  Ja -> Shared/UI.

Är detta litet stabilt domänbegrepp?
  Ja -> Shared/Kernel.

Är detta teknisk infrastruktur?
  Ja -> Shared/Infrastructure eller modulens Infrastructure.
```

Definition of done:

- `Shared` används inte längre som slaskyta
- nya filer har tydlig ägare
- modulgränser blir lättare att förstå vid code review
- gamla registreringsadapters som `RenderCollection` används inte som primär yta för nya moduler

## Fas 9: Förbered API Och Framtida Frontend

Status: efter att use cases och modulgränser stabiliserats.

Åtgärder:

- definiera request/response-kontrakt för centrala use cases
- undvik Blazor-specifika typer i use cases
- introducera controller/minimal API för utvalda flöden
- låt Blazor Server fortsätta anropa application handlers direkt tills en faktisk API-klient eller migrationsövning behöver HTTP-kontrakt

Mål:

- Blazor Server kan ersättas eller kompletteras av Blazor WASM/React utan att skriva om kärnan
- backend äger auth, Bidcon credentials, Mongo och snapshotimport
- frontend äger rendering och interaktion, medan HTTP API senare blir en adapter över samma use cases

Definition of done:

- minst ett centralt flöde kan nås via HTTP API
- kontrakten är frontend-oberoende
- Blazor Server är en klient till samma applikationslogik via handlers, inte ägare av den

## Föreslagen Arbetsordning

1. Lägg till målarkitektur och migrationsplan.
2. Skapa `Modules`-struktur i Blazor-projektet.
3. Flytta ny rapport-/view-kod till `Modules/Reports`.
4. Flytta ny Bidcon-importkod till `Modules/Bidcon`.
5. Bygg första `Projects`-use caset för snapshot av enskilt estimat.
6. Rendera snapshot via befintlig view-motor.
7. Flytta view-motorn till Reports när den ändå berörs.
8. Inför view-rättigheter i Access.
9. Krymp gamla `Shared` stegvis.
10. Förbered HTTP API när use cases är stabila.

## Vad Som Inte Bör Göras Först

Undvik att börja med:

- total filflytt av hela repoet
- byte från Blazor Server till annan frontend
- flera deploybara tjänster
- en generell pluginarkitektur
- global lagerarkitektur över hela lösningen
- automatisk återanvändning eller förslag av ersättningsval mellan projektversioner

Det viktigaste är att etablera rätt produktmoduler och låta koden börja växa åt rätt håll.
