using BidconAccessModule.DirectAccess.Data.Abstractions;
using Data.MongoDb.Enteties;
using MongoDB.Driver;

namespace BidconAccessModule.DirectAccess.Data.Mongo;
internal class MongoDirectCredentialsStore(IMongoCollection<BidconAccessCredentials> collection) : IDirectCredentialsStore
{
    public Task UpsertAsync(string tenantId, Core.Models.BidconAccessCredentials credentialsDto)
    {
        var credentials = new BidconAccessCredentials
        {
            TenantId = tenantId,
            Server = credentialsDto.Server,
            Database = credentialsDto.Database,
            User = credentialsDto.User,
            Password = credentialsDto.Password,
            ServerAuthentication = credentialsDto.ServerAuthentication,
            DesktopPort = credentialsDto.DesktopPort,
            UseDesktopBidconLink = credentialsDto.UseDesktopBidconLink
        };
        return collection.ReplaceOneAsync(x => x.TenantId == tenantId, credentials, new ReplaceOptions { IsUpsert = true });
    }

    Task<Core.Models.BidconAccessCredentials?> IDirectCredentialsStore.GetAsync(string tenantId)
    {
        var credentials = collection.Find(x => x.TenantId == tenantId).FirstOrDefault();
        return Task.FromResult(credentials is null ? null : new Core.Models.BidconAccessCredentials
        {
            TenantId = credentials.TenantId,
            Server = credentials.Server,
            Database = credentials.Database,
            User = credentials.User,
            Password = credentials.Password,
            ServerAuthentication = credentials.ServerAuthentication,
            DesktopPort = credentials.DesktopPort,
            UseDesktopBidconLink = credentials.UseDesktopBidconLink
        });
    }
}
