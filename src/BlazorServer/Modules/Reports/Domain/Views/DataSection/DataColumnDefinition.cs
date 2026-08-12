namespace BlazorServer.Modules.Reports.Domain.Views.DataSection;

public class DataColumnDefinition
{
    public int ColumnIndex { get; set; }
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
