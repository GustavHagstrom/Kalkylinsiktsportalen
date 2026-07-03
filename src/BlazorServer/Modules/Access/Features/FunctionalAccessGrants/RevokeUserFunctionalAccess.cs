namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record RevokeUserFunctionalAccessCommand(
    string UserId,
    string OrganizationId,
    string AccessClaimValue);

public sealed class RevokeUserFunctionalAccessHandler(IFunctionalAccessGrantStore store)
{
    public Task HandleAsync(RevokeUserFunctionalAccessCommand command)
    {
        return store.DeleteUserAccessAsync(
            command.UserId,
            command.OrganizationId,
            command.AccessClaimValue);
    }
}
