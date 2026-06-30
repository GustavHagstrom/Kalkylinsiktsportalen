using BlazorServer.Modules.Bidcon.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public sealed class MongoEstimationSnapshot
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    public string OrganizationId { get; set; } = string.Empty;

    public string BidconEstimationId { get; set; } = string.Empty;

    public int SchemaVersion { get; set; }

    public DateTime SourceEditDate { get; set; }

    public DateTimeOffset ImportedAt { get; set; }

    public EstimationSnapshotPayloadV1? Payload { get; set; }
}
