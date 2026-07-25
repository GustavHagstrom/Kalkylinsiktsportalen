using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Reports.Views;

namespace BlazorServer.Modules.Reports.Features.ListViews;

internal sealed class ListViewsQueryHandler(IViewStore viewStore)
    : IQueryHandler<ListViewsQuery, ListViewsResult>
{
    public async Task<ListViewsResult> HandleAsync(
        ListViewsQuery query,
        CancellationToken cancellationToken)
    {
        var views = await viewStore.GetAllAsync(query.OrganizationId);

        return new ViewsListed(views);
    }
}
