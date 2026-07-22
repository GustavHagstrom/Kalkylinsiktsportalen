using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public sealed class MongoBidconCredentialsAuditEntry
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string OrganizationId { get; set; } = string.Empty;

    public string ActorId { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }

    public string Action { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}
