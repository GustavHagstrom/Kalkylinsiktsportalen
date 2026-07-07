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