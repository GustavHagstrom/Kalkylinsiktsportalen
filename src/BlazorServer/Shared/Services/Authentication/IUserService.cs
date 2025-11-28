using BlazorServer.Shared.Models;

namespace BlazorServer.Shared.Services.Authentication;

public interface IUserService
{
    Task<User?> GetUserAsync(string userId);
    Task<List<User>> GetUsersAsync(string orgId);

}
