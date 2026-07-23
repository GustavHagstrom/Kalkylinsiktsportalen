using System.Security.Claims;

namespace BlazorServer.Modules.Access.Public.Authorization;

public static class ClaimsPrincipalAccessExtensions
{
    public static bool IsApplicationAdmin(this ClaimsPrincipal principal)
        => principal.HasClaim(
            AccessAuthorizationConstants.ExternalDirectoryRoleClaimType,
            AccessAuthorizationConstants.ApplicationAdminRoleClaimValue);

    public static bool HasAccessClaimOrApplicationAdmin(
        this ClaimsPrincipal principal,
        string accessClaimValue)
        => principal.HasClaim(AccessAuthorizationConstants.AccessClaimType, accessClaimValue)
            || principal.IsApplicationAdmin();
}
