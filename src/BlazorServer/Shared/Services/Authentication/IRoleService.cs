using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Services.Authentication;

public interface IRoleService
{
    Task<Role?> GetRoleAsync(string roleId);
    Task<List<Role>> GetRolesAsync(string orgId);
    Task<List<Role>> GetRolesForUserAsync(string userId, string orgId);
    Task<List<User>> GetUsersInRoleAsync(string roleId, string orgId);
    Task<bool> IsAdminAsync(string userId, string orgId);
}
