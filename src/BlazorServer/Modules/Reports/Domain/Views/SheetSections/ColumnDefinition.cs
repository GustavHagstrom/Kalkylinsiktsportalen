namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public class ColumnDefinition
{
    public const double DefaultRelativeWidth = 10;
    public ColumnType ColumnType { get; set; }
    public double RelativeWidth { get; private set; } = DefaultRelativeWidth;
    public bool IsVisible { get; set; } = true;

    public static ColumnDefinition Create(ColumnType columnType, double relativeWidth = DefaultRelativeWidth, bool isVisible = true)
    {
        var definition = new ColumnDefinition
        {
            ColumnType = columnType,
            IsVisible = isVisible
        };
        definition.SetRelativeWidth(relativeWidth);
        return definition;
    }
    private void SetRelativeWidth(double relativeWidth)
    {
        if (relativeWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(relativeWidth), "Relative width must be greater than zero.");
        }
        RelativeWidth = relativeWidth;
    }
}
