namespace BlazorServer.Shared.DTOs.Views.SheetSection;

public class ColumnDefinition
{
    public ColumnType ColumnType { get; set; }
    public double Width { get; set; }
    public bool IsVisible { get; set; } = true;
}
