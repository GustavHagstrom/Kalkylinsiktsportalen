using BlazorServer.Identity;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace BlazorServer.Modules.Access.Public.Authorization;

public sealed record AccessSubject(
    UserId UserId,
    OrganizationId OrganizationId,
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
            new UserId(userId),
            new OrganizationId(organizationId),
            roleIds,
            principal.IsApplicationAdmin());
    }
}
