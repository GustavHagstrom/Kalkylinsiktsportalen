using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Components.Rendering;

public static class FormatBuilder
{
    public static string BuildCellStyle(CellLayout? cellLayout)
    {
        var borderStyles = new[]
        {
            $"border-left: {BuildBorderStyle(cellLayout?.LeftBorder)}; ",
            $"border-top: {BuildBorderStyle(cellLayout?.TopBorder)}; ",
            $"border-right: {BuildBorderStyle(cellLayout?.RightBorder)}; ",
            $"border-bottom: {BuildBorderStyle(cellLayout?.BottomBorder)}; "
        };

        var background = cellLayout?.BackgroundColor is null
            ? string.Empty
            : $" background-color: {cellLayout.BackgroundColor};";

        var padding = HasAnyPadding(cellLayout)
            ? $" padding: {FormatMm(cellLayout!.PaddingTopMm ?? 0)} {FormatMm(cellLayout.PaddingRightMm ?? 0)} {FormatMm(cellLayout.PaddingBottomMm ?? 0)} {FormatMm(cellLayout.PaddingLeftMm ?? 0)};"
            : string.Empty;

        return $"{string.Join("", borderStyles)}{background}{padding}";
    }

    private static string BuildBorderStyle(CellBorder? borderStyle)
    {
        if (borderStyle is null)
            return "none";

        return $"{FormatPt(borderStyle.WidthPt)} {borderStyle.Style.ToString().ToLowerInvariant()} {borderStyle.Color}";
    }

    public static string BuildTextStyle(TextStyle? textStyle, ValueDisplayType displayType)
    {
        var declarations = new List<string>();

        if (textStyle?.TextColor is not null)
            declarations.Add($"color: {textStyle.TextColor}");
        if (textStyle?.FontSizePt is not null)
            declarations.Add($"font-size: {FormatPt(textStyle.FontSizePt.Value)}");
        if (textStyle?.Font is not null)
            declarations.Add($"font-family: {ReportFormatting.ToCssFontFamily(textStyle.Font.Value)}");

        declarations.Add($"font-weight: {(textStyle?.IsBold == true ? "bold" : "normal")}");
        declarations.Add($"font-style: {(textStyle?.IsItalic == true ? "italic" : "normal")}");
        declarations.Add($"text-decoration: {(textStyle?.IsUnderline == true ? "underline" : "none")}");
        declarations.Add($"text-align: {BuildHorizontalAlign(textStyle?.HorizontalAlignment, displayType)}");
        declarations.Add($"vertical-align: {BuildVerticalAlign(textStyle?.VerticalAlignment)}");

        return string.Join("; ", declarations) + ";";
    }

    private static string BuildHorizontalAlign(HorizontalTextAlignment? align, ValueDisplayType displayType)
    {
        var resolvedAlignment = align ?? (displayType == ValueDisplayType.Text
            ? HorizontalTextAlignment.Left
            : HorizontalTextAlignment.Right);

        return resolvedAlignment switch
        {
            HorizontalTextAlignment.Center => "center",
            HorizontalTextAlignment.Right => "right",
            _ => "left"
        };
    }

    private static string BuildVerticalAlign(VerticalTextAlignment? align)
    {
        return align switch
        {
            VerticalTextAlignment.Top => "top",
            VerticalTextAlignment.Middle => "middle",
            _ => "bottom"
        };
    }

    private static bool HasAnyPadding(CellLayout? cellLayout)
        => cellLayout?.PaddingLeftMm is not null
           || cellLayout?.PaddingRightMm is not null
           || cellLayout?.PaddingTopMm is not null
           || cellLayout?.PaddingBottomMm is not null;

    private static string FormatMm(double value)
        => $"{value:0.####}mm";

    private static string FormatPt(double value)
        => $"{value:0.####}pt";
}
