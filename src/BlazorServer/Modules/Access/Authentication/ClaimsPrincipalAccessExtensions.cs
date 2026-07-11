using System.Security.Claims;

namespace BlazorServer.Modules.Access.Authentication;

public static class ClaimsPrincipalAccessExtensions
{
    public static bool IsApplicationAdmin(this ClaimsPrincipal principal)
        => principal.HasClaim(ApplicationClaimTypes.ExternalDirectoryRole, AccessPolicies.ApplicationAdminRoleClaimValue);

    public static bool HasAccessClaimOrApplicationAdmin(this ClaimsPrincipal principal, string accessClaimValue)
        => principal.HasClaim(ApplicationClaimTypes.AccessClaim, accessClaimValue) ||
           principal.IsApplicationAdmin();
}
