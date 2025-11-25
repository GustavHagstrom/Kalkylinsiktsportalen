using BidconAccessModule.DirectAccess.Data.Abstractions;
using MongoDB.Driver;
using Data.MongoDb.Enteties;
namespace BidconAccessModule.DirectAccess.Data.Mongo;
internal class MongoDbConnectionStringStore(IMongoCollection<BidconAccessCredentials> collection) : IDbConnectionStringStore
{
    public async Task<string> BuildAsync(string tenantId)
    {
        var credentials = await collection.Find(x => x.TenantId == tenantId).FirstOrDefaultAsync();
        if (credentials is null)
        {
            throw new Exception("No credentials found for user");
        }
        return credentials.ServerAuthentication
            ? $"Data Source={credentials.Server};Initial Catalog={credentials.Database}; Connect Timeout = 60;uid={credentials.User};pwd={credentials.Password};TrustServerCertificate=True"
            : $"Data Source={credentials.Server};Initial Catalog={credentials.Database}; Connect Timeout = 60;Integrated security=true;TrustServerCertificate=True;Encrypt=False;Multi Subnet Failover=False";
    }
}
