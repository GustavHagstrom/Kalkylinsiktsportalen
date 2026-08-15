using System.Globalization;
using System.Text.RegularExpressions;

namespace BlazorServer.Modules.Reports.Domain.Views;

public enum BorderStyle
{
    Solid,
    Dashed,
    Dotted,
    Double
}

public enum HorizontalTextAlignment
{
    Left,
    Center,
    Right
}

public enum VerticalTextAlignment
{
    Top,
    Middle,
    Bottom
}

public enum ReportFont
{
    Calibri,
    Arial,
    TimesNewRoman
}

public enum ValueFormatKind
{
    Text,
    Number,
    Date,
    DateTimeMinute
}

public enum ValueDisplayType
{
    Text,
    Number,
    DateTime
}

public sealed class ValueFormat
{
    private int _decimalCount;

    public ValueFormatKind Kind { get; set; } = ValueFormatKind.Text;
    public int DecimalCount
    {
        get => _decimalCount;
        set
        {
            if (value < 0 || value > 6)
                throw new ArgumentOutOfRangeException(nameof(value), "Decimal count must be between 0 and 6.");

            _decimalCount = value;
        }
    }
    public bool UseThousandsSeparator { get; set; }

    public static ValueFormat Text => new();
    public static ValueFormat Date => new() { Kind = ValueFormatKind.Date };
    public static ValueFormat DateTimeMinute => new() { Kind = ValueFormatKind.DateTimeMinute };

    public static ValueFormat Number(int decimalCount = 2, bool useThousandsSeparator = true)
        => new()
        {
            Kind = ValueFormatKind.Number,
            DecimalCount = decimalCount,
            UseThousandsSeparator = useThousandsSeparator
        };

    public bool IsDefault()
        => Kind == ValueFormatKind.Text
           && DecimalCount == 0
           && !UseThousandsSeparator;
}

public sealed class TextStyle
{
    private string? _textColor;
    private double? _fontSizePt;

    public ReportFont? Font { get; set; }
    public string? TextColor
    {
        get => _textColor;
        set => _textColor = ReportColor.NormalizeOptional(value);
    }
    public double? FontSizePt
    {
        get => _fontSizePt;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Font size must be greater than zero.");

            _fontSizePt = value;
        }
    }
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    public bool IsUnderline { get; set; }
    public HorizontalTextAlignment? HorizontalAlignment { get; set; }
    public VerticalTextAlignment? VerticalAlignment { get; set; }

    public bool IsDefault()
        => Font is null
           && TextColor is null
           && FontSizePt is null
           && !IsBold
           && !IsItalic
           && !IsUnderline
           && HorizontalAlignment is null
           && VerticalAlignment is null;
}

public sealed class CellLayout
{
    private string? _backgroundColor;
    private double? _paddingLeftMm;
    private double? _paddingRightMm;
    private double? _paddingTopMm;
    private double? _paddingBottomMm;

    public string? BackgroundColor
    {
        get => _backgroundColor;
        set => _backgroundColor = ReportColor.NormalizeOptional(value);
    }

    public double? PaddingLeftMm
    {
        get => _paddingLeftMm;
        set => _paddingLeftMm = ValidateNonNegative(value, nameof(PaddingLeftMm));
    }
    public double? PaddingRightMm
    {
        get => _paddingRightMm;
        set => _paddingRightMm = ValidateNonNegative(value, nameof(PaddingRightMm));
    }
    public double? PaddingTopMm
    {
        get => _paddingTopMm;
        set => _paddingTopMm = ValidateNonNegative(value, nameof(PaddingTopMm));
    }
    public double? PaddingBottomMm
    {
        get => _paddingBottomMm;
        set => _paddingBottomMm = ValidateNonNegative(value, nameof(PaddingBottomMm));
    }

    public CellBorder? LeftBorder { get; set; }
    public CellBorder? RightBorder { get; set; }
    public CellBorder? TopBorder { get; set; }
    public CellBorder? BottomBorder { get; set; }

    public bool IsDefault()
        => BackgroundColor is null
           && PaddingLeftMm is null
           && PaddingRightMm is null
           && PaddingTopMm is null
           && PaddingBottomMm is null
           && LeftBorder is null
           && RightBorder is null
           && TopBorder is null
           && BottomBorder is null;

    private static double? ValidateNonNegative(double? value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, "Padding must not be negative.");

        return value;
    }
}

public sealed class CellBorder
{
    private double _widthPt;
    private string _color = "#000000";

    public double WidthPt
    {
        get => _widthPt;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Border width must be greater than zero.");

            _widthPt = value;
        }
    }
    public BorderStyle Style { get; set; } = BorderStyle.Solid;
    public string Color
    {
        get => _color;
        set => _color = ReportColor.Normalize(value);
    }
}

public sealed class ReportFormatPatch
{
    public CellLayoutPatch? CellLayout { get; init; }
    public TextStylePatch? TextStyle { get; init; }
    public ValueFormat? ValueFormat { get; init; }
    public bool ClearValueFormat { get; init; }
}

public sealed class TextStylePatch
{
    public ReportFont? Font { get; init; }
    public bool ClearFont { get; init; }
    public string? TextColor { get; init; }
    public bool ClearTextColor { get; init; }
    public double? FontSizePt { get; init; }
    public bool ClearFontSize { get; init; }
    public bool? IsBold { get; init; }
    public bool? IsItalic { get; init; }
    public bool? IsUnderline { get; init; }
    public HorizontalTextAlignment? HorizontalAlignment { get; init; }
    public bool ClearHorizontalAlignment { get; init; }
    public VerticalTextAlignment? VerticalAlignment { get; init; }
    public bool ClearVerticalAlignment { get; init; }
}

public sealed class CellLayoutPatch
{
    public string? BackgroundColor { get; init; }
    public bool ClearBackgroundColor { get; init; }
    public double? PaddingLeftMm { get; init; }
    public bool ClearPaddingLeft { get; init; }
    public double? PaddingRightMm { get; init; }
    public bool ClearPaddingRight { get; init; }
    public double? PaddingTopMm { get; init; }
    public bool ClearPaddingTop { get; init; }
    public double? PaddingBottomMm { get; init; }
    public bool ClearPaddingBottom { get; init; }
    public CellBorder? LeftBorder { get; init; }
    public bool ClearLeftBorder { get; init; }
    public CellBorder? RightBorder { get; init; }
    public bool ClearRightBorder { get; init; }
    public CellBorder? TopBorder { get; init; }
    public bool ClearTopBorder { get; init; }
    public CellBorder? BottomBorder { get; init; }
    public bool ClearBottomBorder { get; init; }
}

public readonly record struct DataCellRange(int StartRowIndex, int StartColumnIndex, int EndRowIndex, int EndColumnIndex)
{
    public static DataCellRange FromBounds(int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        => new(startRowIndex, startColumnIndex, endRowIndex, endColumnIndex);
}

public interface IReportFormattedCell
{
    CellLayout? CellLayout { get; set; }
    TextStyle? TextStyle { get; set; }
    ValueFormat? ValueFormat { get; set; }
    TextFormat? TextFormat { get; set; }
    CellFormat? CellFormat { get; set; }
}

public static partial class ReportColor
{
    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Color is required.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();
        if (!HexColorRegex().IsMatch(normalized))
            throw new ArgumentException("Color must be an uppercase #RRGGBB hex value without alpha.", nameof(value));

        return normalized;
    }

    public static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : Normalize(value);

    [GeneratedRegex("^#[0-9A-F]{6}$")]
    private static partial Regex HexColorRegex();
}

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
           && (cell.ValueFormat is null || cell.ValueFormat.IsDefault())
           && (cell.TextFormat is null || IsDefault(cell.TextFormat))
           && (cell.CellFormat is null || IsDefault(cell.CellFormat));

    public static void ApplyLegacyFormatting(IReportFormattedCell cell)
    {
        if (cell.TextFormat is not null)
        {
            cell.TextStyle ??= cell.TextFormat.ToTextStyle();
            cell.ValueFormat ??= cell.TextFormat.ToValueFormat();
            if (cell.ValueFormat.IsDefault())
                cell.ValueFormat = null;
        }

        if (cell.CellFormat is not null)
        {
            cell.CellLayout ??= cell.CellFormat.ToCellLayout();
            if (cell.CellLayout.IsDefault())
                cell.CellLayout = null;
        }
    }

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

    public static TextStyle ToTextStyle(this TextFormat format)
        => new()
        {
            Font = ParseFont(format.FontFamily),
            TextColor = IsDefaultBlack(format.Color) ? null : format.Color,
            FontSizePt = format.FontSize == 12 ? null : format.FontSize,
            IsBold = format.IsBold,
            IsItalic = format.IsItalic,
            IsUnderline = format.IsUnderline,
            HorizontalAlignment = format.HorizontalAlign.ToHorizontalAlignment(),
            VerticalAlignment = format.VerticalAlign.ToVerticalAlignment()
        };

    public static ValueFormat ToValueFormat(this TextFormat format)
        => format.Type switch
        {
            TextType.Number => ValueFormat.Number(format.DecimalCount, format.HasThoasandsSeparator),
            TextType.DateTime when string.Equals(format.DayTimeFormat, "yyyy-MM-dd HH:ss", StringComparison.Ordinal)
                => ValueFormat.DateTimeMinute,
            TextType.DateTime => ValueFormat.Date,
            _ => ValueFormat.Text
        };

    public static CellLayout ToCellLayout(this CellFormat format)
        => new()
        {
            BackgroundColor = IsDefaultWhite(format.BackgroundColor) ? null : format.BackgroundColor,
            PaddingLeftMm = format.PaddingLeft == 4 ? null : format.PaddingLeft,
            PaddingRightMm = format.PaddingRight == 4 ? null : format.PaddingRight,
            PaddingTopMm = format.PaddingTop == 2 ? null : format.PaddingTop,
            PaddingBottomMm = format.PaddingBottom == 2 ? null : format.PaddingBottom,
            LeftBorder = format.LeftBorder.ToCellBorderOrNull(),
            RightBorder = format.RightBorder.ToCellBorderOrNull(),
            TopBorder = format.TopBorder.ToCellBorderOrNull(),
            BottomBorder = format.BottomBorder.ToCellBorderOrNull()
        };

    public static CellBorder? ToCellBorderOrNull(this BorderFormat border)
    {
        if (!border.IsBorder || string.Equals(border.BorderStyle, "None", StringComparison.OrdinalIgnoreCase))
            return null;

        return new CellBorder
        {
            WidthPt = border.BorderWidth,
            Style = border.BorderStyle.ToBorderStyle(),
            Color = border.BorderColor
        };
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

    private static bool IsDefault(TextFormat format)
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

    private static bool IsDefault(CellFormat format)
    {
        var defaultFormat = new CellFormat();
        return IsDefault(format.LeftBorder, defaultFormat.LeftBorder)
               && IsDefault(format.RightBorder, defaultFormat.RightBorder)
               && IsDefault(format.TopBorder, defaultFormat.TopBorder)
               && IsDefault(format.BottomBorder, defaultFormat.BottomBorder)
               && format.BackgroundColor == defaultFormat.BackgroundColor
               && format.PaddingLeft == defaultFormat.PaddingLeft
               && format.PaddingRight == defaultFormat.PaddingRight
               && format.PaddingTop == defaultFormat.PaddingTop
               && format.PaddingBottom == defaultFormat.PaddingBottom;
    }

    private static bool IsDefault(BorderFormat format, BorderFormat defaultFormat)
        => format.IsBorder == defaultFormat.IsBorder
           && format.BorderStyle == defaultFormat.BorderStyle
           && format.BorderColor == defaultFormat.BorderColor
           && format.BorderWidth == defaultFormat.BorderWidth;

    private static ReportFont? ParseFont(string fontFamily)
        => fontFamily.Trim() switch
        {
            "Arial" => ReportFont.Arial,
            "Times New Roman" => ReportFont.TimesNewRoman,
            "Calibri" => null,
            "" => null,
            _ => null
        };

    private static HorizontalTextAlignment? ToHorizontalAlignment(this Align align)
        => align switch
        {
            Align.Center => HorizontalTextAlignment.Center,
            Align.End => HorizontalTextAlignment.Right,
            _ => null
        };

    private static VerticalTextAlignment? ToVerticalAlignment(this Align align)
        => align switch
        {
            Align.Start => VerticalTextAlignment.Top,
            Align.Center => VerticalTextAlignment.Middle,
            _ => null
        };

    private static BorderStyle ToBorderStyle(this string style)
        => style.Trim() switch
        {
            "Dashed" or "dashed" => BorderStyle.Dashed,
            "Dotted" or "dotted" => BorderStyle.Dotted,
            "Double" or "double" => BorderStyle.Double,
            _ => BorderStyle.Solid
        };

    private static bool IsDefaultWhite(string color)
        => string.Equals(color, "#FFFFFF", StringComparison.OrdinalIgnoreCase);

    private static bool IsDefaultBlack(string color)
        => string.Equals(color, "#000000", StringComparison.OrdinalIgnoreCase);
}
