using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public static class MongoEstimationSnapshotMapper
{
    public static MongoEstimationSnapshot ToMongo(this EstimationSnapshot snapshot)
    {
        return new MongoEstimationSnapshot
        {
            Id = snapshot.Id.Value.ToString(),
            OrganizationId = snapshot.OrganizationId.Value.ToString(),
            BidconEstimationId = snapshot.BidconEstimationId.Value,
            SchemaVersion = (int)snapshot.SchemaVersion,
            SourceEditDate = snapshot.SourceEditDate,
            ImportedAt = snapshot.ImportedAt,
            Payload = snapshot.Payload
        };
    }

    public static EstimationSnapshot? ToDomain(this MongoEstimationSnapshot? snapshot)
    {
        if (snapshot?.Payload is null)
        {
            return null;
        }

        return EstimationSnapshot.Create(
            new EstimationSnapshotId(Guid.Parse(snapshot.Id)),
            new OrganizationId(snapshot.OrganizationId),
            new BidconEstimationId(snapshot.BidconEstimationId),
            snapshot.SourceEditDate,
            snapshot.ImportedAt,
            snapshot.Payload);
    }
}
