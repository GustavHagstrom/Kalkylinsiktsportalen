using System.Security.Claims;
using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Reports.Public;
using Microsoft.Identity.Web;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed class ViewAccessDecisionHandler(IViewAccessGrantStore store)
{
    public Task<bool> HandleAsync(CanUseViewQuery query)
    {
        return HasViewAccessAsync(
            query.User,
            query.ViewId,
            ViewAccessRight.UseView);
    }

    public Task<bool> HandleAsync(CanEditViewQuery query)
    {
        return HasViewAccessAsync(
            query.User,
            query.ViewId,
            ViewAccessRight.EditView);
    }

    public Task<bool> HandleAsync(CanAdministerViewAccessQuery query)
    {
        return HasViewAccessAsync(
            query.User,
            query.ViewId,
            ViewAccessRight.AdministerViewAccess);
    }

    async Task<bool> HasViewAccessAsync(
        ClaimsPrincipal user,
        ViewId viewId,
        ViewAccessRight right)
    {
        if (user.IsApplicationAdmin())
        {
            return true;
        }

        var userId = user.FindFirstValue(ClaimConstants.ObjectId);
        var organizationId = user.FindFirstValue(ClaimConstants.TenantId);
        if (userId is null || organizationId is null)
        {
            return false;
        }

        var roleIds = user.FindAll("groups")
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.Ordinal);
        var grants = await store.GetGrantsForViewAsync(viewId, organizationId);

        return grants.Any(grant =>
            grant.Right == right &&
            IsGrantForUserOrRole(grant, userId, roleIds));
    }

    static bool IsGrantForUserOrRole(
        ViewAccessGrant grant,
        string userId,
        IReadOnlySet<string> roleIds)
    {
        return grant.TargetType switch
        {
            ViewAccessGrantTargetType.User => grant.TargetId == userId,
            ViewAccessGrantTargetType.Role => roleIds.Contains(grant.TargetId),
            _ => false
        };
    }
}
