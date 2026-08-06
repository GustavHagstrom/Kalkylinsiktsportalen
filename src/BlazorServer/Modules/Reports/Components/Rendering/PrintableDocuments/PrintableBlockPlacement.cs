using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintableBlockPlacement(
    string BlockId,
    decimal TopMm,
    decimal HeightMm,
    PageBreakBehavior PageBreakBehavior
);
