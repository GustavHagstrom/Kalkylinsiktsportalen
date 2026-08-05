namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PrintableBlockMeasurement(
    string BlockId,
    decimal HeightMm
);
