using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;
using BlazorServer.Modules.Reports.Domain.Views;
using System.Text.Json;

namespace BlazorServer.Modules.Reports.Components.Rendering;

public sealed record ReportRenderingContext
{
    public ReportRenderingContext(
        string EstimationVersionReference,
        string EstimationSnapshotReference,
        string PrintableContentChangeToken,
        string? ViewScopedChangeSetReference = null,
        PageFormat? PageFormat = null,
        IReadOnlyDictionary<string, string>? Values = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(EstimationVersionReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(EstimationSnapshotReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(PrintableContentChangeToken);

        this.EstimationVersionReference = EstimationVersionReference;
        this.EstimationSnapshotReference = EstimationSnapshotReference;
        this.ViewScopedChangeSetReference = ViewScopedChangeSetReference;
        this.PrintableContentChangeToken = PrintableContentChangeToken;
        this.PageFormat = PageFormat ?? PageFormat.A4;
        this.Values = new Dictionary<string, string>(
            Values ?? new Dictionary<string, string>(),
            StringComparer.Ordinal);
    }

    public string EstimationVersionReference { get; }
    public string EstimationSnapshotReference { get; }
    public string? ViewScopedChangeSetReference { get; }
    public string PrintableContentChangeToken { get; }
    public PageFormat PageFormat { get; }
    public IReadOnlyDictionary<string, string> Values { get; }

    public static ReportRenderingContext ForViewPreview(View view)
    {
        ArgumentNullException.ThrowIfNull(view);
        var viewReference = view.Id.Value;
        return new ReportRenderingContext(
            EstimationVersionReference: $"view-preview:{viewReference}:estimation-version",
            EstimationSnapshotReference: $"view-preview:{viewReference}:estimation-snapshot",
            PrintableContentChangeToken: JsonSerializer.Serialize(new
            {
                ViewId = view.Id.Value,
                view.Name,
                view.Description,
                Sections = view.Sections.Select(section => new
                {
                    section.SectionId,
                    section.SectionType,
                    section.Section
                })
            }));
    }

    public static ReportRenderingContext ForViewPreview(string viewReference, string printableContentChangeToken)
        => new(
            EstimationVersionReference: $"view-preview:{viewReference}:estimation-version",
            EstimationSnapshotReference: $"view-preview:{viewReference}:estimation-snapshot",
            PrintableContentChangeToken: printableContentChangeToken);
}
