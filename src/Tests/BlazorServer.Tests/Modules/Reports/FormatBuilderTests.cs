using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Components.Rendering;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class FormatBuilderTests
{
    [Test]
    public void BuildCellStyle_MapsCellLayoutToCss()
    {
        var format = new CellLayout
        {
            BackgroundColor = "#ffffff",
            PaddingTopMm = 1,
            PaddingRightMm = 2,
            PaddingBottomMm = 3,
            PaddingLeftMm = 4,
            LeftBorder = new CellBorder
            {
                WidthPt = 2,
                Style = BorderStyle.Solid,
                Color = "#111111"
            }
        };

        var result = FormatBuilder.BuildCellStyle(format);

        Assert.That(GetCssDeclarations(result), Is.EquivalentTo(new[]
        {
            "border-left: 2pt solid #111111",
            "border-top: none",
            "border-right: none",
            "border-bottom: none",
            "background-color: #FFFFFF",
            "padding: 1mm 2mm 3mm 4mm"
        }));
    }

    [Test]
    public void BuildTextStyle_MapsTextStyleToCss()
    {
        var format = new TextStyle
        {
            TextColor = "#222222",
            FontSizePt = 12,
            Font = ReportFont.Arial,
            IsBold = true,
            IsItalic = true,
            IsUnderline = true,
            HorizontalAlignment = HorizontalTextAlignment.Right,
            VerticalAlignment = VerticalTextAlignment.Middle
        };

        var result = FormatBuilder.BuildTextStyle(format, ValueDisplayType.Text);

        Assert.That(result, Is.EqualTo("color: #222222; font-size: 12pt; font-family: Arial; font-weight: bold; font-style: italic; text-decoration: underline; text-align: right; vertical-align: middle;"));
    }

    private static IReadOnlyList<string> GetCssDeclarations(string style)
    {
        return style
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }
}
