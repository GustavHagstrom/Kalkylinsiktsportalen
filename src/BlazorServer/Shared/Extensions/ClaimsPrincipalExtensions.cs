using BlazorServer.Shared.Constants;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace BlazorServer.Shared.Extensions;

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

}
