namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public interface IBidconApiCredentialsStore
{
    Task<BidconApiCredentials?> GetAsync(string orgId);

    Task UpsertAsync(BidconApiCredentials credentials);

    Task DeleteAsync(string orgId);
}
