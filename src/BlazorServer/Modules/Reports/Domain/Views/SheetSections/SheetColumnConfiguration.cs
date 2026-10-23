namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public sealed record class SheetColumnConfiguration
{
    public const double DefaultRelativeWidth = 10;
    private double _relativeWidth = DefaultRelativeWidth;

    public SheetColumnKind SheetColumnKind { get; init; }
    public double RelativeWidth
    {
        get => _relativeWidth;
        init
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(RelativeWidth), "Relative width must be greater than zero.");
            }

            _relativeWidth = value;
        }
    }
    public bool IsVisible { get; init; } = true;

    public static SheetColumnConfiguration Create(
        SheetColumnKind sheetColumnKind,
        double relativeWidth = DefaultRelativeWidth,
        bool isVisible = true)
        => new()
        {
            SheetColumnKind = sheetColumnKind,
            RelativeWidth = relativeWidth,
            IsVisible = isVisible
        };
}
