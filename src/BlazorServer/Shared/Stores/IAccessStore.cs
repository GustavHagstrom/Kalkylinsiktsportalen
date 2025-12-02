using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Stores;

public interface IAccessStore
{
    Task<UserAccess> GetUserAccess(string userId, string orgId);
    Task<RoleAccess> GetRoleAccess(string roleId, string orgId);
    Task<List<AccessClaim>> GetAccessClaimsForUser(string userId, IEnumerable<string> roleIds, string orgId);
    Task AddUserAccess(string userId, string orgId, string accessClaimValue);
    Task AddRoleAccess(string roleId, string orgId, string accessClaimValue);
    Task DeleteUserAccess(string userId, string orgId, string accessClaimValue);
    Task DeleteRoleAccess(string roleId, string orgId, string accessClaimValue);
}
