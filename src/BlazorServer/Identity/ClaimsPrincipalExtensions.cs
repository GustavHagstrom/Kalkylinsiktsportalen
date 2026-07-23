using System.Security.Claims;

namespace BlazorServer.Identity;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(IdentityClaimTypes.UserId);

    public static string? GetOrganizationId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(IdentityClaimTypes.OrganizationId);

    public static string? GetUserEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(IdentityClaimTypes.Email);

    public static IEnumerable<string> GetGroupIds(this ClaimsPrincipal principal)
        => principal.FindAll(IdentityClaimTypes.Groups)
            .Select(claim => claim.Value);
}
