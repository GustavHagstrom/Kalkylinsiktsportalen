using Core.Enums.ViewTemplate;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;
public class SheetCellFormat : IFormat, IViewEntity
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]

    public required string SheetSectionId { get; set; }
    public SheetSection? SheetSection { get; set; }
    public required SheetRowType RowType { get; set; }
    public required SheetColumnType ColumnType { get; set; }
    public bool IsVisible { get; set; } = true;

    public Align? HorizontalAlign { get; set; } = Align.Start;
    public string? BackgroundColor { get; set; } = null;
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;
    public int DecimalCount { get; set; } = 2;
    public bool DoesIncludeTimeOfDay { get; set; } = true;
    public int FontSize { get; set; } = 12;
    public TextFormatType FormatType { get; set; } = TextFormatType.Text;
    public bool HasBorderBottom { get; set; } = false;
    public bool HasBorderLeft { get; set; } = false;
    public bool HasBorderRight { get; set; } = false;
    public bool HasBorderTop { get; set; } = false;
    public bool HasThoasandsSeparator { get; set; } = true;
    public bool IsBold { get; set; } = false;
    public bool IsItalic { get; set; } = false;
    public bool IsUnderline { get; set; } = false;
    public Align? VerticalAlign { get; set; } = Align.End;
    public string? TextColor { get; set; } = null;
    

    public void ApplyFormat(IFormat format)
    {
        HorizontalAlign = format.HorizontalAlign;
        BackgroundColor = format.BackgroundColor;
        BorderStyle = format.BorderStyle;
        DecimalCount = format.DecimalCount;
        DoesIncludeTimeOfDay = format.DoesIncludeTimeOfDay;
        FontSize = format.FontSize;
        FormatType = format.FormatType;
        HasBorderBottom = format.HasBorderBottom;
        HasBorderLeft = format.HasBorderLeft;
        HasBorderRight = format.HasBorderRight;
        HasBorderTop = format.HasBorderTop;
        HasThoasandsSeparator = format.HasThoasandsSeparator;
        IsBold = format.IsBold;
        IsItalic = format.IsItalic;
        IsUnderline = format.IsUnderline;
        VerticalAlign = format.VerticalAlign;
        TextColor = format.TextColor;
    }
}
