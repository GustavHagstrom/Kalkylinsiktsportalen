using Core.Models.EstimationView;
using Microsoft.EntityFrameworkCore;
using Data.EfCore;
using ViewSettingsModule.Data.Abstractions;

namespace ViewSettingsModule.Data.EfCore;
internal class SqlUserViewStore(IDbContextFactory<ApplicationDbContext> contextFactory) : IUserViewStore
{
    public async Task<IEnumerable<string>> GetPickedUserIdsAsync(View view)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        return await context.UserViews
            .Where(x => x.ViewId == view.Id)
            .Select(x => x.UserId)
            .ToListAsync();
    }
    public async Task PickAsync(View view, IEnumerable<string> userIds)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        var userViews = userIds.Select(x => new UserView
        {
            UserId = x,
            ViewId = view.Id
        });
        await context.UserViews.AddRangeAsync(userViews);
        await context.SaveChangesAsync();
    }
    public async Task UnpickAsync(View view, IEnumerable<string> userIds)
    {
        using var context = await contextFactory.CreateDbContextAsync();
        var userViews = await context.UserViews
            .Where(x => x.ViewId == view.Id && userIds.Contains(x.UserId))
            .ToListAsync();
        context.UserViews.RemoveRange(userViews);
        await context.SaveChangesAsync();
    }
}
