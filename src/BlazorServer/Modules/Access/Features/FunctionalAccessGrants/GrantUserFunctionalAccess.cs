namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record GrantUserFunctionalAccessCommand(
    string UserId,
    string OrganizationId,
    string AccessClaimValue);

public sealed class GrantUserFunctionalAccessHandler(IFunctionalAccessGrantStore store)
{
    public Task HandleAsync(GrantUserFunctionalAccessCommand command)
    {
        return store.AddUserAccessAsync(
            command.UserId,
            command.OrganizationId,
            command.AccessClaimValue);
    }
}
