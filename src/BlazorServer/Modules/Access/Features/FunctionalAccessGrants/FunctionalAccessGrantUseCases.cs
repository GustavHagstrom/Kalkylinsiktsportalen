using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed class FunctionalAccessGrantUseCases(IAccessStore accessStore)
{
    public Task<UserAccess> GetUserAccessAsync(string userId, string organizationId)
    {
        return accessStore.GetUserAccessAsync(userId, organizationId);
    }

    public Task<RoleAccess> GetRoleAccessAsync(string roleId, string organizationId)
    {
        return accessStore.GetRoleAccessAsync(roleId, organizationId);
    }

    public Task<IReadOnlyList<AccessClaim>> GetAccessClaimsForUserAsync(
        string userId,
        IEnumerable<string> roleIds,
        string organizationId)
    {
        return GetAccessClaimsCoreAsync(userId, roleIds, organizationId);
    }

    public Task GrantUserAccessAsync(string userId, string organizationId, string accessClaimValue)
    {
        return accessStore.AddUserAccessAsync(userId, organizationId, accessClaimValue);
    }

    public Task GrantRoleAccessAsync(string roleId, string organizationId, string accessClaimValue)
    {
        return accessStore.AddRoleAccessAsync(roleId, organizationId, accessClaimValue);
    }

    public Task RevokeUserAccessAsync(string userId, string organizationId, string accessClaimValue)
    {
        return accessStore.DeleteUserAccessAsync(userId, organizationId, accessClaimValue);
    }

    public Task RevokeRoleAccessAsync(string roleId, string organizationId, string accessClaimValue)
    {
        return accessStore.DeleteRoleAccessAsync(roleId, organizationId, accessClaimValue);
    }

    private async Task<IReadOnlyList<AccessClaim>> GetAccessClaimsCoreAsync(
        string userId,
        IEnumerable<string> roleIds,
        string organizationId)
    {
        return await accessStore.GetAccessClaimsForUserAsync(userId, roleIds, organizationId);
    }
}
