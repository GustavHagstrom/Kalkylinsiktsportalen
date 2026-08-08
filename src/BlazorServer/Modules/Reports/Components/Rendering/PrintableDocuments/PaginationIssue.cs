namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PaginationIssue(
    PaginationIssueCode Code,
    string Message,
    string? BlockId = null
);
