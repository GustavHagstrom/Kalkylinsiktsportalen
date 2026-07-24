using BlazorServer.Identity;
using BlazorServer.Modules.Access.Public.Authorization;
using System.Security.Claims;

namespace BlazorServer.Modules.Access.Authentication;

public sealed class AccessSubjectFactory(
    IApplicationAdminPrincipalEvaluator applicationAdminEvaluator)
    : IAccessSubjectFactory
{
    public AccessSubject? Create(ClaimsPrincipal principal)
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
            applicationAdminEvaluator.IsApplicationAdmin(principal));
    }
}
