using BlazorServer.Modules.Reports.Components.Rendering;
using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class ReportFormatRenderingTests
{
    [Test]
    public void BuildCellStyle_MapsCellLayoutToCssWithoutFreeCssInDomain()
    {
        var layout = new CellLayout
        {
            BackgroundColor = "#ffeecc",
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

        var result = FormatBuilder.BuildCellStyle(layout);

        Assert.That(GetCssDeclarations(result), Is.EquivalentTo(new[]
        {
            "border-left: 2pt solid #111111",
            "border-top: none",
            "border-right: none",
            "border-bottom: none",
            "background-color: #FFEECC",
            "padding: 1mm 2mm 3mm 4mm"
        }));
    }

    [Test]
    public void BuildTextStyle_MapsTextStyleToCssAndOmitsAutoValues()
    {
        var style = new TextStyle
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

        var result = FormatBuilder.BuildTextStyle(style, ValueDisplayType.Text);

        Assert.That(GetCssDeclarations(result), Is.EquivalentTo(new[]
        {
            "color: #222222",
            "font-size: 12pt",
            "font-family: Arial",
            "font-weight: bold",
            "font-style: italic",
            "text-decoration: underline",
            "text-align: right",
            "vertical-align: middle"
        }));
    }

    [Test]
    public void BuildTextStyle_DefaultHorizontalAlignmentFollowsDisplayType()
    {
        var result = FormatBuilder.BuildTextStyle(null, ValueDisplayType.Number);

        Assert.That(GetCssDeclarations(result), Does.Contain("text-align: right"));
    }

    private static IReadOnlyList<string> GetCssDeclarations(string style)
    {
        return style
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }
}
