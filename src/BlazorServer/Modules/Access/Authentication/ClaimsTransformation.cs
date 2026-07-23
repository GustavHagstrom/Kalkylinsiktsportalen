using BlazorServer.Modules.Access.Application;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace BlazorServer.Modules.Access.Authentication;

public sealed class ClaimsTransformation : IClaimsTransformation
{
    private readonly FunctionalAccessGrantHandler functionalAccessGrants;
    private readonly IApplicationAdminResolver applicationAdminResolver;
    private readonly IAccessClaimsCache cache;

    public ClaimsTransformation(
        FunctionalAccessGrantHandler functionalAccessGrants,
        IApplicationAdminResolver applicationAdminResolver,
        IAccessClaimsCache cache)
    {
        this.functionalAccessGrants = functionalAccessGrants;
        this.applicationAdminResolver = applicationAdminResolver;
        this.cache = cache;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var subject = principal.ToAccessSubject();
        if (subject is null)
        {
            return principal;
        }

        if (!cache.TryGetAccessClaims(
            subject.UserId,
            subject.OrganizationId,
            out List<string>? accessClaimValues))
        {
            cache.ClearAccessClaims(subject.UserId, subject.OrganizationId);
            var accessClaims = await functionalAccessGrants.HandleAsync(
                new GetEffectiveFunctionalAccessClaimsQuery(
                    subject.UserId,
                    subject.RoleIds.ToList(),
                    subject.OrganizationId));
            accessClaimValues = [.. accessClaims.Select(x => x.Value)];
            cache.SetAccessClaims(
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

        var isApplicationAdmin = await applicationAdminResolver.IsApplicationAdminAsync(
            subject.UserId.Value,
            subject.OrganizationId.Value);
        if (isApplicationAdmin && !principal.IsApplicationAdmin())
        {
            ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(
                AccessAuthorizationConstants.ExternalDirectoryRoleClaimType,
                AccessAuthorizationConstants.ApplicationAdminRoleClaimValue));
        }

        return principal;
    }
}
