using Core.Enums.ViewTemplate;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;

public class DataCellFormat : IViewEntity, IFormat
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string DataCellId { get; set; } = string.Empty;
    public DataCell? DataCell { get; set; }

    [StringLength(50)]
    public string FontFamily { get; set; } = string.Empty;
    [StringLength(50)]
    public string? BackgroundColor { get; set; }
    [StringLength(50)]
    public string? TextColor { get; set; }
    public int FontSize { get; set; } = 12;
    public bool IsBold { get; set; } = false;
    public bool IsItalic { get; set; } = false;
    public bool IsUnderline { get; set; } = false;
    public Align? HorizontalAlign { get; set; } = Align.Start;
    public Align? VerticalAlign { get; set; } = Align.End;
    public TextFormatType FormatType { get; set; } = TextFormatType.Text;
    public bool HasThoasandsSeparator { get; set; } = true;
    public int DecimalCount { get; set; } = 2;
    public bool DoesIncludeTimeOfDay { get; set; } = false;
    public bool HasBorderLeft { get; set; } = false;
    public bool HasBorderTop { get; set; } = false;
    public bool HasBorderRight { get; set; } = false;
    public bool HasBorderBottom { get; set; } = false;
    public BorderStyle BorderStyle { get; set; } = BorderStyle.Solid;

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



    //[StringLength(50)]
    //public string? SheetColumnId { get; set; } = string.Empty;
    //public SheetColumn? SheetColumn { get; set; }
    //[StringLength(50)]
    //public string? DataCellId { get; set; } = string.Empty;
    //public DataCell? DataCell { get; set; }
}
