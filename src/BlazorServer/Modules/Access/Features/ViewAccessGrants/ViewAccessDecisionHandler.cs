using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed class ViewAccessDecisionHandler(IViewAccessGrantStore store)
{
    public Task<bool> HandleAsync(CanUseViewQuery query)
    {
        return HasViewAccessAsync(
            query.Subject,
            query.ViewId,
            ViewAccessRight.UseView);
    }

    public Task<bool> HandleAsync(CanEditViewQuery query)
    {
        return HasViewAccessAsync(
            query.Subject,
            query.ViewId,
            ViewAccessRight.EditView);
    }

    public Task<bool> HandleAsync(CanAdministerViewAccessQuery query)
    {
        return HasViewAccessAsync(
            query.Subject,
            query.ViewId,
            ViewAccessRight.AdministerViewAccess);
    }

    async Task<bool> HasViewAccessAsync(
        AccessSubject subject,
        ViewId viewId,
        ViewAccessRight right)
    {
        if (subject.IsApplicationAdmin)
        {
            return true;
        }

        var grants = await store.GetGrantsForViewAsync(viewId, subject.OrganizationId);

        return grants.Any(grant =>
            grant.Right == right &&
            IsGrantForUserOrRole(grant, subject.UserId, subject.RoleIds));
    }

    static bool IsGrantForUserOrRole(
        ViewAccessGrant grant,
        string userId,
        IReadOnlySet<string> roleIds)
    {
        return grant.TargetType switch
        {
            ViewAccessGrantTargetType.User => grant.TargetId == userId,
            ViewAccessGrantTargetType.Role => roleIds.Contains(grant.TargetId),
            _ => false
        };
    }
}
