using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Components.Rendering;
using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;
using BlazorServer.Modules.Reports.Components.Views;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
using BlazorServer.Modules.Reports.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using DataColumnDefinition = BlazorServer.Modules.Reports.Domain.Views.DataSections.ColumnDefinition;
using PrintableDocumentContract = BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments.PrintableDocument;
using ReportPaginatedDocumentRenderer = BlazorServer.Modules.Reports.Components.Rendering.PrintableDocumentRenderer;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class ReportDocumentRendererTargetModelTests
{
    [Test]
    public void BuildDocument_TranslatesViewSectionsIntoPrintableDocument()
    {
        var view = ViewWithDataSection();
        view.ChangePageFormat(PageFormat.A3);

        var document = new PrintableDocumentBuilder().Build(view, RenderingContext("version-1", "snapshot-1"));
        var section = (PrintableContainer)document.BodyItems.Single();
        var firstRow = (PrintableBlock)section.Items[0];
        var dataSection = view.Sections.OfType<DataSection>().Single();

        Assert.Multiple(() =>
        {
            Assert.That(document.Id, Is.EqualTo($"report-view-{view.Id.Value}"));
            Assert.That(document.PageFormat, Is.EqualTo(PageFormat.A3));
            Assert.That(section.Id, Is.EqualTo("data-section-section-1"));
            Assert.That(section.Items, Has.Count.EqualTo(2));
            Assert.That(firstRow.Id, Is.EqualTo("data-section-section-1-row-0"));
            Assert.That(firstRow.ComponentType, Is.EqualTo(typeof(DataSectionRowComponent)));
            Assert.That(firstRow.Parameters[nameof(DataSectionRowComponent.DataSection)], Is.SameAs(dataSection));
            Assert.That(firstRow.Parameters[nameof(DataSectionRowComponent.RowDefinition)], Is.SameAs(dataSection.Rows[0]));
        });
    }

    [Test]
    public void BuildDocument_ChangesTokenWhenPrintableViewContentOrContextChanges()
    {
        var renderer = new PrintableDocumentBuilder();
        var view = ViewWithDataSection();
        var dataSection = view.Sections.OfType<DataSection>().Single();

        var original = renderer.Build(view, RenderingContext("version-1", "snapshot-1"));

        dataSection.EditCellExpression(0, 0, "changed content");
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
        view.Sections.OfType<DataSection>().Single().EditCellExpression(0, 0, "=invalid +");

        var document = new PrintableDocumentBuilder().Build(view, RenderingContext("version-1", "snapshot-1"));

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
        var document = new PrintableDocumentBuilder().Build(ViewWithDataSection(), RenderingContext("version-1", "snapshot-1"));

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
            isArchived: false,
            [SheetSection.Reconstitute(new SectionId("sheet-1"), SheetSection.DefaultColumns(), SheetKind.TenderSheet)]);

        var document = new PrintableDocumentBuilder().Build(view, RenderingContext("version-1", "snapshot-1"));
        var section = (PrintableContainer)document.BodyItems.Single();

        Assert.Multiple(() =>
        {
            Assert.That(section.Id, Is.EqualTo("sheet-section-sheet-1"));
            Assert.That(section.Items, Has.Count.EqualTo(Enum.GetValues<RowType>().Length + 1));
            Assert.That(((PrintableBlock)section.Items[0]).Id, Is.EqualTo("sheet-section-sheet-1-header"));
            Assert.That(((PrintableBlock)section.Items[0]).ComponentType, Is.EqualTo(typeof(SheetSectionHeaderRowComponent)));
            Assert.That(section.Items.Cast<PrintableBlock>().Skip(1).Select(block => block.ComponentType).Distinct(), Is.EqualTo([typeof(SheetSectionRowComponent)]));
        });
    }

    [Test]
    public async Task BuildDocument_RendersSheetSectionVisibleColumnsInConfiguredOrder()
    {
        var columns = new[]
        {
            SheetColumnConfiguration.Create(SheetColumnKind.Quantity, relativeWidth: 2),
            SheetColumnConfiguration.Create(SheetColumnKind.Description, relativeWidth: 1),
            SheetColumnConfiguration.Create(SheetColumnKind.Unit, isVisible: false),
            SheetColumnConfiguration.Create(SheetColumnKind.Reference, isVisible: false),
            SheetColumnConfiguration.Create(SheetColumnKind.Code, isVisible: false),
            SheetColumnConfiguration.Create(SheetColumnKind.Remark, isVisible: false)
        };
        var view = View.Reconstitute(
            new ViewId("sheet-view"),
            new OrganizationId("org-1"),
            "Sheet report",
            "",
            isArchived: false,
            [SheetSection.Reconstitute(new SectionId("sheet-1"), columns, SheetKind.TenderSheet)]);

        var document = new PrintableDocumentBuilder().Build(view, RenderingContext("version-1", "snapshot-1"));
        var html = await RenderDocumentAsync(document);

        Assert.Multiple(() =>
        {
            Assert.That(html, Does.Contain("data-printable-block-id=\"sheet-section-sheet-1-header\""));
            Assert.That(html.IndexOf(">Quantity<", StringComparison.Ordinal), Is.LessThan(html.IndexOf(">Description<", StringComparison.Ordinal)));
            Assert.That(html, Does.Contain("width: 66.67%"));
            Assert.That(html, Does.Contain("width: 33.33%"));
            Assert.That(html, Does.Not.Contain(">Unit<"));
        });
    }

    [Test]
    public async Task BuildDocument_RendersSelectedSheetKind()
    {
        var view = View.Reconstitute(
            new ViewId("sheet-view"),
            new OrganizationId("org-1"),
            "Sheet report",
            "",
            isArchived: false,
            [SheetSection.Reconstitute(new SectionId("sheet-1"), SheetSection.DefaultColumns(), SheetKind.SummarySheet)]);

        var document = new PrintableDocumentBuilder().Build(view, RenderingContext("version-1", "snapshot-1"));
        var html = await RenderDocumentAsync(document);

        Assert.Multiple(() =>
        {
            Assert.That(html, Does.Contain("SummarySheet"));
            Assert.That(html, Does.Not.Contain(">Group<"));
        });
    }

    [Test]
    public async Task BuildDocument_RendersRowsWhenDataCellsAreIncomplete()
    {
        var document = new PrintableDocumentBuilder().Build(
            ViewWithDataSection(includeSecondCell: false),
            RenderingContext("version-1", "snapshot-1"));

        var html = await RenderDocumentAsync(document);

        Assert.That(html, Does.Contain("first"));
    }

    private static View ViewWithDataSection(bool includeSecondCell = true)
    {
        var cells = new Dictionary<DataCellKey, DataCell>
        {
            [new(0, 0)] = new DataCell { RowIndex = 0, ColumnIndex = 0, CellExpression = "first" }
        };
        if (includeSecondCell)
        {
            cells[new DataCellKey(1, 0)] = new DataCell { RowIndex = 1, ColumnIndex = 0, CellExpression = "second" };
        }

        return View.Reconstitute(
            new ViewId("view-1"),
            new OrganizationId("org-1"),
            "Report",
            "Demo report",
            isArchived: false,
            [
                DataSection.Reconstitute(
                    new SectionId("section-1"),
                    [new RowDefinition(10), new RowDefinition(10)],
                    [new DataColumnDefinition(1)],
                    cells)
            ]);
    }

    private static ReportRenderingContext RenderingContext(
        string estimationVersionReference,
        string estimationSnapshotReference,
        string printableContentChangeToken = "content-1",
        string? viewScopedChangeSetReference = null)
        => new(estimationVersionReference, estimationSnapshotReference, printableContentChangeToken, viewScopedChangeSetReference);

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
