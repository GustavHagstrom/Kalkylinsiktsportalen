using BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using ReportPrintableDocument = BlazorServer.Modules.Reports.Views.Rendering.PrintableDocument;

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
            PageFormat: new PageFormat("A4", WidthMm: 210, HeightMm: 297),
            HeaderItems:
            [
                Block("header", "header-text")
            ],
            BodyItems:
            [
                new PrintableContainer(
                    "body-container",
                    [
                        Block("body", "body-text", PageBreakBehavior.After)
                    ])
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
        });
    }

    [Test]
    public void PrintableDocument_FailsFastWhenBodyBlockIdsAreDuplicated()
    {
        var document = new PrintableDocument(
            Id: "demo-document",
            ChangeToken: "change-1",
            PageFormat: new PageFormat("A4", WidthMm: 210, HeightMm: 297),
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
            var output = await renderer.RenderComponentAsync<ReportPrintableDocument>(
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
}
