using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Core.Models;
using Core.Extensions;
using Data.Abstractions;

namespace Data.EfCore.Stores;

public class SqlDarkModeStore(IDbContextFactory<ApplicationDbContext> ContextFactory) : IDarkModeStore
{
    public async Task<bool> GetAsync(ClaimsPrincipal user)
    {
        var dbContext = ContextFactory.CreateDbContext();
        var result = await dbContext.UserSettings.FindAsync(user.GetUserId());
        return result?.IsDarkMode ?? false;
    }
    public async Task SetAsync(ClaimsPrincipal user, bool isDarkMode)
    {
        var userId = user.GetUserId();
        if (userId is null)
        {
            return;
        }
        var dbContext = ContextFactory.CreateDbContext();
        var userSettings = await dbContext.UserSettings.FindAsync(user.GetUserId());
        if (userSettings is null)
        {
            userSettings = new UserSettings
            {
                UserId = userId,
                IsDarkMode = isDarkMode
            };
            dbContext.UserSettings.Add(userSettings);
        }
        else
        {
            userSettings.IsDarkMode = isDarkMode;
        }
        await dbContext.SaveChangesAsync();
    }
}
