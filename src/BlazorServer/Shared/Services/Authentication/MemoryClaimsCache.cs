using Microsoft.Extensions.Caching.Memory;

namespace BlazorServer.Shared.Services.Authentication;

public class MemoryClaimsCache : IClaimsCache
{
    private readonly IMemoryCache _memoryCache;
    private MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public MemoryClaimsCache(IMemoryCache memoryCache)
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

    public void SetIsAdminRole(string userId, string orgId)
    {
        _memoryCache.Set($"isAdminRole:{userId}:{orgId}", true, _cacheOptions);
    }

    public bool TryGetIsAdminRole(string userId, string orgId, out bool isAdmin)
    {
        if (_memoryCache.TryGetValue($"isAdminRole:{userId}:{orgId}", out var cachedIsAdminObj) && cachedIsAdminObj is bool isAdminRole)
        {
            isAdmin = isAdminRole;
            return true;
        }
        isAdmin = false;
        return false;
    }

    public void ClearIsAdminRole(string userId, string orgId)
    {
        _memoryCache.Remove($"isAdminRole:{userId}:{orgId}");
    }
}
