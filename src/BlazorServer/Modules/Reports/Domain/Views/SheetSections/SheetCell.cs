using BlazorServer.Modules.Reports.Domain.Views;
namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public class SheetCell : IReportFormattedCell
{
    public RowType RowType { get; set; }
    public ColumnType ColumnType { get; set; }
    public CellLayout? CellLayout { get; set; }
    public TextStyle? TextStyle { get; set; }
    public ValueFormat? ValueFormat { get; set; }
    public TextFormat? TextFormat { get; set; } = new TextFormat();
    public CellFormat? CellFormat { get; set; } = new CellFormat();
    public bool HideValue { get; set; } = false;

    public bool IsDefault()
        => !HideValue
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
