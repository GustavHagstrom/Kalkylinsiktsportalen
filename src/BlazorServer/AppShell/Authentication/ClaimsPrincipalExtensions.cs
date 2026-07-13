using System.Security.Claims;
using Microsoft.Identity.Web;

namespace BlazorServer.AppShell.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimConstants.ObjectId);

    public static string? GetOrganizationId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimConstants.TenantId);

    public static string? GetUserEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(CurrentUserSessionClaimTypes.Email);

    public static List<string> GetDirectoryRoles(this ClaimsPrincipal principal)
        => principal.FindAll(CurrentUserSessionClaimTypes.DirectoryRoles).Select(claim => claim.Value).ToList();

    public static CurrentUser? ToCurrentUser(this ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var displayName = principal.Identity?.Name ?? userId;
        return new CurrentUser(userId, displayName, principal.GetUserEmail());
    }
}
