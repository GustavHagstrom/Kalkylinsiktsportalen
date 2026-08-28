namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed record ValueFormat
{
    private readonly int _decimalCount;

    public ValueFormatKind Kind { get; init; } = ValueFormatKind.Text;
    public int DecimalCount
    {
        get => _decimalCount;
        init
        {
            if (value < 0 || value > 6)
                throw new ArgumentOutOfRangeException(nameof(value), "Decimal count must be between 0 and 6.");

            _decimalCount = value;
        }
    }
    public bool UseThousandsSeparator { get; init; }

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

    public static ValueFormat Default => new();
    public bool IsDefault()
        => Kind == ValueFormatKind.Text
           && DecimalCount == 0
           && !UseThousandsSeparator;

}
