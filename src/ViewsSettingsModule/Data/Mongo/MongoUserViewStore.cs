using Core.Models.EstimationView;
using ViewSettingsModule.Data.Abstractions;
using MongoDB.Driver;
using MongoEnteties = Data.MongoDb.Enteties;

namespace ViewSettingsModule.Data.Mongo;
internal class MongoUserViewStore(IMongoCollection<MongoEnteties.EstimationView.UserView> userViewStore) : IUserViewStore
{
    public async Task<IEnumerable<string>> GetPickedUserIdsAsync(View view)
    {
        var userViews = await userViewStore.Find(x => x.ViewId == view.Id).ToListAsync();
        return userViews.Select(x => x.UserId);
    }

    public Task PickAsync(View view, IEnumerable<string> userIds)
    {
        var userViews = userIds.Select(x => new MongoEnteties.EstimationView.UserView
        {
            UserId = x,
            ViewId = view.Id
        });
        return userViewStore.InsertManyAsync(userViews);
    }

    public Task UnpickAsync(View view, IEnumerable<string> userIds)
    {
        return userViewStore.DeleteManyAsync(x => x.ViewId == view.Id && userIds.Contains(x.UserId));
    }
}
