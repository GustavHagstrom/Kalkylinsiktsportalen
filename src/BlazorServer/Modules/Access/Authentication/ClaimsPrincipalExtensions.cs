using System.Security.Claims;
using Microsoft.Identity.Web;

namespace BlazorServer.Modules.Access.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimConstants.ObjectId);

    public static string? GetOrganizationId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimConstants.TenantId);
}
