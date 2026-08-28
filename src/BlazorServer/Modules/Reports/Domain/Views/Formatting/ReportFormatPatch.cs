namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed class ReportFormatPatch
{
    public CellLayoutPatch? CellLayout { get; init; }
    public TextStylePatch? TextStyle { get; init; }
    public ValueFormat? ValueFormat { get; init; }
    public bool ClearValueFormat { get; init; }
}
