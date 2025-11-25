using Core.Models;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Data.EfCore;
using BidconAccessModule.SdkAccess.Data.Abstractions;

namespace BidconAccessModule.SdkAccess.Data.EfCore;
internal class SqlCredentialsStore(IDbContextFactory<ApplicationDbContext> contextFactory) : ISdkCredentialsStore
{
    public Task<SdkCredentials?> GetSdkCredentialsAsync(string tenantId)
    {
        using var context = contextFactory.CreateDbContext();
        var result = context.SdkCredentials.Where(c => c.TenantId == tenantId).FirstOrDefaultAsync();
        return result;
    }
    public Task UpsertCredentialsAsync(string tenantId, SdkCredentials credentials)
    {
        using var context = contextFactory.CreateDbContext();

        //Check if credentials exists before updating or inserting
        var existingCredentials = context.SdkCredentials.Where(c => c.TenantId == tenantId).FirstOrDefault();

        if (existingCredentials is null)
        {
            credentials.TenantId = tenantId;
            context.SdkCredentials.Add(credentials);
        }
        else
        {
            existingCredentials.User = credentials.User;
            existingCredentials.Password = credentials.Password;
            existingCredentials.ConfigPath = credentials.ConfigPath;
        }
        return context.SaveChangesAsync();
    }
}
