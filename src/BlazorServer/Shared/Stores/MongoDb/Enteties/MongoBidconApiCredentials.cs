using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Shared.Stores.MongoDb.Enteties;

public class MongoBidconApiCredentials
{
    [BsonId]
    public string OrganizationId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}
