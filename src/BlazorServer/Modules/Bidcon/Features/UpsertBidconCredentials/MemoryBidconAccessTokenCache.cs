using BlazorServer.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed class MemoryBidconAccessTokenCache(IMemoryCache cache) : IBidconAccessTokenCache
{
    public bool TryGet(OrganizationId organizationId, out string? accessToken)
    {
        return cache.TryGetValue(CacheKey(organizationId), out accessToken) &&
            !string.IsNullOrWhiteSpace(accessToken);
    }

    public void Set(OrganizationId organizationId, string accessToken, TimeSpan lifetime)
    {
        cache.Set(CacheKey(organizationId), accessToken, lifetime);
    }

    public void Invalidate(OrganizationId organizationId)
    {
        cache.Remove(CacheKey(organizationId));
    }

    private static string CacheKey(OrganizationId organizationId) => $"bidconAccessToken_{organizationId.Value}";
}
