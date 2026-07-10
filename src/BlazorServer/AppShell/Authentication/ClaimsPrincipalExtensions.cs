using System.Security.Claims;
using BlazorServer.Shared.Constants;
using Microsoft.Identity.Web;

namespace BlazorServer.AppShell.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimConstants.ObjectId);

    public static string? GetOrganizationId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimConstants.TenantId);

    public static string? GetUserEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

    public static bool IsApplicationAdmin(this ClaimsPrincipal principal)
        => principal.IsInRole(AuthConstants.AdminRoleClaimValue);

    public static bool HasAccessClaimOrApplicationAdmin(this ClaimsPrincipal principal, string accessClaimValue)
        => principal.HasClaim(ApplicationClaimTypes.AccessClaim, accessClaimValue) ||
           principal.IsApplicationAdmin();
}
