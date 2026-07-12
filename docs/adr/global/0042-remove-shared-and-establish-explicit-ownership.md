# Remove Shared and establish explicit ownership

Status: accepted

`Shared` hade blivit en blandning av produktlogik, UI, autentisering, persistence och teknisk infrastruktur. Vi avvecklar därför `Shared` helt och placerar all production-kod hos en explicit ägare: `Kernel`, `AppShell`, `UiKit`, top-level `Infrastructure` eller en produktmodul.

`Kernel` ska vara minimal och teknikneutral. `AppShell` äger host-, session- och compositionansvar. `UiKit` innehåller neutrala UI-primitiver. Top-level `Infrastructure` innehåller endast neutral teknisk infrastruktur. Produktlogik, modulnära persistence, integrationsmodeller och feature-UI ägs av respektive modul.

Beslutet innebär att ingen production-kod får ligga i en `Shared`-katalog eller använda namespace `BlazorServer.Shared`. Cross-module-interaktion ska ske genom små, avsiktliga kontrakt under den ägande modulens `Public`-yta.