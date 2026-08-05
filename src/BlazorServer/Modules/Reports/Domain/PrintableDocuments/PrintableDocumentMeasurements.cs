namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PrintableDocumentMeasurements
{
    public PrintableDocumentMeasurements(
        decimal HeaderHeightMm,
        decimal FooterHeightMm,
        IReadOnlyList<PrintableBlockMeasurement> BodyBlocks)
    {
        if (HeaderHeightMm < 0)
        {
            throw new ArgumentException("Header height cannot be negative.", nameof(HeaderHeightMm));
        }

        if (FooterHeightMm < 0)
        {
            throw new ArgumentException("Footer height cannot be negative.", nameof(FooterHeightMm));
        }

        ArgumentNullException.ThrowIfNull(BodyBlocks);

        this.HeaderHeightMm = HeaderHeightMm;
        this.FooterHeightMm = FooterHeightMm;
        this.BodyBlocks = Array.AsReadOnly(BodyBlocks.ToArray());
    }

    public decimal HeaderHeightMm { get; }
    public decimal FooterHeightMm { get; }
    public IReadOnlyList<PrintableBlockMeasurement> BodyBlocks { get; }
}
