using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Extensions;
using BlazorServer.Shared.Models;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace BlazorServer.Shared.Services.Authentication;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly FunctionalAccessGrantUseCases accessGrants;
    private readonly IClaimsCache _cache;

    public ClaimsTransformation(FunctionalAccessGrantUseCases accessGrants, IClaimsCache cache)
    {
        this.accessGrants = accessGrants;
        _cache = cache;
    }
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();
        var orgId = principal.GetOrganizationId();
        var groupIds = principal.FindAll("groups").Select(c => c.Value).ToList();
        
        if(userId is null || orgId is null) return principal;        

        if (!_cache.TryGetAccessClaims(userId, orgId, out List<string>? accessClaimValues))
        {
            _cache.ClearAccessClaims(userId, orgId);
            var accessClaims = await accessGrants.GetAccessClaimsForUserAsync(userId, groupIds, orgId);
            foreach (var claim in accessClaims)
            {
                if (!principal.HasClaim(claim.ValueType, claim.Value))
                {
                    ((ClaimsIdentity)principal.Identity!).AddClaim(claim);
                }
            }
            var values = accessClaims.Select(c => c.Value).ToList();
            _cache.SetAccessClaims(userId, orgId, values);
        }
        else if (accessClaimValues is not null)
        {
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
    
}
