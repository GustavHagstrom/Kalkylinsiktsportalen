using BlazorServer.Modules.Reports.Domain;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;

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
            Assert.That(() => style with { TextColor = "#11223344" }, Throws.TypeOf<ArgumentException>());
            Assert.That(() => style with { TextColor = "red" }, Throws.TypeOf<ArgumentException>());
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
            Assert.That(() => layout with { PaddingLeftMm = -0.1 }, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => layout with { LeftBorder = new CellBorder { WidthPt = 0 } }, Throws.TypeOf<ArgumentOutOfRangeException>());
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
    public void FormatObjectsHaveValueEqualityAndWithCopies()
    {
        var valueFormat = ValueFormat.Number(decimalCount: 2, useThousandsSeparator: true);
        var textStyle = new TextStyle
        {
            Font = ReportFont.Arial,
            TextColor = "#ffeecc",
            FontSizePt = 12,
            IsBold = true,
            HorizontalAlignment = HorizontalTextAlignment.Center,
            VerticalAlignment = VerticalTextAlignment.Middle
        };
        var cellLayout = new CellLayout
        {
            BackgroundColor = "#112233",
            PaddingLeftMm = 1,
            LeftBorder = new CellBorder
            {
                WidthPt = 0.5,
                Style = BorderStyle.Dashed,
                Color = "#445566"
            }
        };

        var matchingValueFormat = ValueFormat.Number(decimalCount: 2, useThousandsSeparator: true);
        var matchingTextStyle = textStyle with { };
        var matchingCellLayout = cellLayout with { };
        var changedValueFormat = valueFormat with { DecimalCount = 4 };
        var changedTextStyle = textStyle with { TextColor = "#abcdef" };
        var changedCellLayout = cellLayout with { LeftBorder = cellLayout.LeftBorder! with { Color = "#778899" } };

        Assert.Multiple(() =>
        {
            Assert.That(matchingValueFormat, Is.EqualTo(valueFormat));
            Assert.That(matchingTextStyle, Is.EqualTo(textStyle));
            Assert.That(matchingCellLayout, Is.EqualTo(cellLayout));
            Assert.That(changedValueFormat, Is.Not.EqualTo(valueFormat));
            Assert.That(changedTextStyle, Is.Not.EqualTo(textStyle));
            Assert.That(changedCellLayout, Is.Not.EqualTo(cellLayout));
            Assert.That(valueFormat.DecimalCount, Is.EqualTo(2));
            Assert.That(textStyle.TextColor, Is.EqualTo("#FFEECC"));
            Assert.That(cellLayout.LeftBorder!.Color, Is.EqualTo("#445566"));
        });
    }

    [Test]
    public void DataSection_AppliesFormatPatchToRangeAndPrunesDefaultCells()
    {
        var section = DataSection.Create(2, 2);

        section.ApplyFormatPatch(
            DataCellRange.FromBounds(0, 0, 0, 1),
            new ReportFormatPatch
            {
                TextStyle = new TextStylePatch { IsBold = true }
            });

        Assert.Multiple(() =>
        {
            Assert.That(section.Cells, Has.Count.EqualTo(2));
            Assert.That(section.GetCell(0, 0).TextStyle!.IsBold, Is.True);
            Assert.That(section.GetCell(0, 1).TextStyle!.IsBold, Is.True);
        });

        section.ApplyFormatPatch(
            DataCellRange.FromBounds(0, 0, 0, 1),
            new ReportFormatPatch
            {
                TextStyle = new TextStylePatch { IsBold = false }
            });

        Assert.That(section.Cells, Is.Empty);
    }

    [Test]
    public void SheetSection_AppliesFormatPatchPerRowTypeAndSheetColumnKind()
    {
        var section = SheetSection.Create();

        section.ApplyFormatPatch(
            RowType.Group,
            SheetColumnKind.Description,
            new ReportFormatPatch
            {
                CellLayout = new CellLayoutPatch { BackgroundColor = "#ffeecc" },
                ValueFormat = ValueFormat.Number(decimalCount: 0, useThousandsSeparator: false)
            });

        var cell = section.GetCell(RowType.Group, SheetColumnKind.Description);

        Assert.Multiple(() =>
        {
            Assert.That(section.SheetCellConfigurations, Is.Not.InstanceOf<Dictionary<SheetCellKey, SheetCellConfiguration>>());
            Assert.That(section.SheetCellConfigurations.Keys.Single(), Is.EqualTo(new SheetCellKey(SheetColumnKind.Description, RowType.Group)));
            Assert.That(cell.CellLayout!.BackgroundColor, Is.EqualTo("#FFEECC"));
            Assert.That(cell.ValueFormat!.Kind, Is.EqualTo(ValueFormatKind.Number));
        });
    }

    [Test]
    public void SheetCellConfiguration_IsValueObjectLikeAndKeepsIdentityInMatrixKey()
    {
        var configuration = SheetCellConfiguration.Create(
            hideValue: true,
            textStyle: new TextStyle { IsItalic = true }
        );
        var matching = SheetCellConfiguration.Create(
            hideValue: true,
            textStyle: new TextStyle { IsItalic = true }
        );
        var changed = configuration with { HideValue = false };

        Assert.Multiple(() =>
        {
            Assert.That(configuration, Is.EqualTo(matching));
            Assert.That(changed, Is.Not.EqualTo(configuration));
            Assert.That(changed.TextStyle, Is.EqualTo(configuration.TextStyle));
            Assert.That(typeof(SheetCellConfiguration).GetProperty(nameof(RowType)), Is.Null);
            Assert.That(typeof(SheetCellConfiguration).GetProperty(nameof(SheetColumnKind)), Is.Null);
        });
    }

    [Test]
    public void SheetColumnConfiguration_IsValueObjectLikeAndRejectsNonPositiveWidths()
    {
        var configuration = SheetColumnConfiguration.Create(
            SheetColumnKind.Quantity,
            relativeWidth: 2,
            isVisible: false);

        var matching = SheetColumnConfiguration.Create(
            SheetColumnKind.Quantity,
            relativeWidth: 2,
            isVisible: false);
        var changed = configuration with { RelativeWidth = 3 };

        Assert.Multiple(() =>
        {
            Assert.That(configuration.SheetColumnKind, Is.EqualTo(SheetColumnKind.Quantity));
            Assert.That(configuration.RelativeWidth, Is.EqualTo(2));
            Assert.That(configuration.IsVisible, Is.False);
            Assert.That(matching, Is.EqualTo(configuration));
            Assert.That(changed, Is.Not.EqualTo(configuration));
            Assert.That(configuration.RelativeWidth, Is.EqualTo(2));
            Assert.That(() => SheetColumnConfiguration.Create(SheetColumnKind.Quantity, relativeWidth: 0), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => configuration with { RelativeWidth = -1 }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
    }

    [Test]
    public void SheetSection_ChangesColumnVisibilityWidthAndOrderThroughMethods()
    {
        var section = SheetSection.Create();

        section.SetColumnVisibility(SheetColumnKind.Unit, false);
        section.SetColumnRelativeWidth(SheetColumnKind.Quantity, 2.5);
        section.MoveColumn(SheetColumnKind.Quantity, 0);

        Assert.Multiple(() =>
        {
            Assert.That(section.Columns, Is.Not.InstanceOf<List<SheetColumnConfiguration>>());
            Assert.That(section.Columns[0].SheetColumnKind, Is.EqualTo(SheetColumnKind.Quantity));
            Assert.That(section.Columns.Single(column => column.SheetColumnKind == SheetColumnKind.Unit).IsVisible, Is.False);
            Assert.That(section.Columns.Single(column => column.SheetColumnKind == SheetColumnKind.Quantity).RelativeWidth, Is.EqualTo(2.5));
            Assert.That(section.Columns.Select(column => column.SheetColumnKind), Is.Unique);
            Assert.That(section.Columns, Has.Count.EqualTo(Enum.GetValues<SheetColumnKind>().Length));
        });
    }

    [Test]
    public void SheetSection_RejectsColumnCatalogsWithoutExactlyOneConfigurationPerKind()
    {
        var section = SheetSection.Create();
        var missingColumn = SheetSection.DefaultColumns()
            .Where(column => column.SheetColumnKind != SheetColumnKind.Remark)
            .ToArray();
        var duplicateColumn = SheetSection.DefaultColumns()
            .Select(column => column.SheetColumnKind == SheetColumnKind.Remark
                ? column with { SheetColumnKind = SheetColumnKind.Description }
                : column)
            .ToArray();
        var unknownColumn = SheetSection.DefaultColumns()
            .Select(column => column.SheetColumnKind == SheetColumnKind.Remark
                ? column with { SheetColumnKind = (SheetColumnKind)999 }
                : column)
            .ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(() => section.ReplaceColumnConfigurations(missingColumn), Throws.TypeOf<ReportsDomainException>()
                .With.Property(nameof(ReportsDomainException.Code)).EqualTo("sheetsection.columns.invalid"));
            Assert.That(() => section.ReplaceColumnConfigurations(duplicateColumn), Throws.TypeOf<ReportsDomainException>()
                .With.Property(nameof(ReportsDomainException.Code)).EqualTo("sheetsection.columns.duplicate"));
            Assert.That(() => section.ReplaceColumnConfigurations(unknownColumn), Throws.TypeOf<ReportsDomainException>()
                .With.Property(nameof(ReportsDomainException.Code)).EqualTo("sheetsection.columns.unknown"));
        });
    }

    [Test]
    public void SheetSection_RejectsColumnCatalogsWithNoVisibleColumns()
    {
        var section = SheetSection.Create();
        var hiddenColumns = SheetSection.DefaultColumns()
            .Select(column => column with { IsVisible = false })
            .ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(() => section.ReplaceColumnConfigurations(hiddenColumns), Throws.TypeOf<ReportsDomainException>()
                .With.Property(nameof(ReportsDomainException.Code)).EqualTo("sheetsection.columns.no_visible_columns"));
            Assert.That(() => section.SetColumnVisibility(SheetColumnKind.Description, false), Throws.Nothing);
            Assert.That(() => section.SetColumnVisibility(SheetColumnKind.Quantity, false), Throws.Nothing);
            Assert.That(() => section.SetColumnVisibility(SheetColumnKind.Unit, false), Throws.Nothing);
            Assert.That(() => section.SetColumnVisibility(SheetColumnKind.Reference, false), Throws.Nothing);
            Assert.That(() => section.SetColumnVisibility(SheetColumnKind.Code, false), Throws.Nothing);
            Assert.That(() => section.SetColumnVisibility(SheetColumnKind.Remark, false), Throws.TypeOf<ReportsDomainException>()
                .With.Property(nameof(ReportsDomainException.Code)).EqualTo("sheetsection.columns.no_visible_columns"));
        });
    }
}
