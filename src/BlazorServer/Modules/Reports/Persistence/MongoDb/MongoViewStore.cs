using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;
using BlazorServer.Identity;

namespace BlazorServer.Modules.Reports.Persistence.MongoDb;

public class MongoViewStore : IViewStore
{
    private readonly IMongoCollection<MongoView> _collection;

    public MongoViewStore(IMongoCollection<MongoView> collection)
    {
        _collection = collection;
    }
    public async Task<View?> GetAsync(ViewId viewId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, viewId.Value);
        var mongoView = await _collection.Find(filter).FirstOrDefaultAsync();
        return ViewMappings.ToDomain(mongoView);
    }
    public async Task<List<View>> GetAllAsync(OrganizationId orgId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.OrganizationId, orgId.Value);
        var mongoViews = await _collection.Find(filter).ToListAsync();
        var views = mongoViews.Select(ViewMappings.ToDomain).OfType<View>().ToList();
        return views;
    }
    public Task DeleteAsync(ViewId viewId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, viewId.Value);
        return _collection.DeleteOneAsync(filter);
    }
    public Task UpsertAsync(View view)
    {
        var mongoView = ViewMappings.ToMongo(view);
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, mongoView.Id);
        return _collection.ReplaceOneAsync(filter, mongoView, new ReplaceOptions { IsUpsert = true });
    }
}
