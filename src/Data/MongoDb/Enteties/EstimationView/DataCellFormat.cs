using Core.Enums.ViewTemplate;

namespace Data.MongoDb.Enteties.EstimationView;

public class DataCellFormat
{
    public string FontFamily { get; set; } = string.Empty;
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public int FontSize { get; set; } = 12;
    public bool IsBold { get; set; } = false;
    public bool IsItalic { get; set; } = false;
    public bool IsUnderline { get; set; } = false;
    public Align? HorizontalAlign { get; set; } = Align.Start;
    public Align? VerticalAlign { get; set; } = Align.End;
    public TextFormatType FormatType { get; set; } = TextFormatType.Text;
    public bool HasThoasandsSeparator { get; set; } = true;
    public int DecimalCount { get; set; } = 2;
    public bool DoesIncludeTimeOfDay { get; set; } = false;
    public bool HasBorderLeft { get; set; } = false;
    public bool HasBorderTop { get; set; } = false;
    public bool HasBorderRight { get; set; } = false;
    public bool HasBorderBottom { get; set; } = false;
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;
}
