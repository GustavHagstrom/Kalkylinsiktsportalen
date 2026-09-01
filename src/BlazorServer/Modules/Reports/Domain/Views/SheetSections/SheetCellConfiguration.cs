namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public sealed record class SheetCellConfiguration
{
    public CellLayout? CellLayout { get; init; }
    public TextStyle? TextStyle { get; init; }
    public ValueFormat? ValueFormat { get; init; }
    public bool HideValue { get; init; } = false;

    public static SheetCellConfiguration Create()
        => new()
        {
            CellLayout = CellLayout.Default,
            TextStyle = TextStyle.Default,
            ValueFormat = ValueFormat.Default,
            HideValue = false
        };

    public bool IsDefault()
        => !HideValue
           && ReportFormatting.IsDefault(new MutableFormattedCell
           {
               CellLayout = CellLayout,
               TextStyle = TextStyle,
               ValueFormat = ValueFormat
           });

    public static SheetCellConfiguration CreateFromPatch(SheetCellConfiguration current, ReportFormatPatch patch)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(patch);

        var formattedCell = new MutableFormattedCell
        {
            CellLayout = current.CellLayout,
            TextStyle = current.TextStyle,
            ValueFormat = current.ValueFormat
        };
        ReportFormatting.ApplyPatch(formattedCell, patch);

        return current with
        {
            CellLayout = formattedCell.CellLayout,
            TextStyle = formattedCell.TextStyle,
            ValueFormat = formattedCell.ValueFormat
        };
    }

    private sealed class MutableFormattedCell : IReportFormattedCell
    {
        public CellLayout? CellLayout { get; set; }
        public TextStyle? TextStyle { get; set; }
        public ValueFormat? ValueFormat { get; set; }
    }
}
