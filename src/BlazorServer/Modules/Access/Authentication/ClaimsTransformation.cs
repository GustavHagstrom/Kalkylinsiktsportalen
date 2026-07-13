using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace BlazorServer.Modules.Access.Authentication;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly FunctionalAccessGrantHandler functionalAccessGrants;
    private readonly IAccessClaimsCache _cache;

    public ClaimsTransformation(
        FunctionalAccessGrantHandler functionalAccessGrants,
        IAccessClaimsCache cache)
    {
        this.functionalAccessGrants = functionalAccessGrants;
        _cache = cache;
    }
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimConstants.ObjectId);
        var orgId = principal.FindFirstValue(ClaimConstants.TenantId);
        var groupIds = principal.FindAll("groups").Select(c => c.Value).ToList();
        
        if(userId is null || orgId is null) return principal;        

        if (!_cache.TryGetAccessClaims(userId, orgId, out List<string>? accessClaimValues))
        {
            _cache.ClearAccessClaims(userId, orgId);
            var accessClaims = await functionalAccessGrants.HandleAsync(new GetEffectiveFunctionalAccessClaimsQuery(userId, groupIds, orgId));
            accessClaimValues = [.. accessClaims.Select(x => x.Value)];
            _cache.SetAccessClaims(userId, orgId, accessClaimValues);
        }
        if (accessClaimValues is not null)
        {
            foreach (var value in accessClaimValues)
            {
                if (!principal.HasClaim(ApplicationClaimConstants.AccessClaimType, value))
                {
                    ((ClaimsIdentity)principal.Identity!).AddClaim(new AccessClaim(value));
                }
            }
        }

        return principal;
    }
}
