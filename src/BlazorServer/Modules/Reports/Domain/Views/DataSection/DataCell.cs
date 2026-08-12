using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.DataSection;

public class DataCell
{
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public TextFormat TextFormat { get; set; } = new TextFormat();
    public CellFormat CellFormat { get; set; } = new CellFormat();
    public string CellExpression { get; set; } = string.Empty;

    public bool IsDefault()
        => string.IsNullOrEmpty(CellExpression)
           && TextFormat is not null
           && CellFormat is not null
           && TextFormatIsDefault(TextFormat)
           && CellFormatIsDefault(CellFormat);

    private static bool TextFormatIsDefault(TextFormat format)
    {
        var defaultFormat = new TextFormat();
        return format.FontFamily == defaultFormat.FontFamily
               && format.Color == defaultFormat.Color
               && format.FontSize == defaultFormat.FontSize
               && format.IsBold == defaultFormat.IsBold
               && format.IsItalic == defaultFormat.IsItalic
               && format.IsUnderline == defaultFormat.IsUnderline
               && format.HorizontalAlign == defaultFormat.HorizontalAlign
               && format.VerticalAlign == defaultFormat.VerticalAlign
               && format.Type == defaultFormat.Type
               && format.DayTimeFormat == defaultFormat.DayTimeFormat
               && format.DecimalCount == defaultFormat.DecimalCount
               && format.HasThoasandsSeparator == defaultFormat.HasThoasandsSeparator;
    }

    private static bool CellFormatIsDefault(CellFormat format)
    {
        var defaultFormat = new CellFormat();
        return BorderFormatIsDefault(format.LeftBorder, defaultFormat.LeftBorder)
               && BorderFormatIsDefault(format.RightBorder, defaultFormat.RightBorder)
               && BorderFormatIsDefault(format.TopBorder, defaultFormat.TopBorder)
               && BorderFormatIsDefault(format.BottomBorder, defaultFormat.BottomBorder)
               && format.BackgroundColor == defaultFormat.BackgroundColor
               && format.PaddingLeft == defaultFormat.PaddingLeft
               && format.PaddingRight == defaultFormat.PaddingRight
               && format.PaddingTop == defaultFormat.PaddingTop
               && format.PaddingBottom == defaultFormat.PaddingBottom;
    }

    private static bool BorderFormatIsDefault(BorderFormat format, BorderFormat defaultFormat)
        => format.IsBorder == defaultFormat.IsBorder
           && format.BorderStyle == defaultFormat.BorderStyle
           && format.BorderColor == defaultFormat.BorderColor
           && format.BorderWidth == defaultFormat.BorderWidth;
}
