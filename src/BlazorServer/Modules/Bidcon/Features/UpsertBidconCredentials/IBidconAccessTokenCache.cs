using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public interface IBidconAccessTokenCache
{
    void Invalidate(OrganizationId organizationId);
}
