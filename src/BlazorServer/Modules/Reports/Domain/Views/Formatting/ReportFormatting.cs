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
            layout = layout with { BackgroundColor = null };
        if (patch.BackgroundColor is not null)
            layout = layout with { BackgroundColor = patch.BackgroundColor };

        if (patch.ClearPaddingLeft)
            layout = layout with { PaddingLeftMm = null };
        if (patch.PaddingLeftMm is not null)
            layout = layout with { PaddingLeftMm = patch.PaddingLeftMm };

        if (patch.ClearPaddingRight)
            layout = layout with { PaddingRightMm = null };
        if (patch.PaddingRightMm is not null)
            layout = layout with { PaddingRightMm = patch.PaddingRightMm };

        if (patch.ClearPaddingTop)
            layout = layout with { PaddingTopMm = null };
        if (patch.PaddingTopMm is not null)
            layout = layout with { PaddingTopMm = patch.PaddingTopMm };

        if (patch.ClearPaddingBottom)
            layout = layout with { PaddingBottomMm = null };
        if (patch.PaddingBottomMm is not null)
            layout = layout with { PaddingBottomMm = patch.PaddingBottomMm };

        if (patch.ClearLeftBorder)
            layout = layout with { LeftBorder = null };
        if (patch.LeftBorder is not null)
            layout = layout with { LeftBorder = patch.LeftBorder };

        if (patch.ClearRightBorder)
            layout = layout with { RightBorder = null };
        if (patch.RightBorder is not null)
            layout = layout with { RightBorder = patch.RightBorder };

        if (patch.ClearTopBorder)
            layout = layout with { TopBorder = null };
        if (patch.TopBorder is not null)
            layout = layout with { TopBorder = patch.TopBorder };

        if (patch.ClearBottomBorder)
            layout = layout with { BottomBorder = null };
        if (patch.BottomBorder is not null)
            layout = layout with { BottomBorder = patch.BottomBorder };

        return layout;
    }

    private static TextStyle ApplyPatch(TextStyle? current, TextStylePatch patch)
    {
        var style = current ?? new TextStyle();

        if (patch.ClearFont)
            style = style with { Font = null };
        if (patch.Font is not null)
            style = style with { Font = patch.Font };

        if (patch.ClearTextColor)
            style = style with { TextColor = null };
        if (patch.TextColor is not null)
            style = style with { TextColor = patch.TextColor };

        if (patch.ClearFontSize)
            style = style with { FontSizePt = null };
        if (patch.FontSizePt is not null)
            style = style with { FontSizePt = patch.FontSizePt };

        if (patch.IsBold is not null)
            style = style with { IsBold = patch.IsBold.Value };
        if (patch.IsItalic is not null)
            style = style with { IsItalic = patch.IsItalic.Value };
        if (patch.IsUnderline is not null)
            style = style with { IsUnderline = patch.IsUnderline.Value };

        if (patch.ClearHorizontalAlignment)
            style = style with { HorizontalAlignment = null };
        if (patch.HorizontalAlignment is not null)
            style = style with { HorizontalAlignment = patch.HorizontalAlignment };

        if (patch.ClearVerticalAlignment)
            style = style with { VerticalAlignment = null };
        if (patch.VerticalAlignment is not null)
            style = style with { VerticalAlignment = patch.VerticalAlignment };

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
