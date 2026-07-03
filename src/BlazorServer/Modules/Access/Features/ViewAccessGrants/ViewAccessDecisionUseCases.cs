using System.Security.Claims;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Extensions;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed class ViewAccessDecisionUseCases(ViewAccessGrantUseCases viewAccessGrants)
{
    public Task<bool> CanUseViewAsync(ClaimsPrincipal user, string viewId)
    {
        return HasViewAccessAsync(user, viewId, ViewAccessRight.UseView);
    }

    public Task<bool> CanEditViewAsync(ClaimsPrincipal user, string viewId)
    {
        return HasViewAccessAsync(user, viewId, ViewAccessRight.EditView);
    }

    public Task<bool> CanAdministerViewAccessAsync(ClaimsPrincipal user, string viewId)
    {
        return HasViewAccessAsync(user, viewId, ViewAccessRight.AdministerViewAccess);
    }

    private Task<bool> HasViewAccessAsync(
        ClaimsPrincipal user,
        string viewId,
        ViewAccessRight right)
    {
        if (user.IsInRole(AuthConstants.AdminRoleClaimValue))
        {
            return Task.FromResult(true);
        }

        var userId = user.GetUserId();
        var organizationId = user.GetOrganizationId();
        if (userId is null || organizationId is null)
        {
            return Task.FromResult(false);
        }

        var roleIds = user.FindAll("groups").Select(claim => claim.Value).ToArray();
        return viewAccessGrants.HasViewAccessAsync(viewId, userId, roleIds, organizationId, right);
    }
}
