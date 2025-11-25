using Core.Enums.ViewTemplate;

namespace Core.Models.EstimationView;
public interface IFormat
{
    Align? HorizontalAlign { get; set; }
    string? BackgroundColor { get; set; }
    BorderStyle BorderStyle { get; set; }
    int DecimalCount { get; set; }
    bool DoesIncludeTimeOfDay { get; set; }
    int FontSize { get; set; }
    TextFormatType FormatType { get; set; }
    bool HasBorderBottom { get; set; }
    bool HasBorderLeft { get; set; }
    bool HasBorderRight { get; set; }
    bool HasBorderTop { get; set; }
    bool HasThoasandsSeparator { get; set; }
    bool IsBold { get; set; }
    bool IsItalic { get; set; }
    bool IsUnderline { get; set; }
    Align? VerticalAlign { get; set; }
    string? TextColor { get; set; }

    void ApplyFormat(IFormat format);
}