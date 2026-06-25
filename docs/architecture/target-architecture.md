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

Inom varje modul är feature/use case-slices den primära arbetsformen. Modulen är ägarskapsgränsen, och varje större användningsfall kan samla request/response-modeller, handler/service, validering, mappning och tester nära varandra.

Moduler får samtidigt ha interna mappar som `Domain`, `Infrastructure` och `Components` när det finns ett konkret behov:
- `Domain` för stabila modulbegrepp och regler
- `Infrastructure` för modulunik persistence, externa klienter och tekniska adapters
- `Components` för modulnära UI som inte är generell shared UI

Skapa inte tomma lager bara för symmetri. Clean architecture används som riktning för beroenden, medan vertical slices används som praktisk struktur för ny produktlogik.

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

Eventuella ändringar ska sparas separat ovanpå snapshotet. På sikt är det viktigaste värdet inte fria kommentarer, utan styrda byten av kalkylposter för användare som inte har kalkylbehörighet i Bidcon.

Vilka kalkylposter som får bytas styrs av definierade taggar i Bidcons `Remarks`/`Remark`-fält vid importen. Portalen får bara erbjuda ändringar där snapshotets importerade Bidcon-data uttryckligen markerar att posten är bytbar.

Initial ersättningspolicy ska följa den befintliga portalens regel:
- en tagg skrivs i `Remark` med klammerformat, till exempel `{minTagg}`
- taggens värde är texten mellan klamrarna, till exempel `minTagg`
- `Remark` kan innehålla flera klammer-taggar, till exempel `{kyla}{premium}`
- initialt räknas alla klammer-taggar i `Remark` som ersättningstaggar för kompatibilitet med befintlig portal
- kalkylrader kan bara ersätta varandra om de ligger på samma nivå i Bidcon-trädet och har minst en gemensam tagg
- initial policy kräver inte samma tekniska radtyp/posttyp utöver samma trädnivå och gemensam tagg
- originalradens mängd appliceras på ersättningsraden
- användaren kan därför byta exempelvis `1 st Kyl A` till `1 st Kyl B`, men inte samtidigt ändra mängden till `2 st`

Taggsyntaxen ska kapslas i en parser, till exempel `ReplacementTagParser`, så att framtida syntax som `{replace:kyla}` eller `{kip:replace=kyla}` kan införas utan att ändra hela modellen.

Regeln ska modelleras som en ersättningspolicy, inte som spridd UI-logik, så att den kan bytas eller utökas senare utan att ändra hela snapshot- och renderingsmodellen.

En rapport renderas därför från:

```text
Snapshot + View + ViewScopedChangeSet
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
  owns ProjectVersions

ProjectEstimation
  references one Bidcon estimation identity
  owns EstimationSnapshots

EstimationSnapshot
  immutable complete imported Bidcon state
  belongs to one Organization
  optionally belongs to one ProjectEstimation

ViewScopedChangeSet
  belongs to one View
  belongs to one ProjectVersion
  belongs to one EstimationSnapshot
  is unique per View + ProjectVersion + EstimationSnapshot
  applies to one version/snapshot state at a time
  is shared by users of that View and version/snapshot state
  uses optimistic concurrency for shared replacement changes
  stores controlled revisions above that Snapshot
  can replace tagged replaceable estimation rows/items
  stores replacement selections, not materialized calculation results
  has at most one active replacement selection per original row/item
  does not mutate the Snapshot

ReplacementSelection
  references original estimation row/item
  references replacement estimation row/item
  stores the replacement tag used as the match reason
  stores original quantity at selection time
  stores policy version used to validate the selection
  stores ChangedByUserId, ChangedByDisplayName, ChangedByEmail and ChangedAt for the active selection
  has a version/concurrency token so concurrent edits to the same original row can be detected
  is validated against the snapshot it belongs to
  is not automatically carried forward to later imports

ReplacementPolicy
  owned by Projects
  determines allowed replacements between estimation rows/items
  initial rule uses same tree level and at least one shared Remark tag value
  does not require the same technical row/item type
  MVP requires enough data for safe rendering, such as quantity and unit price
  records the selected or deterministic shared tag as the match reason
  applies original quantity to the replacement item

ReplacementTagParser
  extracts replacement tags from Bidcon Remark text
  initial syntax treats every bracket tag as a replacement tag

ProjectVersion
  saved project state
  references selected EstimationSnapshots
  does not own snapshot data or snapshot changes

View
  organization-owned report template
  defines sections and presentation rules
  can enable or disable view-local changes
  can be used for any compatible EstimationSnapshot
  can be duplicated without copying ViewScopedChangeSet state
  should be archived rather than hard-deleted when it has history or ViewScopedChangeSet state
  archived views can render and export historical versions read-only

ReportRender
  dynamic result of Snapshot + View + ViewScopedChangeSet
  exportable to PDF
  not the source of truth

RenderableSnapshotState
  active calculation state produced by Projects
  general row/tree model, not a document component model
  includes rows/items, hierarchy, quantities, values, Remark tags and replacement status
  replaced rows expose both original row and active replacement row

RenderableRow
  contains original row/item
  contains active row/item after replacements
  preserves active replacement row's original quantity
  exposes RenderedQuantity, using original row quantity when replaced
  recalculates renderable totals from RenderedQuantity where unit values are explicit
  preserves or exposes active row values when recalculation rules are not explicit
  exposes replacement metadata when replaced
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
- `ReplaceSnapshotEstimationItem`
- `BuildRenderableSnapshotState`

Projects owns `ViewScopedChangeSet` and the replacement policy for controlled estimation item replacements.
Reports owns `View` and `View.EnableChanges`; Projects owns the resulting view-scoped changes and validates them against snapshots and replacement policy.
Projects builds the active calculation state for a View and snapshot/version by applying the relevant `ViewScopedChangeSet`.
Projects should receive only a small view change scope/settings contract, such as `ViewId` and `EnableChanges`, not the full Reports `ViewDefinition`.

### Reports

Äger rapportmallar, vyer och presentation.

Ansvar:
- views som rapportmallar
- sektioner, celler och format
- formel-/presentationspipeline
- WYSIWYG för A4/PDF
- dynamisk rendering av rapporter
- PDF-export av aktivt renderat view-läge inklusive view-lokala ändringar
- view-specifika rättigheter

Viktiga use cases:
- `CreateView`
- `UpdateViewDefinition`
- `GrantViewAccess`
- `RenderViewForSnapshot`
- `ExportReportToPdf`
- `ClearViewScopedChanges`
 
Reports may consume a renderable view/snapshot state from Projects, including allowed `ViewScopedChangeSet` changes, but must not decide which estimation items may be replaced.
Reports owns view rendering: applying `ViewDefinition`, sections, formulas, layout and PDF export to the renderable state returned by Projects.
Reports or the Web layer provides the view change scope/settings needed by Projects without exposing Reports internals.
Reports maps `RenderableSnapshotState` into document components such as `HeaderComponents`, `BodyComponents` and `FooterComponents`.

En `View` ska inte äga affärsändringar i snapshotdata. Den ska beskriva hur data och ändringar presenteras.

`View.EnableChanges` styr om användare får göra view-lokala ersättningsval när de använder vyn. Om en användare har rätt att använda en view där `EnableChanges` är aktiverat får användaren också göra de ersättningsval som vyn och Projects ersättningspolicy tillåter.
Om `EnableChanges` stängs av på en view som redan har aktiva `ViewScopedChangeSet`-ändringar ska ändringarna ligga kvar och fortsätta appliceras vid rendering/export. Flaggan styr om nya ändringar får göras, inte om befintliga ändringar räknas med i det aktiva view-läget.

View-lokala ersättningsval är gemensamma för vyn och versionen/snapshotläget, inte personliga per användare. Om en användare gör ett ersättningsval i en ändringsbar view ser andra användare med tillgång till samma view och version samma aktiva val.

Om en view dupliceras ska bara view-definitionen och `EnableChanges` kopieras. Befintliga `ViewScopedChangeSet`-ändringar ska inte följa med till den nya vyn.

Om en view har historik, exporter eller `ViewScopedChangeSet`-ändringar ska borttagning i första hand arkivera vyn i stället för att hårdradera den. Arkiverade views döljs från normal användning men kan fortfarande förklara, rendera och PDF-exportera historiska rapporter och ändringslägen i read-only-läge. Arkiverade views ska inte användas för nya ändringar eller som valbara views i nya normala flöden.

Eftersom ändringarna är delade ska sparande använda optimistisk konfliktkontroll. Om två användare ändrar samma originalrad samtidigt ska senare sparning kunna nekas och klienten ladda om aktuellt val i stället för att tyst skriva över det.

PDF-export i MVP görs från ett öppnat snapshot med en vald view inom en projektversion, inte från hela projektversionen. Exporten ska som default alltid använda det aktiva renderade view-läget för aktuell `View + ProjectVersion + EstimationSnapshot`, inklusive aktiva `ViewScopedChangeSet`-ändringar. Om ändringar inte ska vara med bör användaren först använda en separat `ClearViewScopedChanges`-funktion som rensar alla aktiva ersättningsval för den aktuella vyn, versionen och snapshotet som användaren tittar på. Initialt får alla användare som får göra ersättningsval i vyn också rensa ändringarna, men rensningen ska kräva tydlig bekräftelse. För arkiverade views använder PDF-export samma historiska read-only-läge.

MVP behöver inte spara varje PDF-export som en egen historisk artefakt. `ExportReportToPdf` kan generera filen på begäran från aktivt renderat läge.

Initialt finns ingen undo/redo-modell och `ClearViewScopedChanges` är permanent. Om undo/redo behövs senare bör det byggas som en operation log för `ViewScopedChangeSet`, med operationer som replace row, clear all, undo och redo, snarare än som specialfält för enskilda åtgärder.

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

Om en användare får se en view ser användaren allt i den vyn. Om vyn har `EnableChanges` får användaren även göra tillåtna view-lokala ersättningsval i den vyn och rensa dessa ändringar med bekräftelse. Fältmaskning inom en view är inte en initial arkitekturregel.

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
  -> Projects contract for renderable view/snapshot state with applied changes
  -> Access-kontrakt för view-rättigheter

Projects
  -> Bidcon-kontrakt för import
  -> SharedKernel
  -> owns ViewScopedChangeSet and ReplacementPolicy
  -> accepts ViewId and view change settings as contract input, not Reports internals
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

- När lösningen ska delas i flera projekt.
- Om WYSIWYG/PDF ska vara en intern del av `Reports` eller en egen renderingmodul.
- Hur mycket av current `IFeature`-mönstret som ska behållas när modulerna blir tydligare.

## Senare möjligheter

- Nya projektversioner kan på sikt få förslag baserat på ersättningsval i föregående version och samma view, men valen ska inte ärvas automatiskt. Användaren behöver bekräfta förslagen, och systemet skapar då nya `ViewScopedChangeSet`-ändringar för den nya versionen.
- Full audit trail över tidigare ersättningsval kan införas senare om verksamheten behöver se varje tidigare val. MVP behöver bara aktuell aktiv ersättning med vem/när-metadata.
- Undo/redo kan införas senare genom en operation log för `ViewScopedChangeSet`. MVP behandlar `ClearViewScopedChanges` som permanent.
- Sparade PDF-exportartefakter kan införas senare som `ReportExportRecord` med `ViewId`, `ProjectVersionId`/`SnapshotId`, `ViewScopedChangeSet`-version, exportör, exporttid och filreferens.
- Export av en hel projektversion med en vald view kan införas senare. MVP exporterar ett öppnat snapshot med vald view.

## Rekommenderad slutsats

Bygg vidare som Blazor Server nu, men strukturera koden som en modulär monolit med tydliga use cases och kontrakt.

Det ger snabb utveckling idag och en rimlig väg till Blazor WASM, React eller annan frontend senare.
