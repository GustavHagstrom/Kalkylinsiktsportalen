# Begreppslista

Det här dokumentet samlar arbetsdefinitioner för centrala begrepp i domänen.

Definitionerna är baserade på nuvarande kod och dokumentation och bör förfinas över tid.

## Bidcon
Det externa system som portalen hämtar kalkyl- och estimationsdata från.

## Bidcon API
Det API som används för att autentisera och läsa data från Bidcon.

## Organisation
Den kontext som data, rättigheter, vyer och externa credentials verkar vara knutna till.

Organisationen fungerar som tenant-/avgränsningsnivå i systemet.

## Användare
En identifierad person i systemet, typiskt hämtad via Microsoft Identity / Graph.

En användare kan ha roller och direkta rättigheter i portalen.

## Roll
En gruppering av användare som används i identitets- eller åtkomstmodellen.

Roller verkar användas tillsammans med portalens egna accessregler.

## Access claim
Ett funktionellt rättighetsvärde i portalen som styr vad en användare eller roll får göra.

Exempel från nuvarande kod är claims för:
- rättighetsadministration
- Bidcon API-inställningar
- view-/mallredigering
- användning av kalkylutforskaren

## UserAccess
Portalens modell för direkta rättigheter kopplade till en enskild användare inom en organisation.

## RoleAccess
Portalens modell för rättigheter kopplade till en roll inom en organisation.

## Bidcon API credentials
Organisationsbundna autentiseringsuppgifter som gör det möjligt att ansluta till Bidcon API.

## Estimation / kalkyl / estimat
Ett centralt affärsobjekt som representerar en kalkyl eller ett estimat från Bidcon.

Ett estimat verkar innehålla metadata som namn, beskrivning, kund, plats, handläggare, datum och status.

## Projekt
Ett framtida centralt domänobjekt som samlar en eller flera kalkyler från Bidcon i ett gemensamt arbetsobjekt.

Ett projekt är tänkt att vara den kontext där användare kan:
- samla relevanta kalkyler
- dela arbetet med andra
- följa historik över tid
- arbeta med versioner och jämförelser

## Projektversion
En sparad version av ett projekt vid en viss tidpunkt.

En projektversion är tänkt att representera ett visst läge för projektets kopplade kalkyler och kunna användas för historik, uppföljning och jämförelse.

## Draft / utkastversion
En ny arbetsversion av ett projekt som ännu inte nödvändigtvis är den definitiva eller publicerade senaste versionen.

Begreppet behöver förtydligas vidare, men verkar vara relevant i målbilden där användaren ska kunna hämta in nya kalkylversioner utan att tappa historik.

## View
En sparad presentations- eller rapportmall som definierar hur estimationsdata ska visas.

En view tillhör en organisation och består av sektioner.

## Rapportvy
Arbetsord för hur en `View` används i verksamheten: en anpassad vy eller rapport över kalkyldata.

## Jämförelse
Funktion eller användningsfall där två eller flera versioner av kalkylinformation ställs mot varandra för att synliggöra skillnader.

## Jämförelserapport
En rapport eller vy som visar vad som har förändrats mellan två versioner av en kalkyl eller ett projekt.

## DataSection
En sektion i en view där data presenteras i ett cell- eller tabellnära format.

## SheetSection
En sektionstyp i en view som verkar representera ett separat blad eller en särskild dokumentdel.

Den exakta affärsbetydelsen behöver förtydligas.

## PageBreakSection
En sektionstyp som representerar en sidbrytning eller utskriftsrelaterad uppdelning i rapporten.

## Data cell
En enskild informationscell i en datasektion.

En cell verkar kunna innehålla:
- råvärde
- formel
- referens till egenskap i estimat
- text- och talformat

## Rapportanpassning
Förmågan att konfigurera hur kalkylinformation visas, struktureras och formateras för olika syften.

## Kalkylutforskare
Den del av applikationen där användaren söker, filtrerar och väljer estimat från Bidcon.

## Readonly-rapport
En rapport som främst är avsedd att visas eller konsumeras, utan att användaren redigerar själva rapportstrukturen.

## Interaktiv rapport
En rapport eller vy där användaren kan arbeta mer aktivt med urval, navigation eller andra interaktiva delar i portalen.

## Mallredigering
Funktionen där en användare med rätt behörighet skapar eller ändrar en view och dess sektioner.

## Historik
Den spårbara följden av tidigare versioner av ett projekt eller dess kopplade kalkyler.

Historiken är viktig för att kunna se hur underlaget har förändrats över tid.
