using BlazorServer.Modules.Reports.Domain.PrintableDocuments;
using PrintableDocumentContract = BlazorServer.Modules.Reports.Domain.PrintableDocuments.PrintableDocument;

namespace BlazorServer.Modules.Reports.Components.Rendering;

internal sealed class PrintableDocumentRendererState
{
    private PaginationKey? currentKey;
    private PaginationKey? paginatedKey;

    public PrintableRendererStatus Status { get; private set; } = PrintableRendererStatus.Measuring;
    public bool NeedsMeasurement => currentKey is not null && currentKey != paginatedKey && Status != PrintableRendererStatus.Paginating;
    public IReadOnlyList<PaginationIssue> Warnings { get; private set; } = [];
    public IReadOnlyList<PaginationIssue> Errors { get; private set; } = [];
    public IReadOnlyList<PrintablePage> Pages { get; private set; } = [];

    public PrintableDocumentPrintResult GetPrintReadiness()
    {
        if (Status == PrintableRendererStatus.Ready && !NeedsMeasurement && Errors.Count == 0)
        {
            return PrintableDocumentPrintResult.Ready();
        }

        return Errors.Count > 0
            ? PrintableDocumentPrintResult.BlockedByPaginationErrors(Errors.Select(ToPrintIssue).ToArray())
            : PrintableDocumentPrintResult.NotReady();
    }

    public void SetDocument(PrintableDocumentContract document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var nextKey = PaginationKey.From(document);
        if (nextKey == currentKey)
        {
            return;
        }

        currentKey = nextKey;
        Status = PrintableRendererStatus.Measuring;
        Warnings = [];
        Errors = [];
        Pages = [];
    }

    public void MarkPaginating()
    {
        Status = PrintableRendererStatus.Paginating;
    }

    public void RequestRefresh()
    {
        paginatedKey = null;
        Status = PrintableRendererStatus.Measuring;
        Warnings = [];
        Errors = [];
        Pages = [];
    }

    public void MarkReady(PrintablePaginationResult pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        Warnings = pagination.Warnings;
        Errors = pagination.Errors;
        Pages = pagination.Pages;
        paginatedKey = currentKey;
        Status = Errors.Count > 0
            ? PrintableRendererStatus.Error
            : PrintableRendererStatus.Ready;
    }

    public void MarkError(PaginationIssue error)
    {
        ArgumentNullException.ThrowIfNull(error);

        Warnings = [];
        Errors = [error];
        Pages = [];
        paginatedKey = currentKey;
        Status = PrintableRendererStatus.Error;
    }

    private sealed record PaginationKey(
        string DocumentId,
        string ChangeToken,
        string PageFormatName,
        decimal PageFormatWidthMm,
        decimal PageFormatHeightMm)
    {
        public static PaginationKey From(PrintableDocumentContract document)
            => new(
                document.Id,
                document.ChangeToken,
                document.PageFormat.Name,
                document.PageFormat.WidthMm,
                document.PageFormat.HeightMm);
    }

    private static PrintableDocumentPrintIssue ToPrintIssue(PaginationIssue issue)
        => new(issue.Code.ToString(), issue.Message, issue.BlockId);
}

internal enum PrintableRendererStatus
{
    Measuring,
    Paginating,
    Error,
    Ready
}
