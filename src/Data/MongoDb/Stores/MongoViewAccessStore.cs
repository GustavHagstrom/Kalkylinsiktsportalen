using Core.Models.EstimationView;
using Data.Abstractions;
using Data.MongoDb.Mappers;
using MongoDB.Driver;
using MongoEntities = Data.MongoDb.Enteties;

namespace Data.MongoDb.Stores;
public class MongoViewAccessStore
    (
    IMongoCollection<MongoEntities.EstimationView.View> viewCollection,
    IMongoCollection<MongoEntities.EstimationView.UserView> userViewCollection,
    IMongoCollection<MongoEntities.EstimationView.RoleView> roleViewCollection,
    IMongoCollection<MongoEntities.Authorization.UserRole> roleCollection
    ) : IViewAccessStore
{
    public async Task<List<View>> GetAvailableViewsAsync(string userId)
    {
        var userRoles = await roleCollection.Find(ur => ur.UserId == userId).ToListAsync();
        var userViews = await userViewCollection.Find(uv => uv.UserId == userId).ToListAsync();
        var roleViews = await roleViewCollection.Find(rv => userRoles.Select(ur => ur.RoleId).Contains(rv.RoleId)).ToListAsync();
        var viewIds = userViews.Select(uv => uv.ViewId).Concat(roleViews.Select(rv => rv.ViewId)).Distinct();
        var views = await viewCollection.Find(v => viewIds.Contains(v.Id)).ToListAsync();
        return views.Select(ViewMapper.ToModel).ToList();
    }
}
