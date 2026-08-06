namespace BlazorServer.Modules.Reports.Domain.Views;

public class BorderStyleDto
{
    public bool IsBorder { get; set; }
    public string BorderStyle { get; set; } = "Solid";
    public string BorderColor { get; set; } = "#000000";
    public int BorderWidth { get; set; } = 1;
}
