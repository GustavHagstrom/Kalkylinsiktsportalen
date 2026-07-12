# Limit Shared to stable cross-cutting building blocks

Status: superseded by ADR-0042

`Shared` skulle krympa över tid och endast innehålla genuint gemensamma byggblock. Erfarenheten visade att även denna begränsade yta fortsatte att blanda ansvar och göra ägarskap otydligt. Beslutet har därför ersatts av [ADR-0042](0042-remove-shared-and-establish-explicit-ownership.md), som avvecklar `Shared` helt och ger varje production-typ en explicit ägare.