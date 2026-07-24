using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace BlazorServer.Modules.Access.Authentication;

public class ClaimsTransformation : IClaimsTransformation
{
    private readonly FunctionalAccessGrantHandler functionalAccessGrants;
    private readonly IAccessClaimsCache _cache;
    private readonly IAccessSubjectFactory _accessSubjectFactory;

    public ClaimsTransformation(
        FunctionalAccessGrantHandler functionalAccessGrants,
        IAccessClaimsCache cache,
        IAccessSubjectFactory accessSubjectFactory)
    {
        this.functionalAccessGrants = functionalAccessGrants;
        _cache = cache;
        _accessSubjectFactory = accessSubjectFactory;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var subject = _accessSubjectFactory.Create(principal);
        if (subject is null)
        {
            return principal;
        }

        if (!_cache.TryGetAccessClaims(
            subject.UserId,
            subject.OrganizationId,
            out List<string>? accessClaimValues))
        {
            _cache.ClearAccessClaims(subject.UserId, subject.OrganizationId);
            var accessClaims = await functionalAccessGrants.HandleAsync(
                new GetEffectiveFunctionalAccessClaimsQuery(
                    subject.UserId,
                    subject.RoleIds.ToList(),
                    subject.OrganizationId));
            accessClaimValues = [.. accessClaims.Select(x => x.Value)];
            _cache.SetAccessClaims(
                subject.UserId,
                subject.OrganizationId,
                accessClaimValues);
        }

        if (accessClaimValues is not null)
        {
            foreach (var value in accessClaimValues)
            {
                if (!principal.HasClaim(AccessAuthorizationConstants.AccessClaimType, value))
                {
                    ((ClaimsIdentity)principal.Identity!).AddClaim(new AccessClaim(value));
                }
            }
        }

        return principal;
    }
}
