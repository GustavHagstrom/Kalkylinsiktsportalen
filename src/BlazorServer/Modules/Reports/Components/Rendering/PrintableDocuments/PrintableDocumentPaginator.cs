using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed class PrintableDocumentPaginator
{
    public PrintablePaginationResult Paginate(PrintableDocument document, PrintableDocumentMeasurements measurements)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(measurements);

        var availableBodyHeightMm = document.PageFormat.HeightMm - measurements.HeaderHeightMm - measurements.FooterHeightMm;
        var errors = new List<PaginationIssue>();
        var warnings = new List<PaginationIssue>();

        if (availableBodyHeightMm <= 0)
        {
            errors.Add(new PaginationIssue(
                PaginationIssueCode.InvalidBodyArea,
                "Header and footer measurements leave no body area for printable blocks."));
            return new PrintablePaginationResult([], warnings, errors);
        }

        var measurementByBlockId = BuildMeasurementLookup(measurements.BodyBlocks, errors);
        var flattenedBlocks = Flatten(document.BodyItems, errors);
        AppendMissingMeasurementErrors(flattenedBlocks, measurementByBlockId, errors);
        if (errors.Count > 0)
        {
            return new PrintablePaginationResult([], warnings, errors);
        }

        var pages = PlaceBlocks(flattenedBlocks, measurementByBlockId, availableBodyHeightMm, errors);

        return errors.Count > 0
            ? new PrintablePaginationResult([], warnings, errors)
            : new PrintablePaginationResult(pages, warnings, errors);
    }

    private static Dictionary<string, PrintableBlockMeasurement> BuildMeasurementLookup(
        IReadOnlyList<PrintableBlockMeasurement> measurements,
        List<PaginationIssue> errors)
    {
        var measurementByBlockId = new Dictionary<string, PrintableBlockMeasurement>(StringComparer.Ordinal);
        foreach (var measurement in measurements)
        {
            if (!measurementByBlockId.TryAdd(measurement.BlockId, measurement))
            {
                errors.Add(new PaginationIssue(
                    PaginationIssueCode.DuplicateMeasurement,
                    $"Block '{measurement.BlockId}' has more than one measurement.",
                    measurement.BlockId));
            }
        }

        return measurementByBlockId;
    }

    private static void AppendMissingMeasurementErrors(
        IReadOnlyList<FlattenedPrintableBlock> blocks,
        Dictionary<string, PrintableBlockMeasurement> measurementByBlockId,
        List<PaginationIssue> errors)
    {
        foreach (var block in blocks)
        {
            if (!measurementByBlockId.ContainsKey(block.BlockId))
            {
                errors.Add(new PaginationIssue(
                    PaginationIssueCode.MissingMeasurement,
                    $"Block '{block.BlockId}' is missing a measurement.",
                    block.BlockId));
            }
        }
    }

    private static IReadOnlyList<FlattenedPrintableBlock> Flatten(IReadOnlyList<PrintableItem> items, List<PaginationIssue> errors)
    {
        var blocks = new List<FlattenedPrintableBlock>();
        foreach (var item in items)
        {
            Flatten(item, blocks, errors);
        }

        return blocks;
    }

    private static void Flatten(PrintableItem item, List<FlattenedPrintableBlock> blocks, List<PaginationIssue> errors)
    {
        switch (item)
        {
            case PrintableBlock block:
                blocks.Add(new FlattenedPrintableBlock(block.Id, block.PageBreakBehavior));
                break;

            case PrintableContainer container:
                var firstDescendantIndex = blocks.Count;
                foreach (var child in container.Items)
                {
                    Flatten(child, blocks, errors);
                }

                if (container.PageBreakBehavior == PageBreakBehavior.After && blocks.Count > firstDescendantIndex)
                {
                    var lastIndex = blocks.Count - 1;
                    blocks[lastIndex] = blocks[lastIndex] with { PageBreakBehavior = PageBreakBehavior.After };
                }

                break;

            default:
                errors.Add(new PaginationIssue(
                    PaginationIssueCode.UnsupportedPrintableItem,
                    $"Printable item '{item.Id}' has unsupported type '{item.GetType().Name}'.",
                    item.Id));
                break;
        }
    }

    private static IReadOnlyList<PrintablePage> PlaceBlocks(
        IReadOnlyList<FlattenedPrintableBlock> blocks,
        Dictionary<string, PrintableBlockMeasurement> measurementByBlockId,
        decimal availableBodyHeightMm,
        List<PaginationIssue> errors)
    {
        var pages = new List<PrintablePage>();
        var currentBlocks = new List<PrintableBlockPlacement>();
        var currentTopMm = 0m;

        foreach (var block in blocks)
        {
            if (!measurementByBlockId.TryGetValue(block.BlockId, out var measurement))
            {
                return [];
            }

            if (measurement.HeightMm > availableBodyHeightMm)
            {
                errors.Add(new PaginationIssue(
                    PaginationIssueCode.BlockOverflow,
                    $"Block '{block.BlockId}' is taller than the available body area.",
                    block.BlockId));
                return [];
            }

            if (currentBlocks.Count > 0 && currentTopMm + measurement.HeightMm > availableBodyHeightMm)
            {
                AddPage(pages, availableBodyHeightMm, currentBlocks);
                currentBlocks = [];
                currentTopMm = 0m;
            }

            currentBlocks.Add(new PrintableBlockPlacement(
                block.BlockId,
                currentTopMm,
                measurement.HeightMm,
                block.PageBreakBehavior));
            currentTopMm += measurement.HeightMm;

            if (block.PageBreakBehavior == PageBreakBehavior.After)
            {
                AddPage(pages, availableBodyHeightMm, currentBlocks);
                currentBlocks = [];
                currentTopMm = 0m;
            }
        }

        if (currentBlocks.Count > 0)
        {
            AddPage(pages, availableBodyHeightMm, currentBlocks);
        }

        return pages;
    }

    private static void AddPage(
        List<PrintablePage> pages,
        decimal availableBodyHeightMm,
        IReadOnlyList<PrintableBlockPlacement> bodyBlocks)
        => pages.Add(new PrintablePage(pages.Count + 1, availableBodyHeightMm, bodyBlocks.ToArray()));

    private sealed record FlattenedPrintableBlock(string BlockId, PageBreakBehavior PageBreakBehavior);
}
