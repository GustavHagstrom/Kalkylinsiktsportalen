# Mall för uppgifter till agenter

Använd mallen nedan när du vill få mer konsekventa svar och ändringar.

## Kort mall

### Mål
Beskriv exakt vad som ska åstadkommas.

Exempel:
- Lägg till filtrering i BidconBrowser för estimatnamn.
- Fixa felaktig formattering i DataCellFormulaEvaluator.
- Lägg till en settings-sida för X.

### Kontext
Beskriv var i lösningen arbetet hör hemma.

Exempel:
- Detta gäller `BlazorServer`.
- Berörd feature är `BidconBrowser`.
- Följ befintligt mönster med `IFeature` och MudBlazor.

### Begränsningar
Lista sådant som inte ska ändras.

Exempel:
- Lägg inte till nya paket.
- Behåll existerande routes.
- Flytta inte kod mellan features utan tydligt skäl.
- Ändra inte auth-flödet.

### Acceptanskriterier
Gör klart när uppgiften räknas som färdig.

Exempel:
- Användaren kan filtrera listan klientside.
- Existerande beteende för valda estimat fortsätter fungera.
- Relevanta tester passerar.

### Verifiering
Säg hur agenten ska kontrollera resultatet.

Exempel:
- Kör relevanta tester.
- Kontrollera att lösningen bygger.
- Sammanfatta vilka filer som ändrades och varför.

## Exempelprompt för detta repo

Läs först relevanta filer i `BlazorServer` och sammanfatta kort hur den berörda featuren fungerar idag.
Föreslå sedan en liten plan.
Implementera därefter minsta möjliga ändring.
Följ befintlig feature-struktur, MudBlazor-mönster och registrering via `IFeature` där det är relevant.
Lägg inte till nya paket.
Uppdatera dokumentation om struktur eller arbetssätt påverkas.
Verifiera med relevanta tester eller annan lämplig validering.

## Tips

En bra uppgift till en agent innehåller nästan alltid:
- mål
- scope
- begränsningar
- definition of done
- verifieringssteg
