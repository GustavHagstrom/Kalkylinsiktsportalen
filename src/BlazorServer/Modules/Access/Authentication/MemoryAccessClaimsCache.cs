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
    public void SetAccessClaims(string userId, string orgId, List<string> claimValues)
    {
        _memoryCache.Set($"accessClaims:{userId}:{orgId}", claimValues, _cacheOptions);
    }
    public bool TryGetAccessClaims(string userId, string orgId, out List<string>? claimValues)
    {
        if (_memoryCache.TryGetValue($"accessClaims:{userId}:{orgId}", out var cachedClaimsObj) && cachedClaimsObj is List<string> accessClaimValues)
        {
            claimValues = accessClaimValues;
            return true;
        }
        claimValues = null;
        return false;
    }
    public void ClearAccessClaims(string userId, string orgId)
    {
        _memoryCache.Remove($"accessClaims:{userId}:{orgId}");
    }

}
