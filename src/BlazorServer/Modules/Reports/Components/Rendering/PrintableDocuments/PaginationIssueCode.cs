namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public enum PaginationIssueCode
{
    MissingMeasurement,
    DuplicateMeasurement,
    InvalidBodyArea,
    BlockOverflow,
    MeasurementFailed,
    UnsupportedPrintableItem
}
