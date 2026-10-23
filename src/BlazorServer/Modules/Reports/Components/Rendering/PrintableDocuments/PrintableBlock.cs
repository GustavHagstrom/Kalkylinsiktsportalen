using BlazorServer.Modules.Reports.Domain.Views;
using System.Collections.ObjectModel;

namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintableBlock : PrintableItem
{
    public PrintableBlock(
        string Id,
        Type ComponentType,
        IReadOnlyDictionary<string, object?>? Parameters = null,
        PageBreakBehavior PageBreakBehavior = PageBreakBehavior.None)
        : base(Id, PageBreakBehavior)
    {
        this.ComponentType = ComponentType ?? throw new ArgumentNullException(nameof(ComponentType));
        this.Parameters = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(Parameters ?? new Dictionary<string, object?>(), StringComparer.Ordinal));
    }

    public Type ComponentType { get; }
    public IReadOnlyDictionary<string, object?> Parameters { get; }
}
