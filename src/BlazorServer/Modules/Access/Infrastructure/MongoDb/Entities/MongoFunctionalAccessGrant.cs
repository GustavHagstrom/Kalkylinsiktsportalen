using BlazorServer.Modules.Access.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;

public sealed class MongoFunctionalAccessGrant
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonRepresentation(BsonType.String)]
    public FunctionalAccessGrantTargetType TargetType { get; set; }

    public string TargetId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public List<string> AccessClaimValues { get; set; } = [];
}
