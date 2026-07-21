using BlazorServer.Identity;

namespace BlazorServer.Modules.Access.Public.Contracts;

public interface IAccessDirectory
{
    Task<AccessDirectoryUser?> GetUserAsync(UserId userId);
    Task<List<AccessDirectoryUser>> GetUsersAsync(OrganizationId organizationId);
    Task<AccessDirectoryRole?> GetRoleAsync(string roleId);
    Task<List<AccessDirectoryRole>> GetRolesAsync(OrganizationId organizationId);
    Task<List<AccessDirectoryRole>> GetRolesForUserAsync(UserId userId, OrganizationId organizationId);
    Task<List<AccessDirectoryUser>> GetUsersInRoleAsync(string roleId, OrganizationId organizationId);
}
