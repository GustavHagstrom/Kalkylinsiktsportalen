namespace BlazorServer.Modules.Reports.Domain.Views;

public class TextFormatDto
{
    public string FontFamily { get; set; } = "Calibri";
    public string Color { get; set; } = "#000000";
    public int FontSize { get; set; } = 12;
    public bool IsBold { get; set; } = false;
    public bool IsItalic { get; set; } = false;
    public bool IsUnderline { get; set; } = false;
    public Align HorizontalAlign { get; set; } = Align.Start;
    public Align VerticalAlign { get; set; } = Align.End;
    public TextType Type { get; set; } = TextType.PlainText;


    //DateTime properties
    public string DayTimeFormat { get; set; } = "yyyy-MM-dd";

    // Number properties
    public int DecimalCount { get; set; } = 2;
    public bool HasThoasandsSeparator { get; set; } = true;


}
