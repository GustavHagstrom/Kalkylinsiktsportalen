namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintableDocumentPrintIssue(
    string Code,
    string Message,
    string? BlockId = null);
