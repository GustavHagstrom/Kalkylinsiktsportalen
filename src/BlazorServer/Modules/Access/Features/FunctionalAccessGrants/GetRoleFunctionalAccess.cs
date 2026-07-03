using BlazorServer.Modules.Access.Models;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record GetRoleFunctionalAccessQuery(string RoleId, string OrganizationId);

public sealed class GetRoleFunctionalAccessHandler(IFunctionalAccessGrantStore store)
{
    public Task<RoleAccess> HandleAsync(GetRoleFunctionalAccessQuery query)
    {
        return store.GetRoleAccessAsync(query.RoleId, query.OrganizationId);
    }
}
