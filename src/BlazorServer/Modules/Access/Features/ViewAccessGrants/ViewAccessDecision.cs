using System.Security.Claims;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Extensions;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

internal static class ViewAccessDecision
{
    public static async Task<bool> HasViewAccessAsync(
        IViewAccessGrantStore store,
        ClaimsPrincipal user,
        string viewId,
        ViewAccessRight right)
    {
        if (user.IsInRole(AuthConstants.AdminRoleClaimValue))
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

    private static bool IsGrantForUserOrRole(
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
