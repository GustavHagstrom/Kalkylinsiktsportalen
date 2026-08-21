using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintableContainer : PrintableItem
{
    public PrintableContainer(
        string Id,
        IReadOnlyList<PrintableItem> Items,
        PageBreakBehavior PageBreakBehavior = PageBreakBehavior.None)
        : base(Id, PageBreakBehavior)
    {
        ArgumentNullException.ThrowIfNull(Items);

        this.Items = Array.AsReadOnly(Items.ToArray());
    }

    public IReadOnlyList<PrintableItem> Items { get; }
}
