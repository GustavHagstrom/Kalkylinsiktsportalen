namespace BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;

public sealed record PrintableDocument
{
    public PrintableDocument(
        string Id,
        string ChangeToken,
        PageFormat PageFormat,
        IReadOnlyList<PrintableItem> HeaderItems,
        IReadOnlyList<PrintableItem> BodyItems,
        IReadOnlyList<PrintableItem> FooterItems)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(ChangeToken);

        this.Id = Id;
        this.ChangeToken = ChangeToken;
        this.PageFormat = PageFormat ?? throw new ArgumentNullException(nameof(PageFormat));
        this.HeaderItems = CopyItems(HeaderItems);
        this.BodyItems = CopyItems(BodyItems);
        this.FooterItems = CopyItems(FooterItems);
    }

    public string Id { get; }
    public string ChangeToken { get; }
    public PageFormat PageFormat { get; }
    public IReadOnlyList<PrintableItem> HeaderItems { get; }
    public IReadOnlyList<PrintableItem> BodyItems { get; }
    public IReadOnlyList<PrintableItem> FooterItems { get; }

    private static IReadOnlyList<PrintableItem> CopyItems(IReadOnlyList<PrintableItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        return Array.AsReadOnly(items.ToArray());
    }
}
