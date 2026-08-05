using BlazorServer.Modules.Reports.Domain.PrintableDocuments;
using BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class PrintableDocumentPaginationTests
{
    [Test]
    public void PrintableDocumentModel_DefensivelyCopiesMutableInputs()
    {
        var bodyItems = new List<PrintableItem> { Block("original") };
        var parameters = new Dictionary<string, object?> { ["text"] = "before" };
        var containerItems = new List<PrintableItem> { Block("nested-original") };

        var document = CreateDocument(bodyItems);
        var block = new PrintableBlock("parameterized", typeof(TestPrintableComponent), parameters);
        var container = new PrintableContainer("container", containerItems);
        bodyItems.Add(Block("mutated"));
        parameters["text"] = "after";
        containerItems.Add(Block("nested-mutated"));

        Assert.Multiple(() =>
        {
            Assert.That(document.BodyItems.Select(item => item.Id), Is.EqualTo(new[] { "original" }));
            Assert.That(block.Parameters["text"], Is.EqualTo("before"));
            Assert.That(container.Items.Select(item => item.Id), Is.EqualTo(new[] { "nested-original" }));
        });
    }

    [Test]
    public void PrintableDocumentModel_RequiresIdentityChangeTokenAndValidPageFormat()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => CreateDocument([], id: ""));
            Assert.Throws<ArgumentException>(() => CreateDocument([], changeToken: ""));
            Assert.Throws<ArgumentException>(() => new PageFormat("Invalid", WidthMm: 0, HeightMm: 100));
            Assert.Throws<ArgumentException>(() => new PageFormat("Invalid", WidthMm: 100, HeightMm: 0));
            Assert.Throws<ArgumentException>(() => new PrintableBlockMeasurement("", HeightMm: 10));
            Assert.Throws<ArgumentException>(() => new PrintableBlockMeasurement("block", HeightMm: 0));
            Assert.Throws<ArgumentNullException>(() => new PrintableDocumentMeasurements(
                HeaderHeightMm: 10,
                FooterHeightMm: 10,
                BodyBlocks: null!));
        });
    }

    [Test]
    public void Paginate_RequiresDocumentAndMeasurements()
    {
        var paginator = new PrintableDocumentPaginator();
        var document = CreateDocument([]);
        var measurements = Measurements();

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() => paginator.Paginate(null!, measurements));
            Assert.Throws<ArgumentNullException>(() => paginator.Paginate(document, null!));
        });
    }

    [Test]
    public void Paginate_PlacesBlocksThatFitOnOnePage()
    {
        var document = CreateDocument(
            [
                Block("intro"),
                Block("summary")
            ]);
        var measurements = new PrintableDocumentMeasurements(
            HeaderHeightMm: 10,
            FooterHeightMm: 10,
            BodyBlocks:
            [
                new PrintableBlockMeasurement("intro", 40),
                new PrintableBlockMeasurement("summary", 30)
            ]);
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Pages, Has.Count.EqualTo(1));
            Assert.That(result.Pages[0].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "intro", "summary" }));
        });
    }

    [Test]
    public void Paginate_ContinuesOnNextPageWhenNextBlockDoesNotFit()
    {
        var document = CreateDocument(
            [
                Block("first"),
                Block("second"),
                Block("third")
            ]);
        var measurements = new PrintableDocumentMeasurements(
            HeaderHeightMm: 10,
            FooterHeightMm: 10,
            BodyBlocks:
            [
                new PrintableBlockMeasurement("first", 50),
                new PrintableBlockMeasurement("second", 30),
                new PrintableBlockMeasurement("third", 10)
            ]);
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Pages, Has.Count.EqualTo(2));
            Assert.That(result.Pages[0].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "first", "second" }));
            Assert.That(result.Pages[1].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "third" }));
        });
    }

    [Test]
    public void Paginate_FlattensContainersIntoPlacementBlocks()
    {
        var document = CreateDocument(
            [
                Block("before"),
                new PrintableContainer(
                    "group",
                    [
                        Block("nested-1"),
                        new PrintableContainer("nested-group", [Block("nested-2")])
                    ]),
                Block("after")
            ]);
        var measurements = Measurements(("before", 10), ("nested-1", 10), ("nested-2", 10), ("after", 10));
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.That(
            result.Pages.Single().BodyBlocks.Select(block => block.BlockId),
            Is.EqualTo(new[] { "before", "nested-1", "nested-2", "after" }));
    }

    [Test]
    public void Paginate_PreservesContainerPageBreakAfterOnLastPlacementDescendant()
    {
        var document = CreateDocument(
            [
                new PrintableContainer(
                    "group",
                    [
                        Block("nested-1"),
                        new PrintableContainer("nested-group", [Block("nested-2")])
                    ],
                    PageBreakBehavior.After),
                Block("after")
            ]);
        var measurements = Measurements(("nested-1", 10), ("nested-2", 10), ("after", 10));
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Pages, Has.Count.EqualTo(2));
            Assert.That(result.Pages[0].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "nested-1", "nested-2" }));
            Assert.That(result.Pages[0].BodyBlocks.Last().PageBreakBehavior, Is.EqualTo(PageBreakBehavior.After));
            Assert.That(result.Pages[1].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "after" }));
        });
    }

    [Test]
    public void Paginate_StartsNextPageAfterBlockWithPageBreakAfter()
    {
        var document = CreateDocument(
            [
                Block("section", PageBreakBehavior.After),
                Block("next-section")
            ]);
        var measurements = Measurements(("section", 10), ("next-section", 10));
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Pages, Has.Count.EqualTo(2));
            Assert.That(result.Pages[0].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "section" }));
            Assert.That(result.Pages[1].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "next-section" }));
        });
    }

    [Test]
    public void Paginate_ReservesHeaderAndFooterHeightBeforePlacingBodyBlocks()
    {
        var document = CreateDocument(
            [
                Block("first"),
                Block("second")
            ]);
        var measurements = new PrintableDocumentMeasurements(
            HeaderHeightMm: 20,
            FooterHeightMm: 30,
            BodyBlocks:
            [
                new PrintableBlockMeasurement("first", 50),
                new PrintableBlockMeasurement("second", 1)
            ]);
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.Multiple(() =>
        {
            Assert.That(result.Errors, Is.Empty);
            Assert.That(result.Pages, Has.Count.EqualTo(2));
            Assert.That(result.Pages.All(page => page.BodyHeightMm == 50), Is.True);
            Assert.That(result.Pages[0].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "first" }));
            Assert.That(result.Pages[1].BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "second" }));
        });
    }

    [Test]
    public void Paginate_ReturnsBlockingErrorWhenPlacementBlockCannotFitBodyArea()
    {
        var document = CreateDocument(
            [
                Block("before"),
                Block("too-tall"),
                Block("after")
            ]);
        var measurements = Measurements(("before", 10), ("too-tall", 81), ("after", 10));
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.Multiple(() =>
        {
            Assert.That(result.Pages, Is.Empty);
            Assert.That(result.Errors, Has.One.Matches<PaginationIssue>(issue =>
                issue.Code == PaginationIssueCode.BlockOverflow &&
                issue.BlockId == "too-tall"));
        });
    }

    [Test]
    public void Paginate_ReturnsBlockingErrorWhenPrintableItemTypeIsUnsupported()
    {
        var document = CreateDocument([new UnsupportedPrintableItem("unsupported")]);
        var measurements = Measurements();
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.Multiple(() =>
        {
            Assert.That(result.Pages, Is.Empty);
            Assert.That(result.Errors, Has.One.Matches<PaginationIssue>(issue =>
                issue.Code == PaginationIssueCode.UnsupportedPrintableItem &&
                issue.BlockId == "unsupported"));
        });
    }

    [Test]
    public void Paginate_ReturnsNoPagesWhenMeasurementsAreMissingOrDuplicated()
    {
        var document = CreateDocument(
            [
                Block("before"),
                Block("missing"),
                Block("after")
            ]);
        var measurements = new PrintableDocumentMeasurements(
            HeaderHeightMm: 10,
            FooterHeightMm: 10,
            BodyBlocks:
            [
                new PrintableBlockMeasurement("before", 10),
                new PrintableBlockMeasurement("before", 10),
                new PrintableBlockMeasurement("after", 10)
            ]);
        var paginator = new PrintableDocumentPaginator();

        var result = paginator.Paginate(document, measurements);

        Assert.Multiple(() =>
        {
            Assert.That(result.Pages, Is.Empty);
            Assert.That(result.Errors.Select(issue => issue.Code), Does.Contain(PaginationIssueCode.DuplicateMeasurement));
            Assert.That(result.Errors.Select(issue => issue.Code), Does.Contain(PaginationIssueCode.MissingMeasurement));
        });
    }

    private static PrintableDocument CreateDocument(
        IReadOnlyList<PrintableItem> bodyItems,
        string id = "document-1",
        string changeToken = "change-1")
        => new(
            Id: id,
            ChangeToken: changeToken,
            PageFormat: new PageFormat("Test", WidthMm: 100, HeightMm: 100),
            HeaderItems: [],
            BodyItems: bodyItems,
            FooterItems: []);

    private static PrintableDocumentMeasurements Measurements(params (string BlockId, decimal HeightMm)[] bodyBlocks)
        => new(
            HeaderHeightMm: 10,
            FooterHeightMm: 10,
            BodyBlocks: bodyBlocks
                .Select(block => new PrintableBlockMeasurement(block.BlockId, block.HeightMm))
                .ToArray());

    private static PrintableBlock Block(string id, PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
        => new(id, typeof(TestPrintableComponent), new Dictionary<string, object?>(), pageBreakBehavior);

    private sealed class TestPrintableComponent;

    private sealed record UnsupportedPrintableItem(string ItemId)
        : PrintableItem(ItemId, PageBreakBehavior.None);
}
