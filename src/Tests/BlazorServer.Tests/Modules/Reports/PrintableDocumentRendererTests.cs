using BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using ReportPaginatedDocumentRenderer = BlazorServer.Modules.Reports.Views.Rendering.PaginatedDocumentRenderer;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class PrintableDocumentRendererTests
{
    [Test]
    public async Task PrintableDocument_RendersModelDrivenHeaderBodyAndFooterBlocks()
    {
        var document = new PrintableDocument(
            Id: "demo-document",
            ChangeToken: "change-1",
            PageFormat: PageFormat.A4,
            HeaderItems:
            [
                Block("header", "header-text")
            ],
            BodyItems:
            [
                new PrintableContainer(
                    "body-container",
                    [
                        Block("body", "body-text")
                    ],
                    PageBreakBehavior.After)
            ],
            FooterItems:
            [
                Block("footer", "footer-text")
            ]);
        var html = await RenderDocumentAsync(document);

        Assert.Multiple(() =>
        {
            Assert.That(html, Does.Contain("header-text"));
            Assert.That(html, Does.Contain("body-text"));
            Assert.That(html, Does.Contain("footer-text"));
            Assert.That(html, Does.Not.Contain("body-container"));
            Assert.That(html, Does.Contain("data-printable-region=\"body\""));
            Assert.That(html, Does.Contain("data-printable-block-id=\"body\""));
            Assert.That(html, Does.Contain("data-page-break-behavior=\"After\""));
            Assert.That(html, Does.Not.Contain("Pageination/engine.js"));
            Assert.That(html, Does.Not.Contain("Pageination/styles.css"));
        });
    }

    [Test]
    public void PrintableDocument_FailsFastWhenBodyBlockIdsAreDuplicated()
    {
        var document = new PrintableDocument(
            Id: "demo-document",
            ChangeToken: "change-1",
            PageFormat: PageFormat.A4,
            HeaderItems: [],
            BodyItems:
            [
                Block("duplicate", "first"),
                Block("duplicate", "second")
            ],
            FooterItems: []);

        var exception = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await RenderDocumentAsync(document));

        Assert.That(exception!.Message, Does.Contain("duplicate"));
    }

    [Test]
    public void PrintableDocument_PrintCssHidesApplicationChromeAndKeepsReadyPagesPrintable()
    {
        var component = File.ReadAllText(FindProjectFile(
            "BlazorServer",
            "Modules",
            "Reports",
            "Views",
            "Rendering",
            "PaginatedDocumentRenderer.razor"));

        Assert.Multiple(() =>
        {
            Assert.That(component, Does.Contain("@@media print"));
            Assert.That(component, Does.Contain("body *"));
            Assert.That(component, Does.Contain("visibility: hidden"));
            Assert.That(component, Does.Contain("printable-document-wrapper[data-printable-print-target=\"true\"]"));
            Assert.That(component, Does.Contain("visibility: visible !important"));
            Assert.That(component, Does.Contain("printable-document-root[data-printable-renderer-state=\"Ready\"] .page"));
            Assert.That(component, Does.Contain("box-shadow: none !important"));
        });
    }

    [Test]
    public void PaginatedDocumentRenderer_ExposesSafePrintCommandForCallers()
    {
        Assert.That(
            typeof(ReportPaginatedDocumentRenderer).GetMethod(nameof(ReportPaginatedDocumentRenderer.PrintAsync)),
            Is.Not.Null);
    }

    private static PrintableBlock Block(
        string id,
        string text,
        PageBreakBehavior pageBreakBehavior = PageBreakBehavior.None)
        => new(
            id,
            typeof(TestPrintableContent),
            new Dictionary<string, object?> { [nameof(TestPrintableContent.Text)] = text },
            pageBreakBehavior);

    private static async Task<string> RenderDocumentAsync(PrintableDocument document)
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

    private sealed class TestPrintableContent : ComponentBase
    {
        [Parameter] public string Text { get; set; } = "";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddContent(1, Text);
            builder.CloseElement();
        }
    }

    private sealed class TestJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => throw new NotSupportedException("The printable document renderer test does not invoke JavaScript.");

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => throw new NotSupportedException("The printable document renderer test does not invoke JavaScript.");
    }

    private static string FindProjectFile(params string[] segments)
    {
        for (var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(new[] { directory.FullName }.Concat(segments).ToArray());
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException($"Could not find project file '{Path.Combine(segments)}'.");
    }
}
