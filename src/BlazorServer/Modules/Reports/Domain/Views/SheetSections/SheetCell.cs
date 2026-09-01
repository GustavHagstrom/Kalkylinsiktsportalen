namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public class SheetCell : IReportFormattedCell
{
    public RowType RowType { get; set; }
    public SheetColumnKind SheetColumnKind { get; set; }
    public CellLayout? CellLayout { get; set; }
    public TextStyle? TextStyle { get; set; }
    public ValueFormat? ValueFormat { get; set; }
    public bool HideValue { get; set; } = false;

    public static SheetCell CreateDefault(SheetColumnKind sheetColumnKind, RowType rowType)
    {
        return new SheetCell
        {
            SheetColumnKind = sheetColumnKind,
            RowType = rowType,
            CellLayout = CellLayout.Default,
            TextStyle = TextStyle.Default,
            ValueFormat = ValueFormat.Default,
            HideValue = false
        };
    }
    public bool IsDefault()
        => !HideValue
           && ReportFormatting.IsDefault(this);

    public void ApplyFormatPatch(ReportFormatPatch patch)
    {
        ReportFormatting.ApplyPatch(this, patch);
    }
}
