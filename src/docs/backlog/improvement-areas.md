# Förbättringsområden och åtgärdslista

Detta dokument sammanfattar identifierade gap mellan projektets nuvarande implementation och de ramar/principer som dokumenterats i repoet.

Syftet är att ge en konkret lista över vad som bör förbättras, i vilken ordning och varför.

## Prioritering

Prioriteringsnivåer:
- **Hög** = bör åtgärdas tidigt
- **Medel** = viktigt men inte akut
- **Låg** = förbättrar tydlighet och långsiktig kvalitet

## Förhållande till MVP

Utöver vanlig prioritet markeras varje punkt också med ett MVP-beslut:
- **Före MVP** = bör åtgärdas innan MVP färdigställs
- **Under MVP vid behov** = åtgärdas bara om punkten aktivt blockerar eller bromsar MVP-arbetet
- **Efter MVP** = kan medvetet vänta tills kärnflödet är levererat

---

## 1. Flytta bort hemligheter från versionshanterad konfiguration
**Prioritet:** Hög
**MVP-beslut:** Före MVP

### Problem
`BlazorServer/appsettings.json` innehåller en `ClientSecret`.

### Varför detta bör åtgärdas
- går emot repoets önskade riktning för säker konfiguration
- ökar risken att känslig information lever kvar i historiken
- gör miljöhantering mindre tydlig

### Rekommenderad åtgärd
- flytta secrets till User Secrets eller miljövariabler
- dokumentera hur lokal konfiguration ska ske
- se över om känslig historik behöver roteras/städas

### Definition of done
- inga hemligheter ligger i versionshanterad `appsettings.json`
- lokal utveckling fungerar med dokumenterad setup

---

## 2. Låt features äga sina routes konsekvent
**Prioritet:** Hög
**MVP-beslut:** Under MVP vid behov

### Problem
Routes verkar finnas både i feature-konstanter och hårdkodade direkt i komponenter.
Det finns också inkonsekvens i casing och route-definitioner.

### Exempel
- `Features/ViewSettings/FeatureConstants.cs`
- `Features/ViewSettings/Components/EditViewPage.razor`
- `Features/Access/FeatureConstants.cs`
- `Features/Access/Components/EditUserAccessPage.razor`

### Varför detta bör åtgärdas
- försvårar underhåll
- gör det lätt att skapa brutna länkar eller inkonsekvent navigation
- bryter mot idén att en feature ska äga sitt kontrakt centralt

### Rekommenderad åtgärd
- definiera feature-routes i respektive feature-konstanter
- använd samma route-definitioner konsekvent i komponenter och navigation
- dokumentera vilka features som äger vilka routes

### Definition of done
- feature-specifika routes är centraliserade och konsekventa
- casing och namngivning är enhetliga

---

## 3. Minska feature-specifik kod i `Shared`
**Prioritet:** Hög
**MVP-beslut:** Efter MVP

### Problem
Kod som verkar tillhöra specifika affärsområden ligger i `Shared`, särskilt under `Shared/Components/Views`.

### Exempel
- `Shared/Components/Views/ViewEditor.cs`
- `Shared/Components/Views/DataCellFormulaEvaluator.cs`
- sannolikt fler filer i samma område

### Varför detta bör åtgärdas
- `Shared` blir snabbt en default-katalog i stället för ett medvetet tvärgående lager
- ansvar blir otydligare
- agentarbete och vidareutveckling blir svårare

### Rekommenderad åtgärd
- flytta view-/rapport-specifik logik närmare rätt feature
- behåll bara verkligt tvärgående delar i `Shared`
- överväg tydligare underkataloger om vissa delar behöver delas men inte är generella UI-primitiver

### Definition of done
- feature-specifik logik ligger i första hand i feature-nära struktur
- `Shared` används främst för verkligt tvärgående ansvar

---

## 4. Tunnare Blazor-komponenter
**Prioritet:** Hög
**MVP-beslut:** Under MVP vid behov

### Problem
Vissa komponenter innehåller relativt mycket orchestration, integrationslogik och dataladdning.

### Exempel
- `Features/BidconBrowser/Components/BrowserPage.razor`
- `Features/Access/Components/EditUserAccessPage.razor`

### Varför detta bör åtgärdas
- komponenter blir svårare att testa
- UI, state och integrationslogik blandas
- återanvändning och felsökning försvåras

### Rekommenderad åtgärd
- flytta orchestration till feature services eller state-objekt
- låt komponenter fokusera mer på rendering och enkel interaktion
- bryt ut mappning och laddningsflöden från UI där det går

### Definition of done
- tung logik ligger inte direkt i `.razor`-komponenter utan i tjänster/state där det är rimligt
- UI-komponenter har tydligare ansvar

---

## 5. Rensa DI-/registreringsmönster som blandar registrering och upplösning
**Prioritet:** Medel
**MVP-beslut:** Efter MVP

### Problem
`FeatureRegistrator` bygger en service provider under registrering.

### Exempel
- `Shared/Registration/FeatureRegistrator.cs`

### Varför detta bör åtgärdas
- skapar ett mindre rent DI-upplägg
- kan ge överraskningar kring livscykler och registrering
- blandar komposition med upplösning av instanser

### Rekommenderad åtgärd
- se över hur features registreras utan temporär service provider
- håll kompositionsroten enkel och tydlig

### Definition of done
- feature-registrering sker utan att bygga separat service provider under registrering

---

## 6. Bygg ut testytan för affärskritisk logik
**Prioritet:** Medel
**MVP-beslut:** Före MVP för MVP-kritisk logik, annars Efter MVP

### Problem
Nuvarande testtäckning verkar vara smal och fokuserad kring `DataCellFormulaEvaluator`.

### Varför detta bör åtgärdas
- flera viktiga områden saknar tydlig regressionssäkring
- repoets egna riktlinjer pekar ut fler prioriterade testytor

### Rekommenderade första kandidater
- authorization policies i features
- mappningar mot Mongo-entiteter
- view-logik som idag ligger i editor-/helper-klasser
- access- och rättighetsrelaterad logik

### Definition of done
- testprojektet täcker fler centrala logikområden än enbart formelutvärdering
- ny affärslogik får tester som standard

---

## 7. Förtydliga repoets gränser och projektansvar
**Prioritet:** Medel
**MVP-beslut:** Efter MVP

### Problem
`PwCracker` sticker ut från repoets huvudsakliga fokus.

### Varför detta bör åtgärdas
- gör repoets syfte mindre tydligt
- ökar risken för blandade ansvar i samma lösning

### Rekommenderad åtgärd
- besluta om `PwCracker` ska vara kvar
- om ja: dokumentera varför
- om nej: flytta ut eller separera tydligare

### Definition of done
- det finns ett tydligt beslut om varför projektet finns kvar eller inte

---

## 8. Städa naming och stavningsinkonsekvenser
**Prioritet:** Medel
**MVP-beslut:** Efter MVP

### Problem
Det finns flera namn/stavningar som gör koden mindre tydlig.

### Exempel
- `IFreature`
- `Enteties`
- `AppNavMenue`
- `HasThoasandsSeparator`

### Varför detta bör åtgärdas
- sänker läsbarhet
- försvårar sökning, onboarding och agentarbete
- skapar onödig friktion i koden

### Rekommenderad åtgärd
- gör små, säkra namnförbättringar stegvis
- undvik stor byta-allt-refaktorering om det inte behövs

### Definition of done
- centrala typer och mappar har konsekventa och korrekta namn

---

## 9. Dokumentera ägarskap för features, policies och routes tydligare
**Prioritet:** Låg
**MVP-beslut:** Efter MVP

### Problem
Arkitekturen är relativt tydlig, men ägarskapet är ännu inte fullt dokumenterat.

### Varför detta bör åtgärdas
- gör det lättare att avgöra var ny kod ska läggas
- gör agentprompter mer träffsäkra
- minskar risken att `Shared` växer felaktigt

### Rekommenderad åtgärd
- komplettera docs med en tabell över features och deras:
  - routes
  - policies
  - claims
  - centrala tjänster/stores

### Definition of done
- det går snabbt att se vilken feature som äger vilket ansvar

---

## Rekommenderad ordning att jobba i

1. Flytta bort secrets från repo-konfiguration
2. Bygg ut tester för MVP-kritisk logik
3. Centralisera och städa routes där de blockerar eller förvirrar MVP-arbetet
4. Tunna ut komponenter bara där det behövs för att kunna leverera MVP säkert
5. Ta övriga strukturförbättringar efter MVP i små steg

## Snabb överblick

### Bör gå före MVP
- 1. Flytta bort hemligheter från versionshanterad konfiguration
- 6. Bygg ut tester för MVP-kritisk logik

### Gör under MVP endast om det blockerar
- 2. Låt features äga sina routes konsekvent
- 4. Tunnare Blazor-komponenter

### Kan vänta till efter MVP
- 3. Minska feature-specifik kod i `Shared`
- 5. Rensa DI-/registreringsmönster som blandar registrering och upplösning
- 7. Förtydliga repoets gränser och projektansvar
- 8. Städa naming och stavningsinkonsekvenser
- 9. Dokumentera ägarskap för features, policies och routes tydligare

---

## Arbetsprincip

Arbeta helst i små steg.

För varje punkt:
1. välj ett avgränsat delproblem
2. uppdatera kod
3. verifiera beteendet
4. uppdatera dokumentationen om struktur eller ansvar ändrats

På så sätt kan projektet successivt närma sig de ramar som nu dokumenterats utan att kräva en stor och riskabel totalrefaktorering.
