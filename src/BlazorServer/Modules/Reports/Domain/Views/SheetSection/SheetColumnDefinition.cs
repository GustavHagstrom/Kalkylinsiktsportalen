namespace BlazorServer.Modules.Reports.Domain.Views.SheetSection;

public class SheetColumnDefinition
{
    public ColumnType ColumnType { get; set; }
    public double Width { get; set; }
    public bool IsVisible { get; set; } = true;
}
