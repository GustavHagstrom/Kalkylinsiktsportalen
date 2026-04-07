namespace BlazorServer.Shared.DTOs.Views;

public class CellFormatDto
{
    public bool HasBorderLeft { get; set; } = false;
    public bool HasBorderTop { get; set; } = false;
    public bool HasBorderRight { get; set; } = false;
    public bool HasBorderBottom { get; set; } = false;
    public string BorderStyle { get; set; } = "Solid";
    public string BorderColor { get; set; } = "#000000";
    public int BorderWidth { get; set; } = 1;
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public int PaddingLeft { get; set; } = 4;
    public int PaddingRight { get; set; } = 4;
    public int PaddingTop { get; set; } = 2;
    public int PaddingBottom { get; set; } = 2;
}
