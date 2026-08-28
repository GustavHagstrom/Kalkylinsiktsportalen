using System.Globalization;

namespace BlazorServer.Modules.Reports.Domain.Views;

public static class ReportFormatting
{
    public static string ToCssFontFamily(ReportFont font)
        => font switch
        {
            ReportFont.Arial => "Arial",
            ReportFont.TimesNewRoman => "\"Times New Roman\"",
            _ => "Calibri"
        };

    public static bool IsDefault(IReportFormattedCell cell)
        => (cell.TextStyle is null || cell.TextStyle.IsDefault())
           && (cell.CellLayout is null || cell.CellLayout.IsDefault())
           && (cell.ValueFormat is null || cell.ValueFormat.IsDefault());

    public static void ApplyPatch(IReportFormattedCell cell, ReportFormatPatch patch)
    {
        ArgumentNullException.ThrowIfNull(patch);

        if (patch.CellLayout is not null)
        {
            cell.CellLayout = ApplyPatch(cell.CellLayout, patch.CellLayout);
            if (cell.CellLayout.IsDefault())
                cell.CellLayout = null;
        }

        if (patch.TextStyle is not null)
        {
            cell.TextStyle = ApplyPatch(cell.TextStyle, patch.TextStyle);
            if (cell.TextStyle.IsDefault())
                cell.TextStyle = null;
        }

        if (patch.ClearValueFormat)
            cell.ValueFormat = null;
        if (patch.ValueFormat is not null)
            cell.ValueFormat = patch.ValueFormat.IsDefault() ? null : patch.ValueFormat;
    }

    private static CellLayout ApplyPatch(CellLayout? current, CellLayoutPatch patch)
    {
        var layout = current ?? new CellLayout();

        if (patch.ClearBackgroundColor)
            layout.BackgroundColor = null;
        if (patch.BackgroundColor is not null)
            layout.BackgroundColor = patch.BackgroundColor;

        if (patch.ClearPaddingLeft)
            layout.PaddingLeftMm = null;
        if (patch.PaddingLeftMm is not null)
            layout.PaddingLeftMm = patch.PaddingLeftMm;

        if (patch.ClearPaddingRight)
            layout.PaddingRightMm = null;
        if (patch.PaddingRightMm is not null)
            layout.PaddingRightMm = patch.PaddingRightMm;

        if (patch.ClearPaddingTop)
            layout.PaddingTopMm = null;
        if (patch.PaddingTopMm is not null)
            layout.PaddingTopMm = patch.PaddingTopMm;

        if (patch.ClearPaddingBottom)
            layout.PaddingBottomMm = null;
        if (patch.PaddingBottomMm is not null)
            layout.PaddingBottomMm = patch.PaddingBottomMm;

        if (patch.ClearLeftBorder)
            layout.LeftBorder = null;
        if (patch.LeftBorder is not null)
            layout.LeftBorder = patch.LeftBorder;

        if (patch.ClearRightBorder)
            layout.RightBorder = null;
        if (patch.RightBorder is not null)
            layout.RightBorder = patch.RightBorder;

        if (patch.ClearTopBorder)
            layout.TopBorder = null;
        if (patch.TopBorder is not null)
            layout.TopBorder = patch.TopBorder;

        if (patch.ClearBottomBorder)
            layout.BottomBorder = null;
        if (patch.BottomBorder is not null)
            layout.BottomBorder = patch.BottomBorder;

        return layout;
    }

    private static TextStyle ApplyPatch(TextStyle? current, TextStylePatch patch)
    {
        var style = current ?? new TextStyle();

        if (patch.ClearFont)
            style.Font = null;
        if (patch.Font is not null)
            style.Font = patch.Font;

        if (patch.ClearTextColor)
            style.TextColor = null;
        if (patch.TextColor is not null)
            style.TextColor = patch.TextColor;

        if (patch.ClearFontSize)
            style.FontSizePt = null;
        if (patch.FontSizePt is not null)
            style.FontSizePt = patch.FontSizePt;

        if (patch.IsBold is not null)
            style.IsBold = patch.IsBold.Value;
        if (patch.IsItalic is not null)
            style.IsItalic = patch.IsItalic.Value;
        if (patch.IsUnderline is not null)
            style.IsUnderline = patch.IsUnderline.Value;

        if (patch.ClearHorizontalAlignment)
            style.HorizontalAlignment = null;
        if (patch.HorizontalAlignment is not null)
            style.HorizontalAlignment = patch.HorizontalAlignment;

        if (patch.ClearVerticalAlignment)
            style.VerticalAlignment = null;
        if (patch.VerticalAlignment is not null)
            style.VerticalAlignment = patch.VerticalAlignment;

        return style;
    }

    internal static string FormatNumber(decimal value, ValueFormat valueFormat)
    {
        var format = valueFormat.UseThousandsSeparator
            ? $"N{valueFormat.DecimalCount}"
            : $"F{valueFormat.DecimalCount}";

        return value.ToString(format, CultureInfo.CurrentCulture);
    }
}
