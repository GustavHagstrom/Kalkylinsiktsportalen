namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record GrantRoleViewAccessCommand(
    string ViewId,
    string RoleId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed class GrantRoleViewAccessHandler(IViewAccessGrantStore store)
{
    public Task HandleAsync(GrantRoleViewAccessCommand command)
    {
        return store.GrantAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.Role,
            command.RoleId,
            command.Right));
    }
}
