using BidconAccessModule.DirectAccess.Data.Abstractions;
using Data.EfCore;
using Microsoft.EntityFrameworkCore;

namespace BidconAccessModule.DirectAccess.Data.EfCore;
public class SqlDbConnectionStringStore(IDbContextFactory<ApplicationDbContext> DbContextFactory) : IDbConnectionStringStore
{
    public async Task<string> BuildAsync(string tenantId)
    {
        using var context = await DbContextFactory.CreateDbContextAsync();
        var credentials = await context.BidconAccessCredentials.Where(x => x.TenantId == tenantId).FirstOrDefaultAsync();

        if (credentials is null)
        {
            throw new Exception("No credentials found for user");
        }

        return credentials.ServerAuthentication
            ? $"Data Source={credentials.Server};Initial Catalog={credentials.Database}; Connect Timeout = 60;uid={credentials.User};pwd={credentials.Password};TrustServerCertificate=True"
            : $"Data Source={credentials.Server};Initial Catalog={credentials.Database}; Connect Timeout = 60;Integrated security=true;TrustServerCertificate=True;Encrypt=False;Multi Subnet Failover=False";
    }
}
