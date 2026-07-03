using System.Security.Claims;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record CanUseViewQuery(ClaimsPrincipal User, string ViewId);

public sealed class CanUseViewHandler(IViewAccessGrantStore store)
{
    public Task<bool> HandleAsync(CanUseViewQuery query)
    {
        return ViewAccessDecision.HasViewAccessAsync(
            store,
            query.User,
            query.ViewId,
            ViewAccessRight.UseView);
    }
}
