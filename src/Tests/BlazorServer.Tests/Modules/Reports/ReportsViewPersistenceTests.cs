using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Public.Contracts.Views;
using BlazorServer.Modules.Reports.Public.Contracts.Views.DataSection;
using BlazorServer.Modules.Reports.Public.Contracts.Views.SheetSection;
using BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ReportsViewPersistenceTests
{
    [Test]
    public void ViewMappingsPreserveViewIdentitySectionsAndFormatting()
    {
        var view = new ViewDto
        {
            Id = new ViewId("view-1"),
            OrganizationId = "org-1",
            Name = "Tender report",
            Description = "For review",
            DataSections =
            [
                new DataSectionDto
                {
                    SectionIndex = 1,
                    RowsCount = 2,
                    ColumnsCount = 2,
                    DataCells =
                    {
                        ["R0C0"] = new DataCellDto
                        {
                            RowIndex = 0,
                            ColumnIndex = 0,
                            RawValue = "=1+2",
                            TextFormat = new TextFormatDto
                            {
                                Type = TextType.Number,
                                DecimalCount = 0,
                                IsBold = true
                            },
                            CellFormat = new CellFormatDto
                            {
                                BackgroundColor = "#ffeecc"
                            }
                        }
                    }
                }
            ],
            SheetSections =
            [
                new SheetSectionDto
                {
                    SectionIndex = 2,
                    Content = "TenderSheet",
                    SheetCells =
                    {
                        ["Group-Description"] = new SheetCellDto
                        {
                            RowType = RowType.Group,
                            ColumnType = ColumnType.Description,
                            HideValue = true
                        }
                    }
                }
            ],
            PageBreakSections =
            [
                new PageBreakSectionDto { SectionIndex = 3 }
            ]
        };

        var mongoView = ViewMappings.ToMongo(view);
        var roundTripped = ViewMappings.ToDto(mongoView);

        Assert.Multiple(() =>
        {
            Assert.That(mongoView.Id, Is.EqualTo("view-1"));
            Assert.That(roundTripped.Id, Is.EqualTo(new ViewId("view-1")));
            Assert.That(roundTripped.OrganizationId, Is.EqualTo("org-1"));
            Assert.That(roundTripped.Name, Is.EqualTo("Tender report"));
            Assert.That(roundTripped.Description, Is.EqualTo("For review"));
            Assert.That(roundTripped.DataSections, Has.Count.EqualTo(1));
            Assert.That(roundTripped.DataSections[0].DataCells["R0C0"].RawValue, Is.EqualTo("=1+2"));
            Assert.That(roundTripped.DataSections[0].DataCells["R0C0"].TextFormat.Type, Is.EqualTo(TextType.Number));
            Assert.That(roundTripped.DataSections[0].DataCells["R0C0"].TextFormat.IsBold, Is.True);
            Assert.That(roundTripped.DataSections[0].DataCells["R0C0"].CellFormat.BackgroundColor, Is.EqualTo("#ffeecc"));
            Assert.That(roundTripped.SheetSections, Has.Count.EqualTo(1));
            Assert.That(roundTripped.SheetSections[0].SheetCells["Group-Description"].HideValue, Is.True);
            Assert.That(roundTripped.PageBreakSections, Has.Count.EqualTo(1));
        });
    }
}
