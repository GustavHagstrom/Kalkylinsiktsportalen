namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed class ViewAccessGrantHandler(IViewAccessGrantStore store)
{
    public Task HandleAsync(GrantUserViewAccessCommand command)
    {
        return store.GrantAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.User,
            command.UserId,
            command.Right));
    }

    public Task HandleAsync(GrantRoleViewAccessCommand command)
    {
        return store.GrantAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.Role,
            command.RoleId,
            command.Right));
    }

    public Task HandleAsync(RevokeUserViewAccessCommand command)
    {
        return store.RevokeAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.User,
            command.UserId,
            command.Right));
    }

    public Task HandleAsync(RevokeRoleViewAccessCommand command)
    {
        return store.RevokeAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.Role,
            command.RoleId,
            command.Right));
    }
}
