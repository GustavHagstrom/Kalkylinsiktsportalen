using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Stores;

public interface IAccessStore
{
    Task<UserAccess> GetUserAccessAsync(string userId, string orgId);
    Task<RoleAccess> GetRoleAccessAsync(string roleId, string orgId);
    Task<List<AccessClaim>> GetAccessClaimsForUserAsync(string userId, IEnumerable<string> roleIds, string orgId);
    Task AddUserAccessAsync(string userId, string orgId, string accessClaimValue);
    Task AddRoleAccessAsync(string roleId, string orgId, string accessClaimValue);
    Task DeleteUserAccessAsync(string userId, string orgId, string accessClaimValue);
    Task DeleteRoleAccessAsync(string roleId, string orgId, string accessClaimValue);
}
