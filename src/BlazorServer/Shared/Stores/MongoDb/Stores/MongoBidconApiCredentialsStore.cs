using BlazorServer.Shared.Models.Bidcon;
using BlazorServer.Shared.Stores.MongoDb.Enteties;
using BlazorServer.Shared.Stores.MongoDb.Mappers;
using MongoDB.Driver;

namespace BlazorServer.Shared.Stores.MongoDb.Stores;

public class MongoBidconApiCredentialsStore : IBidconApiCredentialsStore
{
    private readonly IMongoCollection<MongoBidconApiCredentials> _collection;

    public MongoBidconApiCredentialsStore(IMongoCollection<MongoBidconApiCredentials> collection)
    {
        _collection = collection;
    }
    public async Task<BidconApiCredentials> GetAsync(string orgId)
    {
        var credentials = await _collection.Find(c => c.OrganizationId == orgId).FirstOrDefaultAsync();
        return MongoBidconApiCredentialsMapper.ToModel(credentials);
    }

    public Task UpsertAsync(BidconApiCredentials credentials)
    {
        var mongoCredentials = MongoBidconApiCredentialsMapper.ToMongo(credentials);
        return _collection.ReplaceOneAsync(c => c.OrganizationId == credentials.OrganizationId, mongoCredentials, new ReplaceOptions { IsUpsert = true });
    }
}
