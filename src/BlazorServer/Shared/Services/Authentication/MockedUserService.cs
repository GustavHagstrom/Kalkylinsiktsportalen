using BlazorServer.Shared.MockedData;
using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Services.Authentication;

public class MockedUserService : IUserService
{
    public Task<User?> GetUserAsync(string userId)
    {
        return Task.FromResult(MockedDataStore.Users.FirstOrDefault(u => u.Id == userId));
    }

    public Task<List<User>> GetUsersAsync(string orgId)
    {
        return Task.FromResult(MockedDataStore.Users.ToList());
    }
}
