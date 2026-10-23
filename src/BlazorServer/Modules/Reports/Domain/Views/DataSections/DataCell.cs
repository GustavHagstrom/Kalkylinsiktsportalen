namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class DataCell : IReportFormattedCell
{
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public CellLayout? CellLayout { get; set; }
    public TextStyle? TextStyle { get; set; }
    public ValueFormat? ValueFormat { get; set; }
    public string CellExpression { get; set; } = string.Empty;

    public bool IsDefault()
        => string.IsNullOrEmpty(CellExpression)
           && ReportFormatting.IsDefault(this);
    public void ApplyFormatPatch(ReportFormatPatch patch)
    {
        ReportFormatting.ApplyPatch(this, patch);
    }
    public static DataCell CreateDefault(int rowIndex, int columnIndex)
    {
        return new DataCell
        {
            RowIndex = rowIndex,
            ColumnIndex = columnIndex,
            CellLayout = null,
            TextStyle = null,
            ValueFormat = null,
            CellExpression = string.Empty
        };
    }
}
