namespace BlazorServer.Modules.Reports.Domain.Views;

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

    public static TextStyle Default => new();

    public bool IsDefault()
        => Font is null
           && TextColor is null
           && FontSizePt is null
           && !IsBold
           && !IsItalic
           && !IsUnderline
           && HorizontalAlignment is null
           && VerticalAlignment is null;

    public TextStyle Clone()
        => new()
        {
            Font = Font,
            TextColor = TextColor,
            FontSizePt = FontSizePt,
            IsBold = IsBold,
            IsItalic = IsItalic,
            IsUnderline = IsUnderline,
            HorizontalAlignment = HorizontalAlignment,
            VerticalAlignment = VerticalAlignment
        };
}
