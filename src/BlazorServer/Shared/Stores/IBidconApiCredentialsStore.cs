using BlazorServer.Shared.Models.Bidcon;

namespace BlazorServer.Shared.Stores;

public interface IBidconApiCredentialsStore
{
    Task<BidconApiCredentials> GetAsync(string orgId);
    Task UpsertAsync(BidconApiCredentials credentials);
}