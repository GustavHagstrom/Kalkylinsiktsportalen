using Core.Enums.ViewTemplate;

namespace Data.MongoDb.Enteties.EstimationView;
public class SheetCellFormat
{
    public required SheetRowType RowType { get; set; }
    public required SheetColumnType ColumnType { get; set; }
    public bool IsVisible { get; set; } = true;
    public Align? HorizontalAlign { get; set; } = Align.Start;
    public string? BackgroundColor { get; set; } = null;
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;
    public int DecimalCount { get; set; } = 2;
    public bool DoesIncludeTimeOfDay { get; set; } = true;
    public int FontSize { get; set; } = 12;
    public TextFormatType FormatType { get; set; } = TextFormatType.Text;
    public bool HasBorderBottom { get; set; } = false;
    public bool HasBorderLeft { get; set; } = false;
    public bool HasBorderRight { get; set; } = false;
    public bool HasBorderTop { get; set; } = false;
    public bool HasThoasandsSeparator { get; set; } = true;
    public bool IsBold { get; set; } = false;
    public bool IsItalic { get; set; } = false;
    public bool IsUnderline { get; set; } = false;
    public Align? VerticalAlign { get; set; } = Align.End;
    public string? TextColor { get; set; } = null;
}
