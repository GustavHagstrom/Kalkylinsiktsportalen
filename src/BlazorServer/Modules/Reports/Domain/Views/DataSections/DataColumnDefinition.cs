namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class DataColumnDefinition
{
    public double RelativeWidth { get; set; }

    public double WidthWeight
    {
        get => RelativeWidth;
        set => RelativeWidth = value;
    }

    public double Width
    {
        get => RelativeWidth;
        set => RelativeWidth = value;
    }
}
