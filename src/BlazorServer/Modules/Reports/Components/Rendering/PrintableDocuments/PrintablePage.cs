namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintablePage(
    int PageNumber,
    decimal BodyHeightMm,
    IReadOnlyList<PrintableBlockPlacement> BodyBlocks
);
