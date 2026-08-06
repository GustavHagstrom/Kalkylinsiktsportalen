using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.SheetSection;

public class SheetCellDto
{
    public RowType RowType { get; set; }
    public ColumnType ColumnType { get; set; }
    public TextFormatDto TextFormat { get; set; } = new TextFormatDto();
    public CellFormatDto CellFormat { get; set; } = new CellFormatDto();
    public bool HideValue { get; set; } = false;
}
