using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintableBlockPlacement(
    string BlockId,
    decimal TopMm,
    decimal HeightMm,
    PageBreakBehavior PageBreakBehavior
);
