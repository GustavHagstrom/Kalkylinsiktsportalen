# Migrationsplan mot målarkitektur

## Syfte

Det här dokumentet beskriver hur repoet stegvis kan flyttas från nuvarande struktur till målarkitekturen i [target-architecture.md](target-architecture.md).

Målet är att hitta rätt plats för kod, minska `Shared` som default-katalog och göra systemet redo för en framtida frontend utanför Blazor Server.

## Grundstrategi

Migreringen bör göras i små, verifierbara steg.

Prioriteten är:

1. etablera modulgränser
2. flytta ny kod till rätt modul från början
3. flytta befintlig kod först när den ändå berörs eller blockerar
4. införa use cases mellan UI och produktlogik
5. senare dela upp i fler projekt eller API när gränserna visat sig stabila

## Fas 1: Dokumentera och namnge målstrukturen

Status: kan göras direkt.

Åtgärder:
- lägg till målarkitekturdokument
- lägg till migrationsplan
- uppdatera dokumentationsindex
- komplettera feature-/moduldokument med den nya målbilden

Definition of done:
- utvecklare och agenter kan se vilken riktning repoet ska röra sig mot
- nya ändringar kan bedömas mot målmodulerna

## Fas 2: Inför modulmappar utan att flytta allt

Status: nästa praktiska kodsteg.

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

## Fas 3: Etablera use cases för kärnflödet

Status: bör göras innan större produktutbyggnad.

Första use cases bör spegla kärnresan:

```text
Projects/CreateProject
Projects/AddEstimationToProject
Projects/CreateSnapshotForProjectEstimation
Reports/RenderViewForSnapshot
Reports/UpdateViewDefinition
Access/GrantViewAccess
Bidcon/FetchCompleteEstimation
```

Blazor-komponenter ska anropa dessa use cases i stället för att själva orkestrera stores, Bidcon-klienter och mappning.

Definition of done:
- minst ett kärnflöde går via use case-lager
- UI-komponenten är tunnare
- flödet kan testas utan Blazor-rendering

## Fas 4: Ge Reports-modulen ägarskap över views och rendering

Status: hög arkitektonisk nytta.

Kandidater att flytta eller kapsla in:
- `Shared/Components/Views`
- `Shared/Components/PrintPreview`
- view-DTO:er
- view-store-kontrakt
- formelutvärdering
- WYSIWYG/PDF-flöde

Målet är inte bara att flytta filer, utan att göra Reports ansvar tydligt:

```text
ViewDefinition
ViewSection
ReportRenderModel
FormulaEvaluation
PdfExport
```

Definition of done:
- view-/rapportlogik ligger inte längre som oägd shared-kod
- Reports har tydliga use cases för redigering och rendering
- befintliga tester för formelutvärdering pekar mot Reports

## Fas 5: Ge Bidcon-modulen ägarskap över integration och import

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

## Fas 6: Inför Projects-modulen som ny produktkärna

Status: central för MVP-riktningen.

Nya begrepp:
- `Project`
- `ProjectEstimation`
- `EstimationSnapshot`
- `SnapshotChangeSet`

Första viktiga flödet:

```text
1. användaren väljer ett Bidcon-estimat
2. användaren kopplar estimatet till ett projekt
3. användaren skapar snapshot för estimatet
4. snapshot sparas immutable
5. användaren väljer view för att inspektera snapshotet
```

Definition of done:
- snapshot är separat från live-Bidcon-data
- snapshot kan sparas och hämtas
- minst ett snapshot kan renderas via en view

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

## Fas 8: Krymp gamla Shared

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

## Fas 9: Förbered API och framtida frontend

Status: efter att use cases och modulgränser stabiliserats.

Åtgärder:
- definiera request/response-kontrakt för centrala use cases
- undvik Blazor-specifika typer i use cases
- introducera controller/minimal API för utvalda flöden
- låt Blazor Server själv använda samma kontrakt som en extern frontend skulle använda där det är rimligt

Mål:
- Blazor Server kan ersättas eller kompletteras av Blazor WASM/React utan att skriva om kärnan
- backend äger auth, Bidcon credentials, Mongo och snapshotimport
- frontend äger rendering och interaktion

Definition of done:
- minst ett centralt flöde kan nås via HTTP API
- kontrakten är frontend-oberoende
- Blazor Server är en klient till samma applikationslogik, inte ägare av den

## Föreslagen arbetsordning

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

## Vad som inte bör göras först

Undvik att börja med:
- total filflytt av hela repoet
- byte från Blazor Server till annan frontend
- flera deploybara tjänster
- en generell pluginarkitektur
- global lagerarkitektur över hela lösningen

Det viktigaste är att etablera rätt produktmoduler och låta koden börja växa åt rätt håll.
