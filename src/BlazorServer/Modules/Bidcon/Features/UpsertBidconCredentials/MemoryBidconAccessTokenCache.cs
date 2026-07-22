using BlazorServer.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed class MemoryBidconAccessTokenCache(IMemoryCache cache) : IBidconAccessTokenCache
{
    public void Invalidate(OrganizationId organizationId)
    {
        cache.Remove($"bidconAccessToken_{organizationId.Value}");
    }
}
