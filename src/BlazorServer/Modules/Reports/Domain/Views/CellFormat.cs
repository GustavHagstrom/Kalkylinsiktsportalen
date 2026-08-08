namespace BlazorServer.Modules.Reports.Domain.Views;

public class CellFormat
{
    public BorderFormat LeftBorder { get; set; } = new BorderFormat();
    public BorderFormat RightBorder { get; set; } = new BorderFormat();
    public BorderFormat TopBorder { get; set; } = new BorderFormat();
    public BorderFormat BottomBorder { get; set; } = new BorderFormat();
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public int PaddingLeft { get; set; } = 4;
    public int PaddingRight { get; set; } = 4;
    public int PaddingTop { get; set; } = 2;
    public int PaddingBottom { get; set; } = 2;
}
