using Core.Models;
using Microsoft.EntityFrameworkCore;
using Data.EfCore;
using BidconAccessModule.DirectAccess.Data.Abstractions;

namespace BidconAccessModule.DirectAccess.Data.EfCore;

public class SqlDirectCredentialsStore(IDbContextFactory<ApplicationDbContext> ContextFactory) : IDirectCredentialsStore
{
    public async Task<BidconAccessCredentials?> GetAsync(string tenantId)
    {
        var dbContext = ContextFactory.CreateDbContext();
        var result = await dbContext.BidconAccessCredentials.FirstOrDefaultAsync(x => x.TenantId == tenantId);
        return result;
    }

    public async Task UpsertAsync(string tenantId, BidconAccessCredentials credentials)
    {
        credentials.TenantId = tenantId;
        var dbContext = ContextFactory.CreateDbContext();
        var existingCredentials = await dbContext.BidconAccessCredentials
            .FirstOrDefaultAsync(x => x.TenantId == tenantId);
        if (existingCredentials is null)
        {
            // Insert a new record if it doesn't exist

            dbContext.BidconAccessCredentials.Add(credentials);
        }
        else
        {
            // Update the existing record if it exists
            existingCredentials.TenantId = tenantId;
            existingCredentials.Server = credentials.Server;
            existingCredentials.Database = credentials.Database;
            existingCredentials.User = credentials.User;
            existingCredentials.Password = credentials.Password;
            existingCredentials.ServerAuthentication = credentials.ServerAuthentication;
            existingCredentials.UseDesktopBidconLink = credentials.UseDesktopBidconLink;
        }
        await dbContext.SaveChangesAsync();

    }
}
