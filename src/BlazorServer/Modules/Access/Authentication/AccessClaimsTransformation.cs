using BlazorServer.Shared.Constants;
using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using BlazorServer.AppShell.Authentication;

namespace BlazorServer.Modules.Access.Authentication;

public class AccessClaimsTransformation : IClaimsTransformation
{
    private readonly FunctionalAccessGrantHandler functionalAccessGrants;
    private readonly IAccessClaimsCache _cache;

    public AccessClaimsTransformation(
        FunctionalAccessGrantHandler functionalAccessGrants,
        IAccessClaimsCache cache)
    {
        this.functionalAccessGrants = functionalAccessGrants;
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
            var accessClaims = await functionalAccessGrants.HandleAsync(
                new GetEffectiveFunctionalAccessClaimsQuery(userId, groupIds, orgId));
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
