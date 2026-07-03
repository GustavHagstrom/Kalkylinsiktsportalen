namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record RevokeRoleFunctionalAccessCommand(
    string RoleId,
    string OrganizationId,
    string AccessClaimValue);

public sealed class RevokeRoleFunctionalAccessHandler(IFunctionalAccessGrantStore store)
{
    public Task HandleAsync(RevokeRoleFunctionalAccessCommand command)
    {
        return store.DeleteRoleAccessAsync(
            command.RoleId,
            command.OrganizationId,
            command.AccessClaimValue);
    }
}
