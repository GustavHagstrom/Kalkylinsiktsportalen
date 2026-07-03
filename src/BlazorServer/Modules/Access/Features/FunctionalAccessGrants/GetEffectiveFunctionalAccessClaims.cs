using BlazorServer.Modules.Access.Models;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record GetEffectiveFunctionalAccessClaimsQuery(
    string UserId,
    IReadOnlyCollection<string> RoleIds,
    string OrganizationId);

public sealed class GetEffectiveFunctionalAccessClaimsHandler(IFunctionalAccessGrantStore store)
{
    public async Task<IReadOnlyList<AccessClaim>> HandleAsync(GetEffectiveFunctionalAccessClaimsQuery query)
    {
        return await store.GetAccessClaimsForUserAsync(
            query.UserId,
            query.RoleIds,
            query.OrganizationId);
    }
}
