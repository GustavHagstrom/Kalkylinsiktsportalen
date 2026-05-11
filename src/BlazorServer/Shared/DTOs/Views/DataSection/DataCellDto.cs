namespace BlazorServer.Shared.DTOs.Views.DataSection;

public class DataCellDto
{
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public TextFormatDto TextFormat { get; set; } = new TextFormatDto();
    public CellFormatDto CellFormat { get; set; } = new CellFormatDto();
    public string RawValue { get; set; } = string.Empty;
}
