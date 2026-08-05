namespace BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;

public abstract record PrintableItem
{
    protected PrintableItem(string Id, PageBreakBehavior PageBreakBehavior)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Id);

        this.Id = Id;
        this.PageBreakBehavior = PageBreakBehavior;
    }

    public string Id { get; }
    public PageBreakBehavior PageBreakBehavior { get; }
}
