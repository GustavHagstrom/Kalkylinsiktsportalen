namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record RevokeUserViewAccessCommand(
    string ViewId,
    string UserId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed class RevokeUserViewAccessHandler(IViewAccessGrantStore store)
{
    public Task HandleAsync(RevokeUserViewAccessCommand command)
    {
        return store.RevokeAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.User,
            command.UserId,
            command.Right));
    }
}
