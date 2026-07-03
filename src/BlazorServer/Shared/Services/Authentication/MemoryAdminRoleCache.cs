using Microsoft.Extensions.Caching.Memory;

namespace BlazorServer.Shared.Services.Authentication;

public class MemoryAdminRoleCache : IAdminRoleCache
{
    private readonly IMemoryCache memoryCache;
    private readonly MemoryCacheEntryOptions cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    public MemoryAdminRoleCache(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public void SetIsAdminRole(string userId, string orgId)
    {
        memoryCache.Set($"isAdminRole:{userId}:{orgId}", true, cacheOptions);
    }

    public bool TryGetIsAdminRole(string userId, string orgId, out bool isAdmin)
    {
        if (memoryCache.TryGetValue($"isAdminRole:{userId}:{orgId}", out var cachedIsAdminObj) &&
            cachedIsAdminObj is bool isAdminRole)
        {
            isAdmin = isAdminRole;
            return true;
        }

        isAdmin = false;
        return false;
    }

    public void ClearIsAdminRole(string userId, string orgId)
    {
        memoryCache.Remove($"isAdminRole:{userId}:{orgId}");
    }
}
