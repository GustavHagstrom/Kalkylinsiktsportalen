namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed class ViewAccessGrantUseCases(IViewAccessGrantStore store)
{
    public Task GrantUserViewAccessAsync(
        string viewId,
        string userId,
        string organizationId,
        ViewAccessRight right)
    {
        return store.GrantAsync(new ViewAccessGrant(
            viewId,
            organizationId,
            ViewAccessGrantTargetType.User,
            userId,
            right));
    }

    public Task GrantRoleViewAccessAsync(
        string viewId,
        string roleId,
        string organizationId,
        ViewAccessRight right)
    {
        return store.GrantAsync(new ViewAccessGrant(
            viewId,
            organizationId,
            ViewAccessGrantTargetType.Role,
            roleId,
            right));
    }

    public Task RevokeUserViewAccessAsync(
        string viewId,
        string userId,
        string organizationId,
        ViewAccessRight right)
    {
        return store.RevokeAsync(new ViewAccessGrant(
            viewId,
            organizationId,
            ViewAccessGrantTargetType.User,
            userId,
            right));
    }

    public Task RevokeRoleViewAccessAsync(
        string viewId,
        string roleId,
        string organizationId,
        ViewAccessRight right)
    {
        return store.RevokeAsync(new ViewAccessGrant(
            viewId,
            organizationId,
            ViewAccessGrantTargetType.Role,
            roleId,
            right));
    }

    public async Task<bool> HasViewAccessAsync(
        string viewId,
        string userId,
        IReadOnlyCollection<string> roleIds,
        string organizationId,
        ViewAccessRight right)
    {
        var roleIdSet = roleIds.ToHashSet(StringComparer.Ordinal);
        var grants = await store.GetGrantsForViewAsync(viewId, organizationId);

        return grants.Any(grant =>
            grant.Right == right &&
            IsGrantForUserOrRole(grant, userId, roleIdSet));
    }

    private static bool IsGrantForUserOrRole(
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
