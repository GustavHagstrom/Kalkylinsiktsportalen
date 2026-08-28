namespace BlazorServer.Modules.Reports.Domain.Views;

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

    public static CellLayout Default => new();
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

    public CellLayout Clone()
        => new()
        {
            BackgroundColor = BackgroundColor,
            PaddingLeftMm = PaddingLeftMm,
            PaddingRightMm = PaddingRightMm,
            PaddingTopMm = PaddingTopMm,
            PaddingBottomMm = PaddingBottomMm,
            LeftBorder = LeftBorder?.Clone(),
            RightBorder = RightBorder?.Clone(),
            TopBorder = TopBorder?.Clone(),
            BottomBorder = BottomBorder?.Clone()
        };

    private static double? ValidateNonNegative(double? value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, "Padding must not be negative.");

        return value;
    }
}
