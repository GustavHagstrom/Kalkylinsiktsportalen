using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class ReportFormatModelTests
{
    [Test]
    public void TextStyle_NormalizesHexColorsAndRejectsAlpha()
    {
        var style = new TextStyle { TextColor = "#ffeecc" };

        Assert.Multiple(() =>
        {
            Assert.That(style.TextColor, Is.EqualTo("#FFEECC"));
            Assert.That(() => style.TextColor = "#11223344", Throws.TypeOf<ArgumentException>());
            Assert.That(() => style.TextColor = "red", Throws.TypeOf<ArgumentException>());
        });
    }

    [Test]
    public void CellLayout_UsesReportUnitsAndNoFillByDefault()
    {
        var layout = new CellLayout
        {
            BackgroundColor = "#ffeecc",
            PaddingTopMm = 1.5,
            PaddingRightMm = 2,
            PaddingBottomMm = 3,
            PaddingLeftMm = 4,
            LeftBorder = new CellBorder
            {
                WidthPt = 0.5,
                Style = BorderStyle.Dashed,
                Color = "#111111"
            }
        };

        Assert.Multiple(() =>
        {
            Assert.That(new CellLayout().BackgroundColor, Is.Null);
            Assert.That(layout.BackgroundColor, Is.EqualTo("#FFEECC"));
            Assert.That(layout.LeftBorder!.Color, Is.EqualTo("#111111"));
            Assert.That(() => layout.PaddingLeftMm = -0.1, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => layout.LeftBorder = new CellBorder { WidthPt = 0 }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
    }

    [Test]
    public void ValueFormat_RequiresExplicitNumberAndDateFormats()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ValueFormat.Text.Kind, Is.EqualTo(ValueFormatKind.Text));
            Assert.That(ValueFormat.Number(decimalCount: 6).DecimalCount, Is.EqualTo(6));
            Assert.That(ValueFormat.Date.Kind, Is.EqualTo(ValueFormatKind.Date));
            Assert.That(ValueFormat.DateTimeMinute.Kind, Is.EqualTo(ValueFormatKind.DateTimeMinute));
            Assert.That(() => ValueFormat.Number(decimalCount: 7), Throws.TypeOf<ArgumentOutOfRangeException>());
        });
    }

    [Test]
    public void DataSection_AppliesFormatPatchToRangeAndPrunesDefaultCells()
    {
        var section = DataSection.Create(2, 2, sectionIndex: 0);

        section.ApplyFormatPatch(
            DataCellRange.FromBounds(0, 0, 0, 1),
            new ReportFormatPatch
            {
                TextStyle = new TextStylePatch { IsBold = true }
            });

        Assert.Multiple(() =>
        {
            Assert.That(section.DataCells, Has.Count.EqualTo(2));
            Assert.That(section.GetCell(0, 0).TextStyle!.IsBold, Is.True);
            Assert.That(section.GetCell(0, 1).TextStyle!.IsBold, Is.True);
        });

        section.ApplyFormatPatch(
            DataCellRange.FromBounds(0, 0, 0, 1),
            new ReportFormatPatch
            {
                TextStyle = new TextStylePatch { IsBold = false }
            });

        Assert.That(section.DataCells, Is.Empty);
    }

    [Test]
    public void SheetSection_AppliesFormatPatchPerRowTypeAndColumnType()
    {
        var section = new SheetSection();

        section.ApplyFormatPatch(
            RowType.Group,
            ColumnType.Description,
            new ReportFormatPatch
            {
                CellLayout = new CellLayoutPatch { BackgroundColor = "#ffeecc" },
                ValueFormat = ValueFormat.Number(decimalCount: 0, useThousandsSeparator: false)
            });

        var cell = section.GetCellConfiguration(RowType.Group, ColumnType.Description);

        Assert.Multiple(() =>
        {
            Assert.That(cell.CellLayout!.BackgroundColor, Is.EqualTo("#FFEECC"));
            Assert.That(cell.ValueFormat!.Kind, Is.EqualTo(ValueFormatKind.Number));
        });
    }
}
