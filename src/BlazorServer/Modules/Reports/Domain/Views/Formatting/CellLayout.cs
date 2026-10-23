namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed record CellLayout
{
    private readonly string? _backgroundColor;
    private readonly double? _paddingLeftMm;
    private readonly double? _paddingRightMm;
    private readonly double? _paddingTopMm;
    private readonly double? _paddingBottomMm;

    public string? BackgroundColor
    {
        get => _backgroundColor;
        init => _backgroundColor = ReportColor.NormalizeOptional(value);
    }

    public double? PaddingLeftMm
    {
        get => _paddingLeftMm;
        init => _paddingLeftMm = ValidateNonNegative(value, nameof(PaddingLeftMm));
    }
    public double? PaddingRightMm
    {
        get => _paddingRightMm;
        init => _paddingRightMm = ValidateNonNegative(value, nameof(PaddingRightMm));
    }
    public double? PaddingTopMm
    {
        get => _paddingTopMm;
        init => _paddingTopMm = ValidateNonNegative(value, nameof(PaddingTopMm));
    }
    public double? PaddingBottomMm
    {
        get => _paddingBottomMm;
        init => _paddingBottomMm = ValidateNonNegative(value, nameof(PaddingBottomMm));
    }

    public CellBorder? LeftBorder { get; init; }
    public CellBorder? RightBorder { get; init; }
    public CellBorder? TopBorder { get; init; }
    public CellBorder? BottomBorder { get; init; }

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

    private static double? ValidateNonNegative(double? value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, "Padding must not be negative.");

        return value;
    }
}
