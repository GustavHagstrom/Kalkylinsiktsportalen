using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;
using BlazorServer.Modules.Reports.Infrastructure.MongoDb;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ReportsViewPersistenceTests
{
    [Test]
    public void ViewMappingsPreserveViewIdentitySectionsAndFormatting()
    {
        var view = new View
        {
            Id = new ViewId("view-1"),
            OrganizationId = new OrganizationId("org-1"),
            Name = "Tender report",
            Description = "For review",
            DataSections =
            [
                new DataSection
                {
                    SectionId = "data-section-1",
                    SectionIndex = 1,
                    RowsCount = 2,
                    ColumnsCount = 2,
                    DataCells =
                    {
                        ["R0C0"] = new DataCell
                        {
                            RowIndex = 0,
                            ColumnIndex = 0,
                            RawValue = "=1+2",
                            TextFormat = new TextFormat
                            {
                                Type = TextType.Number,
                                DecimalCount = 0,
                                IsBold = true
                            },
                            CellFormat = new CellFormat
                            {
                                BackgroundColor = "#ffeecc"
                            }
                        }
                    }
                }
            ],
            SheetSections =
            [
                new SheetSection
                {
                    SectionId = "sheet-section-1",
                    SectionIndex = 2,
                    Content = "TenderSheet",
                    SheetCells =
                    {
                        ["Group-Description"] = new SheetCell
                        {
                            RowType = RowType.Group,
                            ColumnType = ColumnType.Description,
                            HideValue = true
                        }
                    }
                }
            ]
        };

        var mongoView = ViewMappings.ToMongo(view);
        var roundTripped = ViewMappings.ToDomain(mongoView);

        Assert.That(roundTripped, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(mongoView.Id, Is.EqualTo("view-1"));
            Assert.That(roundTripped!.Id, Is.EqualTo(new ViewId("view-1")));
            Assert.That(roundTripped.OrganizationId, Is.EqualTo(new OrganizationId("org-1")));
            Assert.That(roundTripped.Name, Is.EqualTo("Tender report"));
            Assert.That(roundTripped.Description, Is.EqualTo("For review"));
            Assert.That(roundTripped.DataSections, Has.Count.EqualTo(1));
            Assert.That(roundTripped.DataSections[0].SectionId, Is.EqualTo("data-section-1"));
            Assert.That(roundTripped.DataSections[0].DataCells["R0C0"].RawValue, Is.EqualTo("=1+2"));
            Assert.That(roundTripped.DataSections[0].DataCells["R0C0"].TextFormat.Type, Is.EqualTo(TextType.Number));
            Assert.That(roundTripped.DataSections[0].DataCells["R0C0"].TextFormat.IsBold, Is.True);
            Assert.That(roundTripped.DataSections[0].DataCells["R0C0"].CellFormat.BackgroundColor, Is.EqualTo("#ffeecc"));
            Assert.That(roundTripped.SheetSections, Has.Count.EqualTo(1));
            Assert.That(roundTripped.SheetSections[0].SectionId, Is.EqualTo("sheet-section-1"));
            Assert.That(roundTripped.SheetSections[0].SheetCells["Group-Description"].HideValue, Is.True);
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
