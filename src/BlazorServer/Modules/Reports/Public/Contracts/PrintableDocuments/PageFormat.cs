namespace BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;

public sealed record PageFormat
{
    public PageFormat(string Name, decimal WidthMm, decimal HeightMm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        if (WidthMm <= 0)
        {
            throw new ArgumentException("Page format width must be greater than zero.", nameof(WidthMm));
        }

        if (HeightMm <= 0)
        {
            throw new ArgumentException("Page format height must be greater than zero.", nameof(HeightMm));
        }

        this.Name = Name;
        this.WidthMm = WidthMm;
        this.HeightMm = HeightMm;
    }

    public string Name { get; }
    public decimal WidthMm { get; }
    public decimal HeightMm { get; }
}
