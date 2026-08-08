using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorServer.Modules.Reports.Components.Rendering;

internal sealed class PrintableDocumentMeasurementAdapter(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private const string ModulePath = $"./Modules/Reports/Components/Rendering/{nameof(PrintableDocumentRenderer)}.razor.js";
    private const decimal PixelsPerMillimeter = 96m / 25.4m;

    private IJSObjectReference? module;

    public async ValueTask<PrintableDocumentMeasurements> MeasureAsync(
        ElementReference rendererRoot,
        CancellationToken cancellationToken = default)
    {
        var measurementModule = await GetModuleAsync(cancellationToken);
        var result = await measurementModule.InvokeAsync<PrintableDocumentMeasurementResult>(
            "measure",
            cancellationToken,
            rendererRoot);

        return new PrintableDocumentMeasurements(
            PixelsToMillimeters(result.HeaderHeightPx),
            PixelsToMillimeters(result.FooterHeightPx),
            result.BodyBlocks
                .Select(block => new PrintableBlockMeasurement(
                    block.Id,
                    PixelsToMillimeters(block.HeightPx),
                    ParsePageBreakBehavior(block.PageBreakBehavior)))
                .ToArray());
    }

    public async ValueTask<bool> PrintAsync(
        ElementReference rendererRoot,
        ElementReference rendererWrapper,
        CancellationToken cancellationToken = default)
    {
        var measurementModule = await GetModuleAsync(cancellationToken);
        return await measurementModule.InvokeAsync<bool>(
            "print",
            cancellationToken,
            rendererRoot,
            rendererWrapper);
    }

    public async ValueTask DisposeAsync()
    {
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync(CancellationToken cancellationToken)
    {
        module ??= await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            cancellationToken,
            ModulePath);

        return module;
    }

    private static decimal PixelsToMillimeters(decimal pixels)
        => Math.Round(pixels / PixelsPerMillimeter, 4, MidpointRounding.AwayFromZero);

    private static PageBreakBehavior ParsePageBreakBehavior(string value)
        => Enum.TryParse<PageBreakBehavior>(value, ignoreCase: false, out var behavior)
            ? behavior
            : PageBreakBehavior.None;
}

internal sealed record PrintableDocumentMeasurementResult(
    decimal HeaderHeightPx,
    decimal FooterHeightPx,
    IReadOnlyList<PrintableBlockMeasurementResult> BodyBlocks);

internal sealed record PrintableBlockMeasurementResult(
    string Id,
    decimal HeightPx,
    string PageBreakBehavior);
