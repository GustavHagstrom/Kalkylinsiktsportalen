# UI/UX

Det här dokumentet äger repoets långlivade UI/UX-principer för Kalkylinsiktsportalen.

Syftet är att ge utvecklare och agenter en gemensam riktning för hur produktens gränssnitt ska kännas, prioriteras och byggas utan att blanda ihop designriktning med domänordlista, arkitekturbeslut eller enskilda implementationer.

## Ägarskap

UI/UX-principer som gäller över flera funktioner dokumenteras här.

Kanoniska produkt- och domäntermer ägs av [../CONTEXT.md](../CONTEXT.md). Praktiska Blazor- och kodriktlinjer ägs av [development.md](development.md). Svårbackade tekniska eller arkitektoniska trade-offs ägs av ADR:er under [adr/](adr/).

Lägg inte designprinciper i `CONTEXT.md`. Uppdatera ordlistan först när ett UI-nära produktbegrepp behöver ett stabilt kanoniskt namn, till exempel för att skilja mellan arbetsläge, preview och publicerat läge i verksamhetsspråket.

## Produktkänsla

Kalkylinsiktsportalen ska kännas som ett arbetsverktyg före en presentationsyta.

UI:t ska vara lugnt, tätt och beslutsstödjande för återkommande användning. Prioritera tydliga tabeller, filter, states, jämförelser och actions nära datan framför dekorativa ytor, marknadsföringskänsla eller stora visuella effekter.

Rapport- och viewbyggande ska kännas kontrollerat och spårbart. Användaren ska förstå vad som visas, var datan kommer från och vilka ändringar som påverkar resultatet.

## Primär Användarsituation

Designa i första hand för en återkommande yrkesanvändare som arbetar med kalkylinformation under tidspress.

UI:t ska prioritera scanning, jämförelse, filtrering, spårbarhet och snabba säkra actions. Onboarding-copy, tomma pedagogiska ytor och visuella wow-moment ska bara användas när de hjälper användaren att komma vidare i ett konkret arbetsflöde.

## Informationsdensitet

Layouten ska vara kompakt men inte trång.

Standardläget ska vara datatätt nog för scanning och jämförelse, särskilt i tabeller, listor och rapportytor. Använd tydliga radavstånd, gruppning, sticky headers eller sticky actions när det hjälper, och konsekventa tomma ytor för att skilja informationsblock.

Använd inte stora dashboard cards när datan egentligen vill vara en lista, tabell eller arbetsyta. Cards passar bäst för upprepade objekt, sammanfattningar eller avgränsade verktyg där ramen gör informationen lättare att skanna.

## Navigation Och Arbetskontext

Portalen ska ha tydliga sidor för navigation, men arbetsflöden ska behålla användarens kontext.

Användaren ska förstå var den är i produkten, samtidigt som arbete med en `Estimation`, ett `Project`, en `View` eller ett rapportläge inte ska kännas som att det börjar om vid varje sidbyte. Behåll relevant arbetskontext genom tydliga rubriker, breadcrumbs, valda objekt, filterläge och actions som ligger nära det användaren arbetar med.

Undvik både isolerade sidor där användaren tappar sitt arbetsobjekt och en enda massiv allt-i-ett-yta där modulansvar och navigation blir otydliga.

## States Och Feedback

UI:t ska vara ärligt och handlingsorienterat när läget är osäkert, tomt, laddar eller har felat.

Skilj tydligt på inget valt än, ingen data finns, data laddar, extern källa svarar inte, saknad behörighet och lägen där användaren kan göra något för att lösa problemet.

Visa inte tomma tabeller eller generiska fel när systemet vet mer. Ge kort status, nästa möjliga action och spårbarhet när datan kommer från Bidcon eller ett sparat snapshot.

## Ändringar Och Konsekvens

Vardagsinteraktioner ska vara snabba och inte drunkna i bekräftelsedialoger.

När en ändring påverkar en `View`, en delad `ReplacementSelection`, en version, export eller annan rapportgrund ska UI:t visa konsekvensen innan ändringen sparas. Det ska framgå om ändringen är personlig, delad, sparad eller bara preview.

Actions som påverkar rapportresultat ska ligga nära datan de påverkar och använda labels som beskriver effekten, inte bara den tekniska operationen.

## Redigering Och Preview

Rapport- och viewbyggande ska skilja arbetsläge från publicerat eller sparat läge.

Användaren ska kunna prova ändringar och se preview utan att direkt påverka delade rapporter. När något sparas ska UI:t tydligt visa vad som sparas, var det hör hemma, och vilket rapportläge, vilken version eller vilket snapshot det gäller.

Redigering ska kännas som att användaren förbereder och bekräftar en ändring, inte som att varje klick riskerar att ändra rapporten.

## Visuell Stil

Portalen ska vara professionell, neutral och saklig, men inte steril.

Använd en begränsad färgpalett där färg främst betyder status, risk, kategori eller interaktion. Typografi ska vara lättläst och funktionell. Komponenter ska kännas moderna men återhållsamma, med tydliga borders, bra kontrast och konsekventa hover- och focus-states.

Undvik stora gradients, dekorativa illustrationer och visuella effekter som tar plats från arbetsflödet.

## Öppna Frågor

- Vilka befintliga UI-mönster i koden ska betraktas som önskade respektive legacy?
