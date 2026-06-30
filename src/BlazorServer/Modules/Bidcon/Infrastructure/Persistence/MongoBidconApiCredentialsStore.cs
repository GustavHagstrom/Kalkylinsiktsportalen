using MongoDB.Driver;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public sealed class MongoBidconApiCredentialsStore : IBidconApiCredentialsStore
{
    private readonly IMongoCollection<MongoBidconApiCredentials> collection;

    public MongoBidconApiCredentialsStore(IMongoCollection<MongoBidconApiCredentials> collection)
    {
        this.collection = collection;
    }

    public async Task<BidconApiCredentials?> GetAsync(string orgId)
    {
        var credentials = await collection
            .Find(credentials => credentials.OrganizationId == orgId)
            .FirstOrDefaultAsync();

        return credentials.ToModel();
    }

    public Task UpsertAsync(BidconApiCredentials credentials)
    {
        var mongoCredentials = credentials.ToMongo();
        return collection.ReplaceOneAsync(
            existing => existing.OrganizationId == credentials.OrganizationId,
            mongoCredentials,
            new ReplaceOptions { IsUpsert = true });
    }
}
