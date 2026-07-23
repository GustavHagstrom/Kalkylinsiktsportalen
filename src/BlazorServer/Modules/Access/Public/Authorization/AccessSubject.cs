using BlazorServer.Identity;
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
        var userId = principal.GetUserId();
        var organizationId = principal.GetOrganizationId();
        if (userId is null || organizationId is null)
        {
            return null;
        }

        var roleIds = principal.GetGroupIds()
            .ToHashSet(StringComparer.Ordinal);

        return new AccessSubject(
            new UserId(userId),
            new OrganizationId(organizationId),
            roleIds,
            principal.IsApplicationAdmin());
    }
}
