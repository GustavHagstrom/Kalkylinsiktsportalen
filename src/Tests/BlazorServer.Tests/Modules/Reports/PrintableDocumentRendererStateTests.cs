using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Components.Rendering;
using PrintableDocumentContract = BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments.PrintableDocument;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class PrintableDocumentRendererStateTests
{
    [Test]
    public void SetDocument_MarksPaginationStaleOnlyWhenPaginationKeyChanges()
    {
        var state = new PrintableDocumentRendererState();
        var document = Document("document-1", "change-1", PageFormat.A4);

        state.SetDocument(document);
        state.MarkPaginating();
        state.MarkReady(EmptyPagination());

        state.SetDocument(Document("document-1", "change-1", PageFormat.A4));
        Assert.That(state.NeedsMeasurement, Is.False);

        state.SetDocument(Document("document-2", "change-1", PageFormat.A4));
        Assert.That(state.NeedsMeasurement, Is.True);

        state.MarkPaginating();
        state.MarkReady(EmptyPagination());
        state.SetDocument(Document("document-2", "change-2", PageFormat.A4));
        Assert.That(state.NeedsMeasurement, Is.True);

        state.MarkPaginating();
        state.MarkReady(EmptyPagination());
        state.SetDocument(Document("document-2", "change-2", PageFormat.A3));
        Assert.That(state.NeedsMeasurement, Is.True);
    }

    [Test]
    public void MarkReady_ExposesReadyPagesOrBlockingErrors()
    {
        var state = new PrintableDocumentRendererState();
        state.SetDocument(Document("document-1", "change-1", PageFormat.A4));
        state.MarkPaginating();

        state.MarkReady(new PrintablePaginationResult(
            Pages:
            [
                new PrintablePage(
                    PageNumber: 1,
                    BodyHeightMm: 100,
                    BodyBlocks: [new PrintableBlockPlacement("block-1", TopMm: 0, HeightMm: 50, PageBreakBehavior.None)])
            ],
            Warnings: [],
            Errors: []));

        Assert.Multiple(() =>
        {
            Assert.That(state.Status, Is.EqualTo(PrintableRendererStatus.Ready));
            Assert.That(state.Pages.SelectMany(page => page.BodyBlocks).Select(block => block.BlockId), Is.EqualTo(new[] { "block-1" }));
            Assert.That(state.Errors, Is.Empty);
        });

        state.SetDocument(Document("document-1", "change-2", PageFormat.A4));
        state.MarkPaginating();
        state.MarkReady(new PrintablePaginationResult(
            Pages: [],
            Warnings: [],
            Errors:
            [
                new PaginationIssue(PaginationIssueCode.BlockOverflow, "Block is too tall.", "block-1")
            ]));

        Assert.Multiple(() =>
        {
            Assert.That(state.Status, Is.EqualTo(PrintableRendererStatus.Error));
            Assert.That(state.Pages, Is.Empty);
            Assert.That(state.Errors.Single().Code, Is.EqualTo(PaginationIssueCode.BlockOverflow));
        });
    }

    [Test]
    public void MarkReady_KeepsPlacementGeometryForReadyPreview()
    {
        var state = new PrintableDocumentRendererState();
        state.SetDocument(Document("document-1", "change-1", PageFormat.A4));
        state.MarkPaginating();

        state.MarkReady(new PrintablePaginationResult(
            Pages:
            [
                new PrintablePage(
                    PageNumber: 1,
                    BodyHeightMm: 120,
                    BodyBlocks:
                    [
                        new PrintableBlockPlacement("block-1", TopMm: 0, HeightMm: 40, PageBreakBehavior.None),
                        new PrintableBlockPlacement("block-2", TopMm: 40, HeightMm: 80, PageBreakBehavior.After)
                    ])
            ],
            Warnings: [],
            Errors: []));

        Assert.Multiple(() =>
        {
            Assert.That(state.Pages.Single().BodyHeightMm, Is.EqualTo(120));
            Assert.That(state.Pages.Single().BodyBlocks.Select(block => block.TopMm), Is.EqualTo(new[] { 0m, 40m }));
            Assert.That(state.Pages.Single().BodyBlocks.Select(block => block.HeightMm), Is.EqualTo(new[] { 40m, 80m }));
        });
    }

    [Test]
    public void RequestRefresh_MarksCurrentDocumentStaleWithoutChangingTheDocumentKey()
    {
        var state = new PrintableDocumentRendererState();
        state.SetDocument(Document("document-1", "change-1", PageFormat.A4));
        state.MarkPaginating();
        state.MarkReady(EmptyPagination());

        state.RequestRefresh();

        Assert.Multiple(() =>
        {
            Assert.That(state.Status, Is.EqualTo(PrintableRendererStatus.Measuring));
            Assert.That(state.NeedsMeasurement, Is.True);
        });
    }

    [Test]
    public void GetPrintReadiness_AllowsPrintOnlyWhenCurrentPreviewIsReadyAndValid()
    {
        var state = new PrintableDocumentRendererState();
        state.SetDocument(Document("document-1", "change-1", PageFormat.A4));
        Assert.That(state.GetPrintReadiness().Status, Is.EqualTo(PrintableDocumentPrintStatus.NotReady));

        state.MarkPaginating();
        Assert.That(state.GetPrintReadiness().Status, Is.EqualTo(PrintableDocumentPrintStatus.NotReady));

        state.MarkReady(EmptyPagination());
        Assert.That(state.GetPrintReadiness().Status, Is.EqualTo(PrintableDocumentPrintStatus.Ready));

        state.SetDocument(Document("document-1", "change-2", PageFormat.A4));
        Assert.That(state.GetPrintReadiness().Status, Is.EqualTo(PrintableDocumentPrintStatus.NotReady));

        state.MarkPaginating();
        state.MarkReady(new PrintablePaginationResult(
            Pages: [],
            Warnings: [],
            Errors: [new PaginationIssue(PaginationIssueCode.BlockOverflow, "Block is too tall.", "block-1")]));

        Assert.Multiple(() =>
        {
            var readiness = state.GetPrintReadiness();
            Assert.That(readiness.Status, Is.EqualTo(PrintableDocumentPrintStatus.BlockedByPaginationErrors));
            Assert.That(readiness.Errors.Single().Code, Is.EqualTo(nameof(PaginationIssueCode.BlockOverflow)));
        });
    }

    [Test]
    public void MarkError_PreventsReadyPagesAfterMeasurementFailure()
    {
        var state = new PrintableDocumentRendererState();
        state.SetDocument(Document("document-1", "change-1", PageFormat.A4));
        state.MarkPaginating();

        state.MarkError(new PaginationIssue(PaginationIssueCode.MeasurementFailed, "Could not measure document."));

        Assert.Multiple(() =>
        {
            Assert.That(state.Status, Is.EqualTo(PrintableRendererStatus.Error));
            Assert.That(state.Pages, Is.Empty);
            Assert.That(state.Errors.Single().Code, Is.EqualTo(PaginationIssueCode.MeasurementFailed));
            Assert.That(state.NeedsMeasurement, Is.False);
        });
    }

    private static PrintableDocumentContract Document(string id, string changeToken, PageFormat pageFormat)
        => new(
            id,
            changeToken,
            pageFormat,
            HeaderItems: [],
            BodyItems: [],
            FooterItems: []);

    private static PrintablePaginationResult EmptyPagination()
        => new(Pages: [], Warnings: [], Errors: []);
}
