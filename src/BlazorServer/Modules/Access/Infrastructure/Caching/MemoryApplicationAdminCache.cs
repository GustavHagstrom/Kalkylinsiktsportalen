using Microsoft.Extensions.Caching.Memory;

namespace BlazorServer.Modules.Access.Infrastructure.Caching;

internal sealed class MemoryApplicationAdminCache : IApplicationAdminCache
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

    public void SetApplicationAdminStatus(string userId, string organizationId, bool isApplicationAdmin)
    {
        memoryCache.Set(CacheKey(userId, organizationId), isApplicationAdmin, cacheOptions);
    }

    public bool TryGetApplicationAdminStatus(string userId, string organizationId, out bool isApplicationAdmin)
    {
        if (memoryCache.TryGetValue(CacheKey(userId, organizationId), out var cachedStatus)
            && cachedStatus is bool cachedIsApplicationAdmin)
        {
            isApplicationAdmin = cachedIsApplicationAdmin;
            return true;
        }

        isApplicationAdmin = false;
        return false;
    }

    private static string CacheKey(string userId, string organizationId)
        => $"applicationAdmin:{userId}:{organizationId}";
}
