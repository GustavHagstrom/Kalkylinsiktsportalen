using Core.Data;
using Core.Extensions;
using Data.Abstractions;
using Data.MongoDb.Enteties;
using MongoDB.Driver;
using System.Security.Claims;

namespace Data.MongoDb.Stores;
public class MongoDarkModeStore : MongoStore<UserSettings>, IDarkModeStore
{
    public MongoDarkModeStore(IMongoCollection<UserSettings> collection) : base(collection)
    {
    }

    public async Task<bool> GetAsync(ClaimsPrincipal user)
    {
        var id = user?.GetUserId() ?? throw new ArgumentNullException();
        var settings = await GetByIdAsync(id);
        return settings?.IsDarkMode ?? false;

    }

    public Task SetAsync(ClaimsPrincipal user, bool isDarkMode)
    {
        var id = user?.GetUserId() ?? throw new ArgumentNullException();
        return UpsertAsync(new UserSettings { UserId = id, IsDarkMode = isDarkMode }, id);
    }
}
