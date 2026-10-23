namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class ColumnDefinition
{
    public double RelativeWidth { get; private set; }

    public ColumnDefinition(double relativeWidth)
    {
        SetRelativeWidth(relativeWidth);
    }

    public void SetRelativeWidth(double relativeWidth)
    {
        if (relativeWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(relativeWidth), "Relative width must be greater than zero.");
        }
        RelativeWidth = relativeWidth;
    }
}