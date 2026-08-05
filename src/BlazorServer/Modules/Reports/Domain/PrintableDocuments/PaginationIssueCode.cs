namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public enum PaginationIssueCode
{
    MissingMeasurement,
    DuplicateMeasurement,
    InvalidBodyArea,
    BlockOverflow,
    UnsupportedPrintableItem
}
