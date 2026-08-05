namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PrintablePage(
    int PageNumber,
    decimal BodyHeightMm,
    IReadOnlyList<PrintableBlockPlacement> BodyBlocks
);
