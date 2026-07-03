using BlazorServer.Modules.Access.Domain;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record GetUserFunctionalAccessQuery(string UserId, string OrganizationId);

public sealed class GetUserFunctionalAccessHandler(IFunctionalAccessGrantStore store)
{
    public Task<UserAccess> HandleAsync(GetUserFunctionalAccessQuery query)
    {
        return store.GetUserAccessAsync(query.UserId, query.OrganizationId);
    }
}
