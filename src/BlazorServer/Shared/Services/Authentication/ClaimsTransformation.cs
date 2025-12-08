using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Extensions;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace BlazorServer.Shared.Services.Authentication;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly IAccessStore _accessStore;
    private readonly IRoleService _roleService;
    private readonly IMemoryCache _cache;
    private MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public ClaimsTransformation(IAccessStore accessStore, IRoleService roleService, IMemoryCache memoryCache)
    {
        _accessStore = accessStore;
        _roleService = roleService;
        _cache = memoryCache;
    }
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();
        var orgId = principal.GetOrganizationId();
        var groupIds = principal.FindAll("groups").Select(c => c.Value).ToList();
        
        if(userId is null || orgId is null) return principal;

        try
        {
            if (!_cache.TryGetValue($"isAdmin:{userId}:{orgId}", out bool isAdmin))
            {
                isAdmin = await _roleService.IsAdminAsync(userId, orgId);
                _cache.Set($"isAdmin:{userId}:{orgId}", isAdmin, _cacheOptions);
                SetAdminRole(principal);
            }
            else if (isAdmin)
            {
                SetAdminRole(principal);
            }
        }
        catch (Exception)
        {
            //log?
        }

        

        if (!_cache.TryGetValue($"accessClaims:{userId}:{orgId}", out var cachedClaimsObj) || cachedClaimsObj is not List<string> accessClaimValues)
        {
            _cache.Remove($"accessClaims:{userId}:{orgId}");
            var accessClaims = await _accessStore.GetAccessClaimsForUserAsync(userId, groupIds, orgId);
            foreach (var claim in accessClaims)
            {
                if (!principal.HasClaim(claim.ValueType, claim.Value))
                {
                    ((ClaimsIdentity)principal.Identity!).AddClaim(claim);
                }
            }
            var values = accessClaims.Select(c => c.Value).ToList();
            _cache.Set($"accessClaims:{userId}:{orgId}", values, _cacheOptions);
            // cachedClaims is not available or not of type List<Claim>
            // You can handle cache miss here if needed
        }
        else
        {
            // cachedClaims is available and of correct type
            foreach (var value in accessClaimValues)
            {
                if (!principal.HasClaim(ApplicationClaimTypes.AccessClaim, value))
                {
                    ((ClaimsIdentity)principal.Identity!).AddClaim(new AccessClaim(value));
                }
            }
        }        
        return principal;
    }
    void SetAdminRole(ClaimsPrincipal principal)
    {
        if (!principal.IsInRole(AuthConstants.AdminRoleClaimValue))
        {
            ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(ClaimTypes.Role, AuthConstants.AdminRoleClaimValue));
        }
    }
}
