# Målarkitektur

## Syfte

Det här dokumentet beskriver den önskade arkitekturen för Kalkylinsiktsportalen.

Målbilden är baserad på produktinriktningen:

1. rapportportal
2. projekt- och versionshantering
3. kalkylutforskare

Blazor Server används fortsatt som första UI-host, men systemet ska byggas så att en framtida frontend i Blazor WASM eller React kan använda samma backend, kontrakt och affärslogik.

## Arkitekturprinciper

### 1. Modulär monolit först

Systemet bör utvecklas som en modulär monolit innan det eventuellt delas upp i fler deploybara delar.

Det betyder:
- en applikation kan fortfarande köra allt
- koden organiseras i tydliga moduler
- modulgränser respekteras även innan de är separata projekt
- externa integrationer och persistence ligger bakom kontrakt

### 2. Vertical slices framför horisontella lager

Repoet ska i första hand organiseras runt produktområden och användningsfall.

Varje modul får ha interna lager om det hjälper, till exempel:
- `Domain`
- `Application`
- `Infrastructure`
- `Components`

Men hela lösningen ska inte drivas av en global lagerstruktur där all domänlogik, all applikationslogik och all infrastruktur blandas per tekniskt lager.

### 3. UI är utbytbart

Blazor-komponenter ska inte vara platsen där central produktlogik bor.

UI ska främst:
- rendera data
- hantera användarinteraktion
- anropa use cases
- visa laddning, fel och resultat

Produktlogik ska kunna återanvändas av:
- Blazor Server
- Blazor WASM
- React
- framtida HTTP API
- tester och bakgrundsjobb

### 4. Snapshots är oföränderliga

Data som hämtas från Bidcon och sparas som snapshot ska behandlas som immutable.

Eventuella korrigeringar, kommentarer eller manuella justeringar ska sparas separat som ändringar ovanpå snapshotet.

En rapport renderas därför från:

```text
Snapshot + ChangeSet + View
```

Inte genom att ändra snapshotet.

### 5. Organisation är tenant-gräns

Organisationer ska vara helt isolerade från varandra.

Organisationsgränsen gäller minst:
- projekt
- estimatkopplingar
- snapshots
- views
- access
- Bidcon credentials
- rapportkonfiguration

## Kärnmodell

```text
Organization
  owns Projects
  owns Views
  owns Access
  owns BidconCredentials

Project
  owns ProjectEstimations

ProjectEstimation
  references one Bidcon estimation identity
  owns EstimationSnapshots

EstimationSnapshot
  immutable complete imported Bidcon state
  belongs to one Organization
  optionally belongs to one ProjectEstimation

SnapshotChangeSet
  stores revisions above a Snapshot
  does not mutate the Snapshot

View
  organization-owned report template
  defines sections and presentation rules
  can be used for any compatible EstimationSnapshot

ReportRender
  dynamic result of Snapshot + ChangeSet + View
  exportable to PDF
  not the source of truth
```

## Huvudmoduler

### Projects

Äger projekt- och versionsflödet.

Ansvar:
- projekt
- projektets estimat
- snapshot-versioner
- skapa ny snapshot för ett estimat
- skapa nya snapshots för alla estimat i ett projekt
- historik över sparade versioner
- koppling mellan projekt och Bidcon-estimat

Viktiga use cases:
- `CreateProject`
- `AddEstimationToProject`
- `CreateSnapshotForProjectEstimation`
- `CreateSnapshotsForProject`
- `ListProjectVersions`
- `GetSnapshotForInspection`

### Reports

Äger rapportmallar, vyer och presentation.

Ansvar:
- views som rapportmallar
- sektioner, celler och format
- formel-/presentationspipeline
- WYSIWYG för A4/PDF
- dynamisk rendering av rapporter
- view-specifika rättigheter

Viktiga use cases:
- `CreateView`
- `UpdateViewDefinition`
- `GrantViewAccess`
- `RenderViewForSnapshot`
- `ExportReportToPdf`

En `View` ska inte äga affärsändringar i snapshotdata. Den ska beskriva hur data och ändringar presenteras.

### Bidcon

Äger integrationen mot Bidcon som extern källa.

Ansvar:
- Bidcon API-klient
- autentisering mot Bidcon med organisationsbundna credentials
- import av komplett estimatdata
- mappning från Bidcon DTO till intern snapshotmodell

Viktiga use cases eller tjänster:
- `BidconClient`
- `AuthenticateBidconClient`
- `FetchCompleteEstimation`
- `MapBidconEstimationToSnapshot`

Bidcon-modulen ska inte känna till rapportmallar eller UI.

### Access

Äger portalens behörighetsmodell.

Ansvar:
- funktionella rättigheter
- view-rättigheter
- roll- och användarbaserad access
- admin-bootstrap per organisation
- översättning från identity/Graph till portalens accessmodell

Rättigheter ska kunna uttrycka separata förmågor, till exempel:
- får använda view
- får redigera view
- får administrera view-rättigheter
- får skapa snapshots
- får hantera Bidcon credentials

Om en användare får se en view ser användaren allt i den vyn. Fältmaskning inom en view är inte en initial arkitekturregel.

### Organizations

Äger tenant-kontext och organisationsbundna inställningar.

Ansvar:
- aktuell organisation
- organisationsinställningar
- tenant-isolering
- koppling till organisationens Bidcon credentials

Organisationen är den hårda gränsen mellan kunders data.

## Shared och SharedKernel

`Shared` ska inte vara defaultplats för oklar kod.

Målbilden är att dela upp nuvarande `Shared` i tydligare ansvar.

### SharedKernel

Får innehålla små, stabila begrepp som används över modulgränser.

Exempel:
- `OrganizationId`
- `UserId`
- `ProjectId`
- `ViewId`
- gemensamma resultat-/feltyper om sådana införs

SharedKernel ska vara litet och sakna produktflöden.

### Shared UI

Får innehålla generella UI-primitiver.

Exempel:
- generella dialogs
- layoutprimitiver
- inputkomponenter utan produktspecifik logik
- navigationstekniska byggblock

### Infrastructure

Får innehålla teknisk implementation som delas av flera moduler.

Exempel:
- MongoDB-registrering
- identity/Graph-adapter
- gemensam konfiguration
- tekniska integrationshjälpare

Produktnära kod ska inte ligga här bara för att den används av mer än en skärm.

## Beroenderiktningar

Rekommenderad riktning:

```text
Web
  -> Modules
  -> Shared UI

Modules
  -> SharedKernel
  -> egna kontrakt

Infrastructure
  -> modulernas kontrakt
  -> externa paket

Bidcon
  -> SharedKernel
  -> externa Bidcon API-kontrakt

Reports
  -> Projects-kontrakt för snapshots
  -> Access-kontrakt för view-rättigheter

Projects
  -> Bidcon-kontrakt för import
  -> SharedKernel
```

Moduler ska undvika att nå in i varandras interna implementationer.

Om en modul behöver något från en annan modul ska det ske via ett avsiktligt kontrakt eller ett use case.

## Frontendstrategi

### Nu: Blazor Server

Blazor Server behålls som snabbaste utvecklingsmiljö.

Komponenterna får gärna ligga kvar i Blazor-projektet under övergången, men de ska stegvis göras tunnare.

### Senare: API och utbytbar frontend

När produktmodellen stabiliserats bör use cases kunna exponeras via HTTP API.

Då kan en framtida frontend byggas med:
- Blazor WASM
- React
- annan klient

För att göra det möjligt redan nu:
- använd request/response-modeller för use cases
- undvik att applikationslogik kräver Blazor-specifika typer
- undvik att UI-komponenter pratar direkt med MongoDB eller externa API-klienter
- låt frontend anropa applikationstjänster/use cases

## Nuvarande kod som behöver få tydligare ägare

Följande områden bör på sikt flyttas eller få tydligare modulägarskap:
- `Shared/Components/Views` bör höra till `Reports`
- `Shared/DTOs/Bidcon` bör höra till `Bidcon` eller ett tydligt kontraktslager
- `Shared/Services/BidconApiAccess.cs` bör ersättas av Bidcon-modulens klient/importtjänster
- `Shared/Components/PrintPreview` bör höra till `Reports` eller en intern del av rapport-rendering
- startsidan i `Shared/Components/Pages/Home.razor` bör på sikt bli en riktig dashboard eller en explicit dev/test-yta
- Mongo stores bör ligga bakom modulägda kontrakt snarare än vara en allmän shared-samling

## Beslut som fortfarande är öppna

- Exakt hur `SnapshotChangeSet` ska modelleras.
- Om ändringar ska sparas per snapshot, per projektversion eller som en egen revisionsström.
- När lösningen ska delas i flera projekt.
- Om WYSIWYG/PDF ska vara en intern del av `Reports` eller en egen renderingmodul.
- Hur mycket av current `IFeature`-mönstret som ska behållas när modulerna blir tydligare.

## Rekommenderad slutsats

Bygg vidare som Blazor Server nu, men strukturera koden som en modulär monolit med tydliga use cases och kontrakt.

Det ger snabb utveckling idag och en rimlig väg till Blazor WASM, React eller annan frontend senare.
