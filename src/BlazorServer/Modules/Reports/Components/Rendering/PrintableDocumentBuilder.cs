using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;
using BlazorServer.Modules.Reports.Components.Views;

namespace BlazorServer.Modules.Reports.Components.Rendering;

public sealed class PrintableDocumentBuilder
{
    public PrintableDocument Build(View view, ReportRenderingContext renderingContext)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(renderingContext);
        view.EnsureSectionIdentities();

        return new PrintableDocument(
            Id: $"report-view-{view.Id.Value}",
            ChangeToken: CreateChangeToken(view, renderingContext),
            PageFormat: renderingContext.PageFormat,
            HeaderItems: [],
            BodyItems: BuildBodyItems(view),
            FooterItems: []);
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
            section.RowDefinitions
                .OrderBy(row => row.RowIndex)
                .Select(row => new PrintableBlock(
                    $"data-section-{section.SectionId}-row-{row.RowIndex}",
                    typeof(DataSectionRowComponent),
                    new Dictionary<string, object?>
                    {
                        [nameof(DataSectionRowComponent.DataSection)] = section,
                        [nameof(DataSectionRowComponent.RowDefinition)] = row
                    }))
                .ToArray());

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

    private static IEnumerable<object> OrderedSections(View view)
        => view.DataSections.Cast<object>()
            .Concat(view.SheetSections)
            .OrderBy(SectionIndex);

    private static int SectionIndex(object section)
        => section switch
        {
            DataSection dataSection => dataSection.SectionIndex,
            SheetSection sheetSection => sheetSection.SectionIndex,
            _ => throw new NotSupportedException($"Unsupported report section type '{section.GetType().Name}'.")
        };

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
                PageFormat = new
                {
                    renderingContext.PageFormat.Name,
                    renderingContext.PageFormat.WidthMm,
                    renderingContext.PageFormat.HeightMm
                },
                Values = renderingContext.Values.OrderBy(value => value.Key)
            },
            View = new
            {
                Id = view.Id.Value,
                view.Name,
                view.Description,
                DataSections = view.DataSections.OrderBy(section => section.SectionIndex),
                SheetSections = view.SheetSections.OrderBy(section => section.SectionIndex)
            }
        });

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(tokenInput));
        return Convert.ToHexString(hash);
    }
}
