using BlazorServer.Shared.DTOs.Views;
using BlazorServer.Shared.Stores.MongoDb.Enteties;
using BlazorServer.Shared.Stores.MongoDb.Mappers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlazorServer.Shared.Stores.MongoDb.Stores;

public class MongoViewStore : IViewStore
{
    private readonly IMongoCollection<MongoView> _collection;

    public MongoViewStore(IMongoCollection<MongoView> collection)
    {
        _collection = collection;
    }
    public async Task<ViewDto?> GetAsync(string viewId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, viewId);
        var mongoView = await _collection.Find(filter).FirstOrDefaultAsync();
        return MongoViewMapper.ToDto(mongoView);
    }
    public async Task<List<ViewDto>> GetAllAsync(string orgId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.OrganizationId, orgId);
        var mongoViews = await _collection.Find(filter).ToListAsync();
        var viewDtos = mongoViews.Select(MongoViewMapper.ToDto).ToList();
        return viewDtos;
    }
    public Task DeleteAsync(string viewId)
    {
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, viewId);
        return _collection.DeleteOneAsync(filter);
    }
    public Task UpsertAsync(ViewDto view)
    {
        var mongoView = MongoViewMapper.ToMongo(view);
        var filter = Builders<MongoView>.Filter.Eq(v => v.Id, mongoView.Id);
        return _collection.ReplaceOneAsync(filter, mongoView, new ReplaceOptions { IsUpsert = true });
    }
}
