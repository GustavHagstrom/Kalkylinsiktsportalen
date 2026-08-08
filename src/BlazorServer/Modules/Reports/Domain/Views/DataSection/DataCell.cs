using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.DataSection;

public class DataCell
{
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public TextFormat TextFormat { get; set; } = new TextFormat();
    public CellFormat CellFormat { get; set; } = new CellFormat();
    public string RawValue { get; set; } = string.Empty;
}
