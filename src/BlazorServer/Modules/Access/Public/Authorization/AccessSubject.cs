using System.Security.Claims;
using Microsoft.Identity.Web;

namespace BlazorServer.Modules.Access.Public.Authorization;

public sealed record AccessSubject(
    string UserId,
    string OrganizationId,
    IReadOnlySet<string> RoleIds,
    bool IsApplicationAdmin);

public static class ClaimsPrincipalAccessSubjectExtensions
{
    public static AccessSubject? ToAccessSubject(this ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimConstants.ObjectId);
        var organizationId = principal.FindFirstValue(ClaimConstants.TenantId);
        if (userId is null || organizationId is null)
        {
            return null;
        }

        var roleIds = principal.FindAll("groups")
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.Ordinal);

        return new AccessSubject(
            userId,
            organizationId,
            roleIds,
            principal.IsApplicationAdmin());
    }
}
