using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;

public class MongoUserAccess
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public string UserId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public List<string> AccessClaimValues { get; set; } = []; 
}
