using BlazorServer.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorServer.Modules.Access.Authentication;

public class MemoryAccessClaimsCache : IAccessClaimsCache
{
    private readonly IMemoryCache _memoryCache;
    private MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    public MemoryAccessClaimsCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public void SetAccessClaims(UserId userId, OrganizationId orgId, List<string> claimValues)
    {
        _memoryCache.Set($"accessClaims:{userId.Value}:{orgId.Value}", claimValues, _cacheOptions);
    }
    public bool TryGetAccessClaims(UserId userId, OrganizationId orgId, out List<string>? claimValues)
    {
        if (_memoryCache.TryGetValue($"accessClaims:{userId.Value}:{orgId.Value}", out var cachedClaimsObj) && cachedClaimsObj is List<string> accessClaimValues)
        {
            claimValues = accessClaimValues;
            return true;
        }
        claimValues = null;
        return false;
    }
    public void ClearAccessClaims(UserId userId, OrganizationId orgId)
    {
        _memoryCache.Remove($"accessClaims:{userId.Value}:{orgId.Value}");
    }

}
