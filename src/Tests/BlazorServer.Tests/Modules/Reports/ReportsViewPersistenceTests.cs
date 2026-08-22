using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
using BlazorServer.Modules.Reports.Infrastructure.MongoDb;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public class ReportsViewPersistenceTests
{
    [Test]
    public void ViewMappingsPreserveViewIdentitySectionsAndFormatting()
    {
        var view = View.Reconstitute(
            new ViewId("view-1"),
            new OrganizationId("org-1"),
            "Tender report",
            "For review",
            PageFormat.A5,
            enableChanges: true,
            isArchived: true,
            dataSections:
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
                            ValueFormat = ValueFormat.Number(decimalCount: 0, useThousandsSeparator: false),
                            TextStyle = new TextStyle
                            {
                                IsBold = true
                            },
                            CellLayout = new CellLayout
                            {
                                BackgroundColor = "#ffeecc"
                            }
                        }
                    }
                }
            ],
            sheetSections:
            [
                new SheetSection
                {
                    SectionId = "sheet-section-1",
                    SectionIndex = 2,
                    Content = "TenderSheet",
                    EnableChanges = true,
                    SheetCells =
                    {
                        ["Group-Description"] = new SheetCell
                        {
                            RowType = RowType.Group,
                            ColumnType = ColumnType.Description,
                            HideValue = true,
                            TextStyle = new TextStyle
                            {
                                IsItalic = true
                            }
                        }
                    }
                }
            ]);

        var mongoView = ViewMappings.ToMongo(view);
        var roundTripped = ViewMappings.ToDomain(mongoView);

        Assert.That(roundTripped, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(mongoView.Id, Is.EqualTo("view-1"));
            Assert.That(mongoView.EnableChanges, Is.False);
            Assert.That(roundTripped!.Id, Is.EqualTo(new ViewId("view-1")));
            Assert.That(roundTripped.OrganizationId, Is.EqualTo(new OrganizationId("org-1")));
            Assert.That(roundTripped.Name, Is.EqualTo("Tender report"));
            Assert.That(roundTripped.Description, Is.EqualTo("For review"));
            Assert.That(roundTripped.PageFormat, Is.EqualTo(PageFormat.A5));
            Assert.That(roundTripped.IsArchived, Is.True);
            Assert.That(roundTripped.LegacyDataSections, Has.Count.EqualTo(1));
            Assert.That(roundTripped.LegacyDataSections[0].SectionId, Is.EqualTo("data-section-1"));
            Assert.That(roundTripped.LegacyDataSections[0].DataCells["R0C0"].RawValue, Is.EqualTo("=1+2"));
            Assert.That(roundTripped.LegacyDataSections[0].DataCells["R0C0"].ValueFormat!.Kind, Is.EqualTo(ValueFormatKind.Number));
            Assert.That(roundTripped.LegacyDataSections[0].DataCells["R0C0"].ValueFormat!.DecimalCount, Is.EqualTo(0));
            Assert.That(roundTripped.LegacyDataSections[0].DataCells["R0C0"].TextStyle!.IsBold, Is.True);
            Assert.That(roundTripped.LegacyDataSections[0].DataCells["R0C0"].CellLayout!.BackgroundColor, Is.EqualTo("#FFEECC"));
            Assert.That(roundTripped.LegacySheetSections, Has.Count.EqualTo(1));
            Assert.That(roundTripped.LegacySheetSections[0].SectionId, Is.EqualTo("sheet-section-1"));
            Assert.That(roundTripped.LegacySheetSections[0].EnableChanges, Is.True);
            Assert.That(roundTripped.LegacySheetSections[0].SheetCells["Group-Description"].HideValue, Is.True);
            Assert.That(roundTripped.LegacySheetSections[0].SheetCells["Group-Description"].TextStyle!.IsItalic, Is.True);
        });
    }

    [Test]
    public void ViewMappingsHydrateLegacyFormattingToTargetModel()
    {
        var mongoView = new MongoView
        {
            Id = "view-1",
            OrganizationId = "org-1",
            Name = "Tender report",
            DataSections =
            [
                new DataSection
                {
                    SectionId = "data-section-1",
                    RowsCount = 1,
                    ColumnsCount = 1,
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
            ]
        };

        var view = ViewMappings.ToDomain(mongoView);
        var cell = view!.LegacyDataSections.Single().DataCells["R0C0"];

        Assert.Multiple(() =>
        {
            Assert.That(cell.ValueFormat!.Kind, Is.EqualTo(ValueFormatKind.Number));
            Assert.That(cell.ValueFormat.DecimalCount, Is.EqualTo(0));
            Assert.That(cell.TextStyle!.IsBold, Is.True);
            Assert.That(cell.CellLayout!.BackgroundColor, Is.EqualTo("#FFEECC"));
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
