namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class ColumnDefinition
{
    public double RelativeWidth { get; set; }

    public ColumnDefinition(double relativeWidth)
    {
        RelativeWidth = relativeWidth;
    }
}