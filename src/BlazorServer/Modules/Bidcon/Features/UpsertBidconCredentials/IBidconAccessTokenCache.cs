using BlazorServer.Identity;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public interface IBidconAccessTokenCache
{
    bool TryGet(OrganizationId organizationId, out string? accessToken);

    void Set(OrganizationId organizationId, string accessToken, TimeSpan lifetime);

    void Invalidate(OrganizationId organizationId);
}
