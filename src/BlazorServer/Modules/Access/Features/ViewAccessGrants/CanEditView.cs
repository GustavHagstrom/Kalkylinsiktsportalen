using System.Security.Claims;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record CanEditViewQuery(ClaimsPrincipal User, string ViewId);

public sealed class CanEditViewHandler(IViewAccessGrantStore store)
{
    public Task<bool> HandleAsync(CanEditViewQuery query)
    {
        return ViewAccessDecision.HasViewAccessAsync(
            store,
            query.User,
            query.ViewId,
            ViewAccessRight.EditView);
    }
}
