using BlazorServer.Modules.Reports.Components.Rendering;
using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;
using BlazorServer.Modules.Reports.Domain.Views;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorServer.Tests.Modules.Reports;

[TestFixture]
public sealed class PrintableDocumentMeasurementAdapterTests
{
    [Test]
    public async Task MeasureAsync_ImportsCollocatedModuleAndReturnsMeasurementsByStableBlockId()
    {
        var jsRuntime = new RecordingJsRuntime(new PrintableDocumentMeasurementResult(
            HeaderHeightPx: 96,
            FooterHeightPx: 48,
            BodyBlocks:
            [
                new PrintableBlockMeasurementResult("intro", 192, "None"),
                new PrintableBlockMeasurementResult("totals", 24, "After")
            ]));
        var adapter = new PrintableDocumentMeasurementAdapter(jsRuntime);

        var measurements = await adapter.MeasureAsync(default);

        Assert.Multiple(() =>
        {
            Assert.That(jsRuntime.ImportedModulePath, Is.EqualTo("./Modules/Reports/Components/Rendering/PrintableDocumentRenderer.razor.js"));
            Assert.That(jsRuntime.Module.InvokedIdentifier, Is.EqualTo("measure"));
            Assert.That(measurements.HeaderHeightMm, Is.EqualTo(25.4m));
            Assert.That(measurements.FooterHeightMm, Is.EqualTo(12.7m));
            Assert.That(measurements.BodyBlocks.Select(block => block.BlockId), Is.EqualTo(new[] { "intro", "totals" }));
            Assert.That(measurements.BodyBlocks.Select(block => block.HeightMm), Is.EqualTo(new[] { 50.8m, 6.35m }));
            Assert.That(measurements.BodyBlocks.Select(block => block.PageBreakBehavior), Is.EqualTo(new[] { PageBreakBehavior.None, PageBreakBehavior.After }));
        });
    }

    [Test]
    public async Task PrintAsync_UsesCollocatedModuleToPrintTheRendererRoot()
    {
        var jsRuntime = new RecordingJsRuntime(new PrintableDocumentMeasurementResult(
            HeaderHeightPx: 0,
            FooterHeightPx: 0,
            BodyBlocks: []));
        var adapter = new PrintableDocumentMeasurementAdapter(jsRuntime);

        var printed = await adapter.PrintAsync(default, default);

        Assert.Multiple(() =>
        {
            Assert.That(jsRuntime.ImportedModulePath, Is.EqualTo("./Modules/Reports/Components/Rendering/PrintableDocumentRenderer.razor.js"));
            Assert.That(jsRuntime.Module.InvokedIdentifier, Is.EqualTo("print"));
            Assert.That(printed, Is.True);
        });
    }

    private sealed class RecordingJsRuntime(PrintableDocumentMeasurementResult measurementResult) : IJSRuntime
    {
        public RecordingJsModule Module { get; } = new(measurementResult);
        public string? ImportedModulePath { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
        {
            Assert.That(identifier, Is.EqualTo("import"));
            ImportedModulePath = AssertSingleStringArgument(args);
            return ValueTask.FromResult((TValue)(object)Module);
        }
    }

    private sealed class RecordingJsModule(PrintableDocumentMeasurementResult measurementResult) : IJSObjectReference
    {
        public string? InvokedIdentifier { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
        {
            InvokedIdentifier = identifier;
            Assert.That(args, Is.Not.Null);
            Assert.That(args![0], Is.TypeOf<ElementReference>());
            if (identifier == "print")
            {
                Assert.That(args, Has.Length.EqualTo(2));
                Assert.That(args![1], Is.TypeOf<ElementReference>());
                return ValueTask.FromResult((TValue)(object)true);
            }

            Assert.That(args, Has.Length.EqualTo(1));
            return ValueTask.FromResult((TValue)(object)measurementResult);
        }

        public ValueTask DisposeAsync()
            => ValueTask.CompletedTask;
    }

    private static string AssertSingleStringArgument(object?[]? args)
    {
        Assert.That(args, Is.Not.Null);
        Assert.That(args, Has.Length.EqualTo(1));
        Assert.That(args![0], Is.TypeOf<string>());
        return (string)args[0]!;
    }
}
