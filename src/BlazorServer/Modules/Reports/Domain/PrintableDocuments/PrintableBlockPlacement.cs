using BlazorServer.Modules.Reports.Domain.PrintableDocuments;

namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PrintableBlockPlacement(
    string BlockId,
    decimal TopMm,
    decimal HeightMm,
    PageBreakBehavior PageBreakBehavior
);
