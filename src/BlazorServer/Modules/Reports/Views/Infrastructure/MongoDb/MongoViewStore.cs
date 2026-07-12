using BlazorServer.Modules.Reports.Public.Contracts.Views;
using BlazorServer.Modules.Reports.Public;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlazorServer.Modules.Reports.Views.Infrastructure.MongoDb;

public class MongoViewStore : IViewStore
{
    private readonly IMongoCollection<MongoView> _collection;

    public MongoViewStore(IMongoCollection<MongoView> collection)
    {
        _collection = collection;
    }
    public async Task<ViewDto?> GetAsync(ViewId viewId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, viewId.Value);
        var mongoView = await _collection.Find(filter).FirstOrDefaultAsync();
        return ViewMappings.ToDto(mongoView);
    }
    public async Task<List<ViewDto>> GetAllAsync(string orgId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.OrganizationId, orgId);
        var mongoViews = await _collection.Find(filter).ToListAsync();
        var viewDtos = mongoViews.Select(ViewMappings.ToDto).ToList();
        return viewDtos;
    }
    public Task DeleteAsync(ViewId viewId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, viewId.Value);
        return _collection.DeleteOneAsync(filter);
    }
    public Task UpsertAsync(ViewDto view)
    {
        var mongoView = ViewMappings.ToMongo(view);
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, mongoView.Id);
        return _collection.ReplaceOneAsync(filter, mongoView, new ReplaceOptions { IsUpsert = true });
    }
}
