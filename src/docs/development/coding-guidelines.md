# Kodriktlinjer

Det här dokumentet är ett första förslag på repo-specifika riktlinjer baserat på nuvarande struktur.

## Grundprinciper

### 1. Gör minsta rimliga ändring
När du fixar ett problem, försök att:
- ändra så få filer som möjligt
- hålla dig inom rätt feature/lager
- undvika bred refaktorering om den inte behövs för uppgiften

### 2. Följ befintlig struktur
I det här repoet verkar följande vara viktigt:
- feature-baserad indelning i `Features/*`
- tvärgående kod i `Shared/*`
- registrering via `ServiceAndFeatureRegistrations`
- UI byggt med Blazor Server och MudBlazor

### 3. Shared är inte default
Ny kod ska inte automatiskt hamna i `Shared`.

Använd `Shared` först när:
- flera features faktiskt använder samma ansvar
- det finns en tydlig tvärgående abstraktion
- det skulle skapa dubbelarbete att ha separat implementation

## Riktlinjer för Blazor-komponenter

- håll komponenter fokuserade på rendering, interaktion och enkel orchestration
- flytta återanvändbar eller testbar logik till tjänster, helpers eller state-objekt
- undvik att blanda för mycket integrationslogik direkt i UI
- återanvänd befintliga MudBlazor-mönster där det är rimligt

## Riktlinjer för features

En feature bör i normalfallet äga:
- egna routes
- egna komponenter
- egen state
- egna accessregler/policies
- egen registrering via `IFeature`

När en ny feature läggs till, kontrollera att följande är tydligt:
- vad featuren ansvarar för
- vilka användare/roller/claims som ska ha åtkomst
- vilka externa beroenden som används
- hur featuren testas

## Riktlinjer för tjänster och stores

- lägg integrationskod bakom tjänster
- lägg persistence bakom stores eller tydliga kontrakt
- håll beroenden explicita via DI
- återanvänd existerande interface när det ger verkligt värde

## Riktlinjer för tester

Nuvarande testyta är begränsad, så följande är en bra miniminivå:
- lägg tester där ny logik är enkel att isolera
- prioritera tester för beräkningar, formattering, accessregler och mappningar
- undvik att låsa fast testsviten i onödig UI-detalj om logiken kan testas lägre ned

## Definition of done för kodändringar

En ändring är normalt inte klar förrän:
- lösningen kompilerar eller berörda delar verifierats
- relevant beteende är testat
- ny struktur eller nytt arbetssätt är dokumenterat
- ändringen följer feature- och lagergränserna

## Rekommenderade regler för agentarbete i detta repo

- be agenten läsa relevanta filer innan ändring
- be agenten föreslå en kort plan först
- be agenten hålla sig till existerande struktur
- be agenten undvika nya paket om det inte är nödvändigt
- be agenten uppdatera docs när arkitektur eller arbetsflöde ändras
