namespace BlazorServer.Modules.Reports.Domain.Views;

public sealed record PageFormat
{
    public static PageFormat A3 { get; } = new("A3", WidthMm: 297, HeightMm: 420);
    public static PageFormat A4 { get; } = new("A4", WidthMm: 210, HeightMm: 297);
    public static PageFormat A5 { get; } = new("A5", WidthMm: 148, HeightMm: 210);

    private PageFormat(string Name, decimal WidthMm, decimal HeightMm)
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

    public static PageFormat FromPersistedName(string? name)
    {
        if (string.Equals(name, A3.Name, StringComparison.Ordinal))
            return A3;

        if (string.Equals(name, A5.Name, StringComparison.Ordinal))
            return A5;

        return A4;
    }
}
