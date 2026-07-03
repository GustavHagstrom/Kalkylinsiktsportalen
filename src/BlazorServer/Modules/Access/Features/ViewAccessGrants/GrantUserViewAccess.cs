namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record GrantUserViewAccessCommand(
    string ViewId,
    string UserId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed class GrantUserViewAccessHandler(IViewAccessGrantStore store)
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
}
