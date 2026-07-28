using BlazorServer.Identity;
using BlazorServer.Modules.Access.Persistence;
using BlazorServer.Modules.Access.Public.Authorization;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed class ViewAccessDecisionHandler(IViewAccessGrantStore store) : IViewAccessDecisionService
{
    public Task<bool> CanUseViewAsync(AccessSubject subject, ViewId viewId)
        => HasViewAccessAsync(subject, viewId, ViewAccessRight.UseView);

    public Task<bool> CanEditViewAsync(AccessSubject subject, ViewId viewId)
        => HasViewAccessAsync(subject, viewId, ViewAccessRight.EditView);

    public Task<bool> CanAdministerViewAccessAsync(AccessSubject subject, ViewId viewId)
        => HasViewAccessAsync(subject, viewId, ViewAccessRight.AdministerViewAccess);

    private async Task<bool> HasViewAccessAsync(
        AccessSubject subject,
        ViewId viewId,
        ViewAccessRight right)
    {
        if (subject.IsApplicationAdmin)
        {
            return true;
        }

        var grants = await store.GetGrantsForViewAsync(viewId, subject.OrganizationId.Value);

        return grants.Any(grant =>
            grant.Right == right &&
            IsGrantForUserOrRole(grant, subject.UserId, subject.RoleIds));
    }

    private static bool IsGrantForUserOrRole(
        ViewAccessGrant grant,
        UserId userId,
        IReadOnlySet<string> roleIds)
    {
        return grant.TargetType switch
        {
            ViewAccessGrantTargetType.User => grant.TargetId == userId.Value,
            ViewAccessGrantTargetType.Role => roleIds.Contains(grant.TargetId),
            _ => false
        };
    }
}
