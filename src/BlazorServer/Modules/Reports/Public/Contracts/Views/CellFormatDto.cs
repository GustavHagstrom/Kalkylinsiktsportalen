namespace BlazorServer.Modules.Reports.Public.Contracts.Views;

public class CellFormatDto
{
    public BorderStyleDto LeftBorder { get; set; } = new BorderStyleDto();
    public BorderStyleDto RightBorder { get; set; } = new BorderStyleDto();
    public BorderStyleDto TopBorder { get; set; } = new BorderStyleDto();
    public BorderStyleDto BottomBorder { get; set; } = new BorderStyleDto();
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public int PaddingLeft { get; set; } = 4;
    public int PaddingRight { get; set; } = 4;
    public int PaddingTop { get; set; } = 2;
    public int PaddingBottom { get; set; } = 2;
}
