using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.AppShell.Settings.Infrastructure.MongoDb.Entities;

public class MongoUserSettings
{
    [BsonId]
    public string UserId { get; set; } = string.Empty;
    public bool IsDarkMode { get; set; } = false;
}
