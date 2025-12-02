using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Shared.Stores.MongoDb.Enteties;

public class MongoUserSettings
{
    [BsonId]
    public string UserId { get; set; } = string.Empty;
    public bool IsDarkMode { get; set; } = false;
}
