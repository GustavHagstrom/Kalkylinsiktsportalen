namespace BlazorServer.Modules.Reports.Domain.Views;

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

    public CellBorder Clone()
        => new()
        {
            WidthPt = WidthPt,
            Style = Style,
            Color = Color
        };
}
