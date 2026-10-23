namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed record TextStyle
{
    private readonly string? _textColor;
    private readonly double? _fontSizePt;

    public ReportFont? Font { get; init; }
    public string? TextColor
    {
        get => _textColor;
        init => _textColor = ReportColor.NormalizeOptional(value);
    }
    public double? FontSizePt
    {
        get => _fontSizePt;
        init
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Font size must be greater than zero.");

            _fontSizePt = value;
        }
    }
    public bool IsBold { get; init; }
    public bool IsItalic { get; init; }
    public bool IsUnderline { get; init; }
    public HorizontalTextAlignment? HorizontalAlignment { get; init; }
    public VerticalTextAlignment? VerticalAlignment { get; init; }

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

}
