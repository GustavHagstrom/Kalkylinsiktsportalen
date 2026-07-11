using BlazorServer.Modules.Access.Public.Contracts;

namespace BlazorServer.Modules.Access.Infrastructure.Identity;

public class MockAccessDirectory : IAccessDirectory
{
    private readonly string organizationId;
    private readonly List<AccessDirectoryUser> users;
    private readonly List<AccessDirectoryRole> roles;
    private readonly Dictionary<string, List<string>> roleIdsByUserId;

    public MockAccessDirectory(string organizationId)
    {
        this.organizationId = organizationId;
        users =
        [
            new("Admin", "Admin", null),
            new("User", "User", null),
            new("Guest", "Guest", null)
        ];
        roles =
        [
            new("Admin", "AdminRole", "Administrator role with full permissions."),
            new("User", "UserRole", "Standard user role with limited permissions."),
            new("Guest", "GuestRole", "Guest role with minimal permissions.")
        ];
        roleIdsByUserId = new(StringComparer.Ordinal)
        {
            ["Admin"] = ["Admin"],
            ["User"] = ["User"],
            ["Guest"] = ["Guest"]
        };
    }

    public Task<AccessDirectoryUser?> GetUserAsync(string userId)
        => Task.FromResult(users.FirstOrDefault(user => user.Id == userId));

    public Task<List<AccessDirectoryUser>> GetUsersAsync(string organizationId)
        => Task.FromResult(IsConfiguredOrganization(organizationId) ? users.ToList() : []);

    public Task<AccessDirectoryRole?> GetRoleAsync(string roleId)
        => Task.FromResult(roles.FirstOrDefault(role => role.Id == roleId));

    public Task<List<AccessDirectoryRole>> GetRolesAsync(string organizationId)
        => Task.FromResult(IsConfiguredOrganization(organizationId) ? roles.ToList() : []);

    public Task<List<AccessDirectoryRole>> GetRolesForUserAsync(string userId, string organizationId)
    {
        if (!IsConfiguredOrganization(organizationId))
        {
            return Task.FromResult(new List<AccessDirectoryRole>());
        }

        if (!roleIdsByUserId.TryGetValue(userId, out var roleIds))
        {
            return Task.FromResult(new List<AccessDirectoryRole>());
        }

        return Task.FromResult(roles.Where(role => roleIds.Contains(role.Id)).ToList());
    }

    public Task<List<AccessDirectoryUser>> GetUsersInRoleAsync(string roleId, string organizationId)
    {
        if (!IsConfiguredOrganization(organizationId))
        {
            return Task.FromResult(new List<AccessDirectoryUser>());
        }

        var userIds = roleIdsByUserId
            .Where(pair => pair.Value.Contains(roleId))
            .Select(pair => pair.Key)
            .ToHashSet(StringComparer.Ordinal);

        return Task.FromResult(users.Where(user => userIds.Contains(user.Id)).ToList());
    }

    private bool IsConfiguredOrganization(string candidate)
    {
        return string.Equals(candidate, organizationId, StringComparison.Ordinal);
    }
}
