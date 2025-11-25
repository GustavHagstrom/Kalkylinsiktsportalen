using Core.Models.Authorization;

namespace UserRightSettingsModule.Data.Abstractions;
internal interface IRoleStore
{
    Task<Role?> GetRoleAsync(string roleId);
    Task<List<Role>> GetRolesAsync(string tenantId);
    Task UpsertRoleAsync(string tenantId, Role role);
    Task DeleteRoleAsync(Role role);
    Task<List<Role>> GetUserRolesForUserAsync(string userId);
    Task<List<UserRole>> GetUserRolesForRoleAsync(string roleId);
    Task CreateUserRoleAsync(string userId, Role role);
    Task DeleteUserRoleAsync(string userId, Role role);
    Task CreateUserRoleAsync(UserRole userRole);
    Task DeleteUserRoleAsync(UserRole userRole);
}
