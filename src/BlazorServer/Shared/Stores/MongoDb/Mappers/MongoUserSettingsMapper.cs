using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores.MongoDb.Enteties;

namespace BlazorServer.Shared.Stores.MongoDb.Mappers;

public static class MongoUserSettingsMapper
{
    public static MongoUserSettings ToMongo(UserSettings model)
    {
        return new MongoUserSettings
        {
            UserId = model.UserId,
            IsDarkMode = model.IsDarkMode
        };
    }
    public static UserSettings ToModel(MongoUserSettings mongo)
    {
        return new UserSettings
        {
            UserId = mongo.UserId,
            IsDarkMode = mongo.IsDarkMode
        };
    }
}
