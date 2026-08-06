using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.SheetSection;

public class SheetCell
{
    public RowType RowType { get; set; }
    public ColumnType ColumnType { get; set; }
    public TextFormat TextFormat { get; set; } = new TextFormat();
    public CellFormat CellFormat { get; set; } = new CellFormat();
    public bool HideValue { get; set; } = false;
}
