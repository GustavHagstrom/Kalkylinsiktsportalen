using BlazorServer.Modules.Bidcon.Contracts;
using BlazorServer.Modules.Bidcon.Domain;
using MongoDB.Driver;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public sealed class MongoEstimationSnapshotStore : IEstimationSnapshotStore
{
    private readonly IMongoCollection<MongoEstimationSnapshot> collection;

    public MongoEstimationSnapshotStore(IMongoCollection<MongoEstimationSnapshot> collection)
    {
        this.collection = collection;
    }

    public Task AddAsync(EstimationSnapshot snapshot, CancellationToken cancellationToken)
    {
        return collection.InsertOneAsync(snapshot.ToMongo(), cancellationToken: cancellationToken);
    }

    public async Task<EstimationSnapshot?> GetAsync(
        EstimationSnapshotId snapshotId,
        CancellationToken cancellationToken)
    {
        var filter = Builders<MongoEstimationSnapshot>.Filter.Eq(snapshot => snapshot.Id, snapshotId.Value.ToString());
        var snapshot = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return snapshot.ToDomain();
    }
}
