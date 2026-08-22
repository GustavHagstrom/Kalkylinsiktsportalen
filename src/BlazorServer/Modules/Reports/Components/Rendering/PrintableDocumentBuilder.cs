using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
using BlazorServer.Modules.Reports.Components.Views;
using BlazorServer.Modules.Reports.Rendering;

namespace BlazorServer.Modules.Reports.Components.Rendering;

public sealed class PrintableDocumentBuilder
{
    public PrintableDocument Build(View view, ReportRenderingContext renderingContext)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(renderingContext);

        return new PrintableDocument(
            Id: $"report-view-{view.Id.Value}",
            ChangeToken: CreateChangeToken(view, renderingContext),
            PageFormat: view.PageFormat,
            HeaderItems: [],
            BodyItems: BuildBodyItems(view),
            FooterItems: [],
            PrintBlockingIssues: BuildPrintBlockingIssues(view));
    }

    private static IReadOnlyList<PrintableItem> BuildBodyItems(View view)
    {
        var items = new List<PrintableItem>();
        foreach (var section in OrderedSections(view))
        {
            switch (section)
            {
                case DataSection dataSection:
                    items.Add(BuildDataSectionItem(dataSection));
                    break;

                case SheetSection sheetSection:
                    items.Add(BuildSheetSectionItem(sheetSection));
                    break;
            }
        }

        return items;
    }

    private static PrintableContainer BuildDataSectionItem(DataSection section)
        => new(
            $"data-section-{section.SectionId}",
            section.Rows
                .Select((row, rowIndex) => new PrintableBlock(
                    $"data-section-{section.SectionId}-row-{rowIndex}",
                    typeof(DataSectionRowComponent),
                    new Dictionary<string, object?>
                    {
                        [nameof(DataSectionRowComponent.DataSection)] = section,
                        [nameof(DataSectionRowComponent.RowDefinition)] = row,
                        [nameof(DataSectionRowComponent.RowIndex)] = rowIndex
                    }))
                .ToArray());

    private static IReadOnlyList<PrintableDocumentPrintIssue> BuildPrintBlockingIssues(View view)
        => OrderedSections(view)
            .OfType<DataSection>()
            .SelectMany(BuildDataSectionPrintBlockingIssues)
            .ToArray();

    private static IEnumerable<PrintableDocumentPrintIssue> BuildDataSectionPrintBlockingIssues(DataSection section)
    {
        foreach (var rowIndex in Enumerable.Range(0, section.Rows.Count))
        {
            foreach (var columnIndex in Enumerable.Range(0, section.Columns.Count))
            {
                var cell = section.GetCell(rowIndex, columnIndex);
                if (cell.IsDefault() || DataCellFormulaEvaluator.GetDisplayValue(cell) != DataCellFormulaEvaluator.ErrorDisplayValue)
                    continue;

                yield return new PrintableDocumentPrintIssue(
                    "CellEvaluationError",
                    $"Data section '{section.SectionId}' cell ({rowIndex}, {columnIndex}) contains an invalid expression.",
                    $"data-section-{section.SectionId}-row-{rowIndex}");
            }
        }
    }

    private static PrintableContainer BuildSheetSectionItem(SheetSection section)
        => new(
            $"sheet-section-{section.SectionId}",
            Enum.GetValues<RowType>()
                .Select(rowType => new PrintableBlock(
                    $"sheet-section-{section.SectionId}-row-{rowType}",
                    typeof(SheetSectionRowComponent),
                    new Dictionary<string, object?>
                    {
                        [nameof(SheetSectionRowComponent.SheetSection)] = section,
                        [nameof(SheetSectionRowComponent.RowType)] = rowType
                    }))
                .ToArray());

    private static IEnumerable<IViewSection> OrderedSections(View view)
        => view.Sections;

    private static string CreateChangeToken(View view, ReportRenderingContext renderingContext)
    {
        var tokenInput = JsonSerializer.Serialize(new
        {
            RenderingContext = new
            {
                renderingContext.EstimationVersionReference,
                renderingContext.EstimationSnapshotReference,
                renderingContext.ViewScopedChangeSetReference,
                renderingContext.PrintableContentChangeToken,
                Values = renderingContext.Values.OrderBy(value => value.Key)
            },
            View = new
            {
                Id = view.Id.Value,
                view.Name,
                view.Description,
                PageFormat = new
                {
                    view.PageFormat.Name,
                    view.PageFormat.WidthMm,
                    view.PageFormat.HeightMm
                },
                Sections = view.Sections.Select(section => new
                {
                    section.SectionId
                })
            }
        });

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(tokenInput));
        return Convert.ToHexString(hash);
    }
}
