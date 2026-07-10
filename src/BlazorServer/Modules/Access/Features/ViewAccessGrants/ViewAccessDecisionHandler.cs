using System.Security.Claims;
using BlazorServer.AppShell.Authentication;

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
        string viewId,
        ViewAccessRight right)
    {
        if (user.IsApplicationAdmin())
        {
            return true;
        }

        var userId = user.GetUserId();
        var organizationId = user.GetOrganizationId();
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
