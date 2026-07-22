using BlazorServer.Identity;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public interface IBidconAccessTokenCache
{
    void Invalidate(OrganizationId organizationId);
}
