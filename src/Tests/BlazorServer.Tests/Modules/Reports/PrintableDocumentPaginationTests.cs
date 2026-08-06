using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

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
    public void PrintableDocumentModel_RequiresIdentityChangeTokenAndUsesPresetPageFormats()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => CreateDocument([], id: ""));
            Assert.Throws<ArgumentException>(() => CreateDocument([], changeToken: ""));
            Assert.That(PageFormat.A3.Name, Is.EqualTo("A3"));
            Assert.That(PageFormat.A3.WidthMm, Is.EqualTo(297m));
            Assert.That(PageFormat.A3.HeightMm, Is.EqualTo(420m));
            Assert.That(PageFormat.A4.Name, Is.EqualTo("A4"));
            Assert.That(PageFormat.A4.WidthMm, Is.EqualTo(210m));
            Assert.That(PageFormat.A4.HeightMm, Is.EqualTo(297m));
            Assert.That(PageFormat.A5.Name, Is.EqualTo("A5"));
            Assert.That(PageFormat.A5.WidthMm, Is.EqualTo(148m));
            Assert.That(PageFormat.A5.HeightMm, Is.EqualTo(210m));
            Assert.That(
                typeof(PageFormat).GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance),
                Is.Empty);
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
                new PrintableBlockMeasurement("first", 100),
                new PrintableBlockMeasurement("second", 80),
                new PrintableBlockMeasurement("third", 20)
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
            HeaderHeightMm: 80,
            FooterHeightMm: 80,
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
        var measurements = Measurements(("before", 10), ("too-tall", 191), ("after", 10));
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
            PageFormat: PageFormat.A5,
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
