using Microsoft.Identity.Web;
using System.Security.Claims;

namespace BlazorServer.Shared.Kernel.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimConstants.ObjectId)?.Value; 
    }
    public static string? GetOrganizationId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimConstants.TenantId)?.Value;
    }
    public static string? GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
    }
}
