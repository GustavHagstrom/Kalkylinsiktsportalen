using Microsoft.Extensions.Caching.Memory;

namespace BlazorServer.AppShell.Authentication;

public class MemoryApplicationAdminCache : IApplicationAdminCache
{
    private readonly IMemoryCache memoryCache;
    private readonly MemoryCacheEntryOptions cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    public MemoryApplicationAdminCache(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public void SetIsAdminRole(string userId, string organizationId)
    {
        memoryCache.Set($"isAdminRole:{userId}:{organizationId}", true, cacheOptions);
    }

    public bool TryGetIsAdminRole(string userId, string organizationId, out bool isAdmin)
    {
        if (memoryCache.TryGetValue($"isAdminRole:{userId}:{organizationId}", out var cachedIsAdminObj) &&
            cachedIsAdminObj is bool isAdminRole)
        {
            isAdmin = isAdminRole;
            return true;
        }

        isAdmin = false;
        return false;
    }

    public void ClearIsAdminRole(string userId, string organizationId)
    {
        memoryCache.Remove($"isAdminRole:{userId}:{organizationId}");
    }
}
