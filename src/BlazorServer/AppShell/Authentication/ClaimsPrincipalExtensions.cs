using System.Security.Claims;
using BlazorServer.Identity;

namespace BlazorServer.AppShell.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUser? ToCurrentUser(this ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }
        var userId = principal.GetUserId();
        var organizationId = principal.GetOrganizationId();
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(organizationId))
        {
            return null;
        }

        var displayName = principal.Identity?.Name ?? userId;
        return new CurrentUser(new UserId(userId), new OrganizationId(organizationId), displayName, principal.GetUserEmail());
    }
}
