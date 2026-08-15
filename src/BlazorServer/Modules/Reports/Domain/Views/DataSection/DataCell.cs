using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.DataSection;

public class DataCell : IReportFormattedCell
{
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public CellLayout? CellLayout { get; set; }
    public TextStyle? TextStyle { get; set; }
    public ValueFormat? ValueFormat { get; set; }
    public TextFormat? TextFormat { get; set; } = new TextFormat();
    public CellFormat? CellFormat { get; set; } = new CellFormat();
    public string CellExpression { get; set; } = string.Empty;

    public string RawValue
    {
        get => CellExpression;
        set => CellExpression = value ?? string.Empty;
    }

    public bool IsDefault()
        => string.IsNullOrEmpty(CellExpression)
           && ReportFormatting.IsDefault(this);

    public void ApplyLegacyFormatting()
    {
        ReportFormatting.ApplyLegacyFormatting(this);
    }

    public void ApplyFormatPatch(ReportFormatPatch patch)
    {
        ReportFormatting.ApplyPatch(this, patch);
    }
}
