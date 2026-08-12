using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;
using BlazorServer.Modules.Reports.Components.Views;
using BlazorServer.Modules.Reports.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using PrintableDocumentContract = BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments.PrintableDocument;
using ReportPaginatedDocumentRenderer = BlazorServer.Modules.Reports.Components.Rendering.PrintableDocumentRenderer;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class ReportDocumentRendererTests
{
    [Test]
    public void BuildDocument_TranslatesViewSectionsIntoPrintableDocument()
    {
        var view = ViewWithDataSection();
        view.ChangePageFormat(PageFormat.A3);

        var document = new PrintableDocumentBuilder().Build(
            view,
            RenderingContext("version-1", "snapshot-1"));

        Assert.Multiple(() =>
        {
            Assert.That(document.Id, Is.EqualTo($"report-view-{view.Id.Value}"));
            Assert.That(document.PageFormat, Is.EqualTo(PageFormat.A3));
            Assert.That(document.HeaderItems, Is.Empty);
            Assert.That(document.FooterItems, Is.Empty);
            Assert.That(document.BodyItems, Has.Count.EqualTo(1));
        });

        var section = (PrintableContainer)document.BodyItems.Single();
        Assert.Multiple(() =>
        {
            Assert.That(section.Id, Is.EqualTo("data-section-section-1"));
            Assert.That(section.PageBreakBehavior, Is.EqualTo(PageBreakBehavior.None));
            Assert.That(section.Items, Has.Count.EqualTo(2));
        });

        var firstRow = (PrintableBlock)section.Items[0];
        Assert.Multiple(() =>
        {
            Assert.That(firstRow.Id, Is.EqualTo("data-section-section-1-row-0"));
            Assert.That(firstRow.ComponentType, Is.EqualTo(typeof(DataSectionRowComponent)));
            Assert.That(firstRow.Parameters[nameof(DataSectionRowComponent.DataSection)], Is.SameAs(view.MutableLegacyDataSections[0]));
            Assert.That(firstRow.Parameters[nameof(DataSectionRowComponent.RowDefinition)], Is.SameAs(view.MutableLegacyDataSections[0].RowDefinitions[0]));
        });
    }

    [Test]
    public void BuildDocument_ChangesTokenWhenPrintableViewContentOrContextChanges()
    {
        var renderer = new PrintableDocumentBuilder();
        var view = ViewWithDataSection();

        var original = renderer.Build(view, RenderingContext("version-1", "snapshot-1"));

        view.MutableLegacyDataSections[0].DataCells["R0C0"].RawValue = "changed height";
        var changedView = renderer.Build(view, RenderingContext("version-1", "snapshot-1"));
        var changedContext = renderer.Build(view, RenderingContext("version-1", "snapshot-2"));

        Assert.Multiple(() =>
        {
            Assert.That(changedView.ChangeToken, Is.Not.EqualTo(original.ChangeToken));
            Assert.That(changedContext.ChangeToken, Is.Not.EqualTo(changedView.ChangeToken));
        });
    }

    [Test]
    public void BuildDocument_AddsPrintBlockingIssueForDataCellEvaluationErrors()
    {
        var view = ViewWithDataSection();
        view.MutableLegacyDataSections[0].EditCellExpression(0, 0, "=invalid +");

        var document = new PrintableDocumentBuilder().Build(
            view,
            RenderingContext("version-1", "snapshot-1"));

        Assert.Multiple(() =>
        {
            Assert.That(document.PrintBlockingIssues, Has.Count.EqualTo(1));
            Assert.That(document.PrintBlockingIssues[0].Code, Is.EqualTo("CellEvaluationError"));
            Assert.That(document.PrintBlockingIssues[0].BlockId, Is.EqualTo("data-section-section-1-row-0"));
        });
    }

    [Test]
    public async Task BuildDocument_CanRenderThroughPrintableDocumentDynamicComponents()
    {
        var document = new PrintableDocumentBuilder().Build(
            ViewWithDataSection(),
            RenderingContext("version-1", "snapshot-1"));

        var html = await RenderDocumentAsync(document);

        Assert.Multiple(() =>
        {
            Assert.That(html, Does.Contain("first"));
            Assert.That(html, Does.Contain("second"));
            Assert.That(html, Does.Contain("data-printable-block-id=\"data-section-section-1-row-0\""));
            Assert.That(html, Does.Contain("data-printable-block-id=\"data-section-section-1-row-1\""));
        });
    }

    [Test]
    public void BuildDocument_EmitsSheetSectionsAsBreakableRowBlocks()
    {
        var view = View.Reconstitute(
            new ViewId("sheet-view"),
            new OrganizationId("org-1"),
            "Sheet report",
            "",
            enableChanges: false,
            isArchived: false,
            [],
            [new SheetSection { SectionId = "sheet-1", SectionIndex = 0, Content = "Tender sheet" }]);

        var document = new PrintableDocumentBuilder().Build(
            view,
            RenderingContext("version-1", "snapshot-1"));

        var section = (PrintableContainer)document.BodyItems.Single();

        Assert.Multiple(() =>
        {
            Assert.That(section.Id, Is.EqualTo("sheet-section-sheet-1"));
            Assert.That(section.Items, Has.Count.EqualTo(Enum.GetValues<RowType>().Length));
            Assert.That(section.Items.Cast<PrintableBlock>().Select(block => block.ComponentType).Distinct(), Is.EqualTo([typeof(SheetSectionRowComponent)]));
        });
    }

    [Test]
    public async Task BuildDocument_RendersRowsWhenDataCellsAreIncomplete()
    {
        var view = ViewWithDataSection();
        view.MutableLegacyDataSections[0].DataCells.Remove("R1C0");
        var document = new PrintableDocumentBuilder().Build(
            view,
            RenderingContext("version-1", "snapshot-1"));

        var html = await RenderDocumentAsync(document);

        Assert.That(html, Does.Contain("first"));
    }

    private static View ViewWithDataSection()
    {
        var view = View.Reconstitute(
            new ViewId("view-1"),
            new OrganizationId("org-1"),
            "Report",
            "Demo report",
            enableChanges: false,
            isArchived: false,
            dataSections:
            [
                new DataSection
                {
                    SectionId = "section-1",
                    SectionIndex = 0,
                    RowsCount = 2,
                    ColumnsCount = 1,
                    RowDefinitions =
                    [
                        new RowDefinition { RowIndex = 0, Height = 10 },
                        new RowDefinition { RowIndex = 1, Height = 10 }
                    ],
                    ColumnDefinitions =
                    [
                        new DataColumnDefinition { ColumnIndex = 0, Width = 100 }
                    ],
                    DataCells =
                    {
                        ["R0C0"] = new DataCell { RawValue = "first" },
                        ["R1C0"] = new DataCell { RawValue = "second" }
                    }
                }
            ],
            sheetSections: []);

        return view;
    }

    private static ReportRenderingContext RenderingContext(
        string estimationVersionReference,
        string estimationSnapshotReference,
        string printableContentChangeToken = "content-1",
        string? viewScopedChangeSetReference = null)
        => new(
            estimationVersionReference,
            estimationSnapshotReference,
            printableContentChangeToken,
            viewScopedChangeSetReference);

    private static async Task<string> RenderDocumentAsync(PrintableDocumentContract document)
    {
        var services = new ServiceCollection()
            .AddSingleton<IJSRuntime, TestJsRuntime>()
            .BuildServiceProvider();
        using var serviceProvider = services;
        await using var renderer = new HtmlRenderer(serviceProvider, NullLoggerFactory.Instance);

        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await renderer.RenderComponentAsync<ReportPaginatedDocumentRenderer>(
                ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    ["Document"] = document,
                    ["ShowPaginationOverlay"] = false
                }));

            return output.ToHtmlString();
        });
    }

    private sealed class TestJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => throw new NotSupportedException("The printable document renderer test does not invoke JavaScript.");

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => throw new NotSupportedException("The printable document renderer test does not invoke JavaScript.");
    }
}
