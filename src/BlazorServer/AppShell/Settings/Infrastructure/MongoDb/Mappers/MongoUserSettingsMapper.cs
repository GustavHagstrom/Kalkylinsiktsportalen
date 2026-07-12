using BlazorServer.AppShell.Settings.Infrastructure.MongoDb.Entities;

namespace BlazorServer.AppShell.Settings.Infrastructure.MongoDb.Mappers;

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
