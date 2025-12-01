using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores.MongoDb.Enteties;
using BlazorServer.Shared.Stores.MongoDb.Mappers;
using MongoDB.Driver;

namespace BlazorServer.Shared.Stores.MongoDb.Stores;

public class MongoUserSettingsStore(IMongoCollection<MongoUserSettings> mongoCollection) : IUserSettingsStore, IDarkmodeStore
{
    public Task<bool> IsDarkmodeAsync(string userId)
    {
        return GetByIdAsync(userId)
            .ContinueWith(t => t.Result?.IsDarkMode ?? false);
    }

    public async Task SetDarkmodeAsync(string userId, bool darkmode)
    {
        var settings = await GetByIdAsync(userId);
        settings.IsDarkMode = darkmode;
        await UpsertAsync(settings);
    }

    public async Task<UserSettings> GetByIdAsync(string userId)
    {
        var filter = Builders<MongoUserSettings>.Filter.Eq(x => x.UserId, userId);
        var result = await mongoCollection.Find(filter).FirstOrDefaultAsync();
        if (result is null)
        {
            var defaultSettings = new UserSettings
            {
                UserId = userId,
                IsDarkMode = false
            };
            await UpsertAsync(defaultSettings);
            return defaultSettings;
        }
        return MongoUserSettingsMapper.ToModel(result);

    }

    public Task UpsertAsync(UserSettings model)
    {
        var mongoEntity = MongoUserSettingsMapper.ToMongo(model);
        var filter = Builders<MongoUserSettings>.Filter.Eq(x => x.UserId, model.UserId);
        return mongoCollection.ReplaceOneAsync(filter, mongoEntity, new ReplaceOptions { IsUpsert = true });
    }
}
