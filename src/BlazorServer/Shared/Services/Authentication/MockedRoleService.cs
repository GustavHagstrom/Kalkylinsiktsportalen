using BlazorServer.Shared.Constants;
using BlazorServer.Shared.MockedData;
using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedRoleService : IRoleService
{
    public Task<bool> IsAdminAsync(string userId, string orgId)
    {
        if (userId == "Admin")
        {
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<Role?> GetRoleAsync(string roleId)
    {
        return Task.FromResult(MockedDataStore.Roles.FirstOrDefault(r => r.Id == roleId));
    }

    public Task<List<Role>> GetRolesAsync(string orgId)
    {
        return Task.FromResult(MockedDataStore.Roles.ToList());
    }

    public Task<List<Role>> GetRolesForUserAsync(string userId, string orgId)
    {
        var roles = MockedDataStore.Roles.Where(r => r.Id == userId).ToList();
        return Task.FromResult(roles);
    }

    public Task<List<User>> GetUsersInRoleAsync(string roleId, string orgId)
    {
        var users = MockedDataStore.Users.Where(u => u.Id == roleId).ToList();
        return Task.FromResult(users);
    }
}
