namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record RevokeRoleViewAccessCommand(
    string ViewId,
    string RoleId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed class RevokeRoleViewAccessHandler(IViewAccessGrantStore store)
{
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
