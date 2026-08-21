namespace BlazorServer.Modules.Reports.Domain.Views.SheetSections;

public class ColumnDefinition
{
    public ColumnType ColumnType { get; set; }
    public double RelativeWidth { get; set; }
    public bool IsVisible { get; set; } = true;
}
