namespace BlazorServer.Modules.Reports.Components.Rendering.PrintableDocuments;

public sealed record PrintablePaginationResult(
    IReadOnlyList<PrintablePage> Pages,
    IReadOnlyList<PaginationIssue> Warnings,
    IReadOnlyList<PaginationIssue> Errors
);
