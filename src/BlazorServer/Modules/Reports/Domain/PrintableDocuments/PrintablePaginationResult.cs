namespace BlazorServer.Modules.Reports.Domain.PrintableDocuments;

public sealed record PrintablePaginationResult(
    IReadOnlyList<PrintablePage> Pages,
    IReadOnlyList<PaginationIssue> Warnings,
    IReadOnlyList<PaginationIssue> Errors
);
