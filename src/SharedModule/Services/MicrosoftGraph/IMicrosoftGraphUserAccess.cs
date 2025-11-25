using Core.Models.FromShared;
using Microsoft.Graph.Models;

namespace SharedModule.Services.MicrosoftGraph;
public interface IMicrosoftGraphUserAccess
{
    
    Task<User?> GetUserAsync(string id);
    Task<UserWithPhoto?> GetUserWithPhotoAsync(string id);
    Task<User?> GetMeAsync();
    Task<UserWithPhoto?> GetMeWithPhotoAsync();
    Task<IEnumerable<User>> GetUsersAsync();
    Task<IEnumerable<UserWithPhoto>> GetUsersWithPhotoAsync();
    
}
