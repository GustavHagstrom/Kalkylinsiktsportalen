namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintableDocumentPrintResult(
    PrintableDocumentPrintStatus Status,
    IReadOnlyList<PrintableDocumentPrintIssue> Errors)
{
    public static PrintableDocumentPrintResult Ready()
        => new(PrintableDocumentPrintStatus.Ready, []);

    public static PrintableDocumentPrintResult NotReady()
        => new(PrintableDocumentPrintStatus.NotReady, []);

    public static PrintableDocumentPrintResult BlockedByPaginationErrors(IReadOnlyList<PrintableDocumentPrintIssue> errors)
        => new(PrintableDocumentPrintStatus.BlockedByPaginationErrors, errors);

    public static PrintableDocumentPrintResult BlockedByContentErrors(IReadOnlyList<PrintableDocumentPrintIssue> errors)
        => new(PrintableDocumentPrintStatus.BlockedByContentErrors, errors);
}
