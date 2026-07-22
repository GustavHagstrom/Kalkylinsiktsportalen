using BlazorServer.Identity;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public interface IBidconApiCredentialsStore
{
    Task<BidconApiCredentials?> GetAsync(OrganizationId orgId);

    Task UpsertAsync(BidconApiCredentials credentials);

    Task DeleteAsync(OrganizationId orgId);
}
