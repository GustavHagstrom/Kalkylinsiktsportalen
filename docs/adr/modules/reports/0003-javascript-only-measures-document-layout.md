# JavaScript Only Measures Document Layout

The paginated document renderer uses a collocated JavaScript module as a browser measurement adapter. Blazor renders a measurement DOM with renderer-owned wrappers for document regions and printable blocks; JavaScript returns measured dimensions for those elements and does not move Blazor-owned DOM nodes or decide page placement.

C# owns pagination rules, including page placement, page-break behavior, warnings, and errors. Blazor then renders the final paginated preview from the calculated pagination result. This keeps browser-specific measurement in JavaScript while keeping pagination testable in C#.
