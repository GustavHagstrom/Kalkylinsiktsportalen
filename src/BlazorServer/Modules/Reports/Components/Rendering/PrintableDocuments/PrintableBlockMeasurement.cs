namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

using BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintableBlockMeasurement
{
    public PrintableBlockMeasurement(
        string BlockId,
        decimal HeightMm,
        PageBreakBehavior PageBreakBehavior = PageBreakBehavior.None)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(BlockId);
        if (HeightMm <= 0)
        {
            throw new ArgumentException("Block height must be greater than zero.", nameof(HeightMm));
        }

        this.BlockId = BlockId;
        this.HeightMm = HeightMm;
        this.PageBreakBehavior = PageBreakBehavior;
    }

    public string BlockId { get; }
    public decimal HeightMm { get; }
    public PageBreakBehavior PageBreakBehavior { get; }
}
