namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PrintableDocumentPrintIssue(
    string Code,
    string Message,
    string? BlockId = null);
