using Core.Models.FromShared;
using Microsoft.Graph.Models;

namespace SharedModule.Services.MicrosoftGraph;
public interface IMicrosoftGraphUserAccess
{
    
    Task<User?> GetUserAsync(string userId);
    Task<UserWithPhoto?> GetUserWithPhotoAsync(string userId);
    Task<User?> GetMeAsync();
    Task<UserWithPhoto?> GetMeWithPhotoAsync();
    Task<IEnumerable<User>> GetUsersAsync();
    Task<IEnumerable<UserWithPhoto>> GetUsersWithPhotoAsync();
    Task<string?> GetUserPhotoAsync(string userId);
}
