using Core.Models.EstimationView;
using Data.MongoDb.Mappers;
using ViewSettingsModule.Data.Abstractions;
using MongoDB.Driver;
using MongoEnteties = Data.MongoDb.Enteties.EstimationView;
namespace ViewSettingsModule.Data.Mongo;
internal class MongoViewStore(IMongoCollection<MongoEnteties.View> viewCollection) : IViewStore
{
    public Task DeleteAsync(string tenantId, View view)
    {
        return viewCollection.DeleteOneAsync(x => x.Id == view.Id);
    }

    public async Task<View?> GetAsync(string tenantId, string viewId)
    {
        var view = await viewCollection.Find(x => x.Id == viewId && x.TenantId == tenantId).FirstOrDefaultAsync();
        if (view is null)
        {
            return null;
        }
        return ViewMapper.ToModel(view);
    }

    public Task<List<View>> GetViewListAsync(string tenantId)
    {
        return viewCollection.Find(x => x.TenantId == tenantId).ToListAsync().ContinueWith(x => x.Result.Select(ViewMapper.ToModel).ToList());
    }

    public Task UpsertAsync(string tenantId, View view)
    {
        view.TenantId = tenantId;
        return viewCollection.ReplaceOneAsync(x => x.Id == view.Id, ViewMapper.ToMongo(view), new ReplaceOptions { IsUpsert = true });
    }
}
