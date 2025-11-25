using Core.Models.EstimationView;
using Core.Enums.ViewTemplate;
using System.Globalization;

namespace SharedModule.Services.Views;
public class ViewStyleService : IViewStyleService
{
    //public string CreateSheetColumnStyles(SheetColumn column, int allColumnsWidthSum)
    //{
    //    //var classString = $".{name} {{";
    //    var classString = $"width: {((double)column.Width / allColumnsWidthSum * 100).ToString(CultureInfo.InvariantCulture)}%;\n";
    //    classString += CreateFormatStyles(column); 
    //  /*  classString += "}"*/;
    //    return classString;
    //}
    public string CreateFormatStyles(IFormat format)
    {
        var styleString = string.Empty;
        styleString += CreateColorStyles(format);
        styleString +=  CreateTextStyles(format);
        styleString += CreateBorderStyles(format);

        //if (    format.Padding is not null)
        //{
        //    classString += $"padding: {format.Padding};";
        //}
        //if (format.Margin is not null)
        //{
        //    classString += $"margin: {format.Margin};";
        //}
        return styleString;
    }

    private string CreateBorderStyles(IFormat format)
    {
        string classString = string.Empty;
        string borderStyle = GetBorderStyle(format.BorderStyle);
        if (format.HasBorderLeft)
        {
            classString += $"border-left: 1px {borderStyle} black;";
        }
        if (format.HasBorderTop)
        {
            classString += $"border-top: 1px {borderStyle} black;";
        }
        if (format.HasBorderRight)
        {
            classString += $"border-right: 1px {borderStyle} black;";
        }
        if (format.HasBorderBottom)
        {
            classString += $"border-bottom: 1px {borderStyle} black;";
        }
        return classString;
    }

    private string CreateTextStyles(IFormat format)
    {
        var classString = $"font-size: {format.FontSize}px;\n";
        //classString += $"font-family: {format.FontFamily};\n";
        classString += format.HorizontalAlign is null ? string.Empty : $"justify-content: {GetAlign(format.HorizontalAlign.Value)};";
        classString += format.HorizontalAlign is null ? string.Empty : $"text-align: {GetAlign(format.HorizontalAlign.Value)};";
        classString += format.VerticalAlign is null ? string.Empty : $"align-items: {GetAlign(format.VerticalAlign.Value)};";

        if (format.IsBold)
        {
            classString += $"font-weight: 600;";
        }
        else
        {
            classString += $"font-weight: 400;";
        }
        if (format.IsItalic)
        {
            classString += $"font-style: italic;";
        }
        if (format.IsUnderline)
        {
            classString += $"text-decoration: underline;";
        }
        return classString;
    }

    private string CreateColorStyles(IFormat format)
    {
        string classString = string.Empty;
        if (format.BackgroundColor is not null)
        {
            classString += $"background-color: {format.BackgroundColor};";
        }
        if (format.TextColor is not null)
        {
            classString += $"color: {format.TextColor};";
        }
        return classString;
    }
    private string GetHorizontalAlign(Align align)
    {
        return align switch
        {
            Align.Start => "start",
            Align.Center => "center",
            Align.End => "end",
            _ => "start"
        };
    }
    private string GetAlign(Align align)
    {
        return align switch
        {
            Align.Start => "start",
            Align.Center => "center",
            Align.End => "end",
            _ => "start"
        };
    }
    private string GetBorderStyle(BorderStyle style)
    {
        return style switch
        {
            BorderStyle.Solid => "solid",
            BorderStyle.Dashed => "dashed",
            BorderStyle.Dotted => "dotted",
            BorderStyle.Double => "double",
            _ => "solid"
        };
    }

}
