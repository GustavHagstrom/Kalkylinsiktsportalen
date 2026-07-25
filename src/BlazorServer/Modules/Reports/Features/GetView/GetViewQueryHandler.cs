using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Reports.Views;

namespace BlazorServer.Modules.Reports.Features.GetView;

internal sealed class GetViewQueryHandler(IViewStore viewStore)
    : IQueryHandler<GetViewQuery, GetViewResult>
{
    public async Task<GetViewResult> HandleAsync(
        GetViewQuery query,
        CancellationToken cancellationToken)
    {
        var view = await viewStore.GetAsync(query.ViewId);

        return new ViewLoaded(view);
    }
}
