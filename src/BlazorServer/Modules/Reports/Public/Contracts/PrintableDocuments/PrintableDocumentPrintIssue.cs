namespace BlazorServer.Modules.Reports.Public.Contracts.PrintableDocuments;

public sealed record PrintableDocumentPrintIssue(
    string Code,
    string Message,
    string? BlockId = null);
