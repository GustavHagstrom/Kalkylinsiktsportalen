using System.Collections.ObjectModel;

namespace BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;

public sealed record PrintableBlock : PrintableItem
{
    public PrintableBlock(
        string Id,
        Type ComponentType,
        IReadOnlyDictionary<string, object?> Parameters,
        PageBreakBehavior PageBreakBehavior = PageBreakBehavior.None)
        : base(Id, PageBreakBehavior)
    {
        this.ComponentType = ComponentType ?? throw new ArgumentNullException(nameof(ComponentType));
        this.Parameters = new ReadOnlyDictionary<string, object?>(
            new Dictionary<string, object?>(Parameters ?? throw new ArgumentNullException(nameof(Parameters)), StringComparer.Ordinal));
    }

    public Type ComponentType { get; }
    public IReadOnlyDictionary<string, object?> Parameters { get; }
}
