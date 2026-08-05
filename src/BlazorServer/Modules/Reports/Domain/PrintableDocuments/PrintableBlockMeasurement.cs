namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PrintableBlockMeasurement
{
    public PrintableBlockMeasurement(string BlockId, decimal HeightMm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(BlockId);
        if (HeightMm <= 0)
        {
            throw new ArgumentException("Block height must be greater than zero.", nameof(HeightMm));
        }

        this.BlockId = BlockId;
        this.HeightMm = HeightMm;
    }

    public string BlockId { get; }
    public decimal HeightMm { get; }
}
