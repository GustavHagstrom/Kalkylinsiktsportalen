using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Components.Rendering;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class FormatBuilderTests
{
    [Test]
    public void BuildCellStyle_MapsCellFormatToCss()
    {
        var format = new CellFormat
        {
            BackgroundColor = "#ffffff",
            PaddingTop = 1,
            PaddingRight = 2,
            PaddingBottom = 3,
            PaddingLeft = 4,
            LeftBorder = new BorderFormat
            {
                IsBorder = true,
                BorderWidth = 2,
                BorderStyle = "solid",
                BorderColor = "#111111"
            }
        };

        var result = FormatBuilder.BuildCellStyle(format);

        Assert.That(GetCssDeclarations(result), Is.EquivalentTo(new[]
        {
            "border-left: 2px solid #111111",
            "border-top: none",
            "border-right: none",
            "border-bottom: none",
            "background-color: #ffffff",
            "padding: 1px 2px 3px 4px"
        }));
    }

    [Test]
    public void BuildTextStyle_MapsTextFormatToCss()
    {
        var format = new TextFormat
        {
            Color = "#222222",
            FontSize = 12,
            FontFamily = "Arial",
            IsBold = true,
            IsItalic = true,
            IsUnderline = true,
            HorizontalAlign = Align.End,
            VerticalAlign = Align.Center
        };

        var result = FormatBuilder.BuildTextStyle(format);

        Assert.That(result, Is.EqualTo("color: #222222; font-size: 12px; font-family: Arial; font-weight: bold; font-style: italic; text-decoration: underline; text-align: right; vertical-align: middle;"));
    }

    private static IReadOnlyList<string> GetCssDeclarations(string style)
    {
        return style
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }
}
