namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record GrantRoleFunctionalAccessCommand(
    string RoleId,
    string OrganizationId,
    string AccessClaimValue);

public sealed class GrantRoleFunctionalAccessHandler(IFunctionalAccessGrantStore store)
{
    public Task HandleAsync(GrantRoleFunctionalAccessCommand command)
    {
        return store.AddRoleAccessAsync(
            command.RoleId,
            command.OrganizationId,
            command.AccessClaimValue);
    }
}
