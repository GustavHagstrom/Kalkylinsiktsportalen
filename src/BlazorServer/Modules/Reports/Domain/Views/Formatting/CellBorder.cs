namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed record CellBorder
{
    private readonly double _widthPt;
    private readonly string _color = "#000000";

    public double WidthPt
    {
        get => _widthPt;
        init
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Border width must be greater than zero.");

            _widthPt = value;
        }
    }
    public BorderStyle Style { get; init; } = BorderStyle.Solid;
    public string Color
    {
        get => _color;
        init => _color = ReportColor.Normalize(value);
    }

}
