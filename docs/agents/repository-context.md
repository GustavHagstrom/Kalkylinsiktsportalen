# Repository context för agenter

Det här dokumentet är tänkt att kunna återanvändas som stabil kontext när du arbetar med AI-agenter.

## Kort sammanfattning

Detta repo innehåller en **Blazor Server-applikation på .NET 10** med **MudBlazor**, **MongoDB**, feature-baserad struktur och integration mot ett externt **Bidcon API**.

## Fakta som agenten bör känna till

- Huvudprojektet är `BlazorServer`
- UI byggs med **Blazor Server**, inte MVC eller Razor Pages
- Lösningen använder **feature-baserad registrering** via `IFeature`
- Tvärgående kod ligger i `Shared/*`
- Data lagras bland annat i **MongoDB**
- Auth skiljer sig mellan development och production
- Extern integration mot Bidcon sker via tjänst och namngiven `HttpClient`
- Tester finns i `Tests/BlazorServer.Tests`

## Repo-specifika arbetssätt att föredra

- gör små, fokuserade ändringar
- håll ny funktionalitet nära berörd feature
- lägg inte saker i `Shared` i onödan
- använd befintliga mönster för registrering, rendering och access
- uppdatera dokumentation när struktur eller riktning ändras

## Saker agenten bör kontrollera innan ändring

1. Vilken feature äger problemet?
2. Är ändringen UI, state, service, store eller auth?
3. Finns redan ett etablerat mönster i repoet?
4. Kräver ändringen ny policy, claim eller registrering?
5. Behöver test eller docs uppdateras?

## Standardprompt att utgå från

Anpassa gärna detta per uppgift:

- Läs först relevanta filer och sammanfatta kort nuvarande struktur.
- Föreslå sedan en liten plan.
- Gör minsta möjliga ändring.
- Följ befintlig feature-struktur och undvik nya paket.
- Uppdatera dokumentation om ansvar, regler eller arkitektur påverkas.
- Verifiera resultatet med relevanta tester eller annan lämplig validering.

## När mer kontext behövs

Om agenten börjar gissa bör du hellre ge:
- vilka filer som sannolikt berörs
- vilka begränsningar som gäller
- hur lösningen ska verifieras
- vad som absolut inte får ändras
