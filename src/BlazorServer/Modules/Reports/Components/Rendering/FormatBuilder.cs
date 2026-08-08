using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Components.Rendering;

public static class FormatBuilder
{
    public static string BuildCellStyle(CellFormat cellFormat)
    {
        var borderStyles = new[]
        {
            $"border-left: {BuildBorderStyle(cellFormat.LeftBorder)}; ",
            $"border-top: {BuildBorderStyle(cellFormat.TopBorder)}; ",
            $"border-right: {BuildBorderStyle(cellFormat.RightBorder)}; ",
            $"border-bottom: {BuildBorderStyle(cellFormat.BottomBorder)}; "
        };

        return $"{string.Join("", borderStyles)} background-color: {cellFormat.BackgroundColor}; padding: {cellFormat.PaddingTop}px {cellFormat.PaddingRight}px {cellFormat.PaddingBottom}px {cellFormat.PaddingLeft}px;";
    }

    private static string BuildBorderStyle(BorderFormat borderStyle)
    {
        if (!borderStyle.IsBorder)
            return "none";

        return $"{borderStyle.BorderWidth}px {borderStyle.BorderStyle} {borderStyle.BorderColor}";
    }
    public static string BuildTextStyle(TextFormat textFormat)
    {
        return $"color: {textFormat.Color}; font-size: {textFormat.FontSize}px; font-family: {textFormat.FontFamily}; font-weight: {(textFormat.IsBold ? "bold" : "normal")}; font-style: {(textFormat.IsItalic ? "italic" : "normal")}; text-decoration: {(textFormat.IsUnderline ? "underline" : "none")}; text-align: {BuildHorizontalAlign(textFormat.HorizontalAlign)}; vertical-align: {BuildVerticalAlign(textFormat.VerticalAlign)};";
    }

    private static string BuildHorizontalAlign(Align align)
    {
        return align switch
        {
            Align.Start => "left",
            Align.Center => "center",
            Align.End => "right",
            _ => "left"
        };
    }

    private static string BuildVerticalAlign(Align align)
    {
        return align switch
        {
            Align.Start => "top",
            Align.Center => "middle",
            Align.End => "bottom",
            _ => "bottom"
        };
    }
}
