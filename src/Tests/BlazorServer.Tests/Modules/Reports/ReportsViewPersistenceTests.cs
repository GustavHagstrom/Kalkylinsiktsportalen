using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
using BlazorServer.Modules.Reports.Persistence.MongoDb;
using BlazorServer.Modules.Reports.Public;
using DataColumnDefinition = BlazorServer.Modules.Reports.Domain.Views.DataSections.ColumnDefinition;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ReportsViewPersistenceTargetModelTests
{
    [Test]
    public void ViewMappingsPreserveViewIdentitySectionsAndFormatting()
    {
        var view = View.Reconstitute(
            new ViewId("view-1"),
            new OrganizationId("org-1"),
            "Tender report",
            "For review",
            isArchived: true,
            [
                DataSection.Reconstitute(
                    new SectionId("data-section-1"),
                    [new RowDefinition(8), new RowDefinition(8)],
                    [new DataColumnDefinition(1), new DataColumnDefinition(1)],
                    new Dictionary<DataCellKey, DataCell>
                    {
                        [new(0, 0)] = new DataCell
                        {
                            RowIndex = 0,
                            ColumnIndex = 0,
                            CellExpression = "=1+2",
                            ValueFormat = ValueFormat.Number(decimalCount: 0, useThousandsSeparator: false),
                            TextStyle = new TextStyle { IsBold = true },
                            CellLayout = new CellLayout { BackgroundColor = "#ffeecc" }
                        }
                    }),
                SheetSection.Reconstitute(
                    new SectionId("sheet-section-1"),
                    SheetSection.DefaultColumns(),
                    SheetKind.SummarySheet,
                    enableChanges: true,
                    sheetCells: new Dictionary<SheetCellKey, SheetCell>
                    {
                        [new(ColumnType.Description, RowType.Group)] = new SheetCell
                        {
                            RowType = RowType.Group,
                            ColumnType = ColumnType.Description,
                            HideValue = true,
                            TextStyle = new TextStyle { IsItalic = true }
                        }
                    })
            ],
            PageFormat.A5);

        var mongoView = ViewMappings.ToMongo(view);
        var roundTripped = ViewMappings.ToDomain(mongoView);
        var roundTrippedDataCell = roundTripped!.Sections.OfType<DataSection>().Single().GetCell(0, 0);
        var roundTrippedSheetSection = roundTripped.Sections.OfType<SheetSection>().Single();
        var roundTrippedSheetCell = roundTrippedSheetSection.GetCell(RowType.Group, ColumnType.Description);

        Assert.Multiple(() =>
        {
            Assert.That(mongoView.Id, Is.EqualTo("view-1"));
            Assert.That(mongoView.EnableChanges, Is.False);
            Assert.That(mongoView.SheetSections.Single().SheetKind, Is.EqualTo(SheetKind.SummarySheet));
            Assert.That(roundTripped.Id, Is.EqualTo(new ViewId("view-1")));
            Assert.That(roundTripped.OrganizationId, Is.EqualTo(new OrganizationId("org-1")));
            Assert.That(roundTripped.Name, Is.EqualTo("Tender report"));
            Assert.That(roundTripped.Description, Is.EqualTo("For review"));
            Assert.That(roundTripped.PageFormat, Is.EqualTo(PageFormat.A5));
            Assert.That(roundTripped.IsArchived, Is.True);
            Assert.That(roundTrippedDataCell.CellExpression, Is.EqualTo("=1+2"));
            Assert.That(roundTrippedDataCell.ValueFormat!.Kind, Is.EqualTo(ValueFormatKind.Number));
            Assert.That(roundTrippedDataCell.ValueFormat.DecimalCount, Is.EqualTo(0));
            Assert.That(roundTrippedDataCell.TextStyle!.IsBold, Is.True);
            Assert.That(roundTrippedDataCell.CellLayout!.BackgroundColor, Is.EqualTo("#FFEECC"));
            Assert.That(roundTrippedSheetSection.SectionId.Value, Is.EqualTo("sheet-section-1"));
            Assert.That(roundTrippedSheetSection.SheetKind, Is.EqualTo(SheetKind.SummarySheet));
            Assert.That(roundTrippedSheetSection.EnableChanges, Is.True);
            Assert.That(roundTrippedSheetCell.HideValue, Is.True);
            Assert.That(roundTrippedSheetCell.TextStyle!.IsItalic, Is.True);
        });
    }

    [Test]
    public void ViewMappingsReturnNullForMissingMongoView()
    {
        MongoView? mongoView = null;

        var view = ViewMappings.ToDomain(mongoView);

        Assert.That(view, Is.Null);
    }
}
