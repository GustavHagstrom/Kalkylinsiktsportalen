using BlazorServer.Shared.MockedData;
using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedRoleService : IRoleService
{
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
        throw new NotImplementedException();
    }

    public Task<List<User>> GetUsersInRoleAsync(string roleId, string orgId)
    {
        throw new NotImplementedException();
    }
}
