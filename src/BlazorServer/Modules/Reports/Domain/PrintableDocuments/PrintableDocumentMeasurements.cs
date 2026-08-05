namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PrintableDocumentMeasurements(
    decimal HeaderHeightMm,
    decimal FooterHeightMm,
    IReadOnlyList<PrintableBlockMeasurement> BodyBlocks
);
