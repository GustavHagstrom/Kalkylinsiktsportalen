namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PaginationIssue(
    PaginationIssueCode Code,
    string Message,
    string? BlockId = null
);
