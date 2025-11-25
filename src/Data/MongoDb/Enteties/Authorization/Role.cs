using Core.Models.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Enteties.Authorization;
public class Role : ITenantEntety
{
    [BsonId]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> AccessClaimValues { get; set; } = [];
}
