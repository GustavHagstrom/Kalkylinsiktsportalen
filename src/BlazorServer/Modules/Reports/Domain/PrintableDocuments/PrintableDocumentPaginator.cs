using BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;

namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed class PrintableDocumentPaginator
{
    public PrintablePaginationResult Paginate(PrintableDocument document, PrintableDocumentMeasurements measurements)
    {
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
        var flattenedBlocks = Flatten(document.BodyItems);
        var pages = PlaceBlocks(flattenedBlocks, measurementByBlockId, availableBodyHeightMm, errors);

        return new PrintablePaginationResult(pages, warnings, errors);
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

    private static IReadOnlyList<FlattenedPrintableBlock> Flatten(IReadOnlyList<PrintableItem> items)
    {
        var blocks = new List<FlattenedPrintableBlock>();
        foreach (var item in items)
        {
            Flatten(item, blocks);
        }

        return blocks;
    }

    private static void Flatten(PrintableItem item, List<FlattenedPrintableBlock> blocks)
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
                    Flatten(child, blocks);
                }

                if (container.PageBreakBehavior == PageBreakBehavior.After && blocks.Count > firstDescendantIndex)
                {
                    var lastIndex = blocks.Count - 1;
                    blocks[lastIndex] = blocks[lastIndex] with { PageBreakBehavior = PageBreakBehavior.After };
                }

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
                errors.Add(new PaginationIssue(
                    PaginationIssueCode.MissingMeasurement,
                    $"Block '{block.BlockId}' is missing a measurement.",
                    block.BlockId));
                continue;
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
