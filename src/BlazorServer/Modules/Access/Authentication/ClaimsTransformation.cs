using BlazorServer.AppShell.Authentication;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace BlazorServer.Modules.Access.Authentication;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly FunctionalAccessGrantHandler functionalAccessGrants;
    private readonly IApplicationAdminLookup _applicationAdminLookup;
    private readonly IAccessClaimsCache _cache;

    public ClaimsTransformation(
        FunctionalAccessGrantHandler functionalAccessGrants,
        IApplicationAdminLookup applicationAdminLookup,
        IAccessClaimsCache cache)
    {
        this.functionalAccessGrants = functionalAccessGrants;
        _applicationAdminLookup = applicationAdminLookup;
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
            var accessClaims = await functionalAccessGrants.HandleAsync(new GetEffectiveFunctionalAccessClaimsQuery(userId, groupIds, orgId));
            accessClaimValues = [.. accessClaims.Select(x => x.Value)];
            _cache.SetAccessClaims(userId, orgId, accessClaimValues);
        }
        if (accessClaimValues is not null)
        {
            foreach (var value in accessClaimValues)
            {
                if (!principal.HasClaim(ApplicationClaimTypes.AccessClaim, value))
                {
                    ((ClaimsIdentity)principal.Identity!).AddClaim(new AccessClaim(value));
                }
            }
        }
        var isAdmin = await _applicationAdminLookup.IsApplicationAdminAsync(userId, orgId);
        if (isAdmin && !principal.HasClaim(ClaimTypes.Role, AccessPolicies.ApplicationAdminRoleClaimValue))
        {
            ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(ClaimTypes.Role, AccessPolicies.ApplicationAdminRoleClaimValue));
        }
        return principal;
    }
    
}
