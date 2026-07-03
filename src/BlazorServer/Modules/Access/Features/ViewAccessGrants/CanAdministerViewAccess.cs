using System.Security.Claims;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record CanAdministerViewAccessQuery(ClaimsPrincipal User, string ViewId);

public sealed class CanAdministerViewAccessHandler(IViewAccessGrantStore store)
{
    public Task<bool> HandleAsync(CanAdministerViewAccessQuery query)
    {
        return ViewAccessDecision.HasViewAccessAsync(
            store,
            query.User,
            query.ViewId,
            ViewAccessRight.AdministerViewAccess);
    }
}
