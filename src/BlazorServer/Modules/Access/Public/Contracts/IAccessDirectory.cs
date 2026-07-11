namespace BlazorServer.Modules.Access.Public.Contracts;

public interface IAccessDirectory
{
    Task<AccessDirectoryUser?> GetUserAsync(string userId);
    Task<List<AccessDirectoryUser>> GetUsersAsync(string organizationId);
    Task<AccessDirectoryRole?> GetRoleAsync(string roleId);
    Task<List<AccessDirectoryRole>> GetRolesAsync(string organizationId);
    Task<List<AccessDirectoryRole>> GetRolesForUserAsync(string userId, string organizationId);
    Task<List<AccessDirectoryUser>> GetUsersInRoleAsync(string roleId, string organizationId);
}
