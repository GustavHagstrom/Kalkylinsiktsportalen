using Microsoft.Identity.Web;
using System.Security.Claims;

namespace Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal userPrincipal)
    {
        return userPrincipal.FindFirst(ClaimConstants.ObjectId)?.Value;
    }
    public static string? GetTenantId(this ClaimsPrincipal userPrincipal)
    {
        return userPrincipal.FindFirst(ClaimConstants.TenantId)?.Value;
    }
}
