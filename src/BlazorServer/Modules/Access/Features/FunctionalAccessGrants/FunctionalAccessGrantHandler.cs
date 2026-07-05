using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Access.Domain;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed class FunctionalAccessGrantHandler(IFunctionalAccessGrantStore store)
{
    public Task HandleAsync(GrantUserFunctionalAccessCommand command)
    {
        return store.AddAccessGrantAsync(
            FunctionalAccessGrantTarget.User(command.UserId),
            command.OrganizationId,
            command.AccessClaimValue);
    }

    public Task HandleAsync(GrantRoleFunctionalAccessCommand command)
    {
        return store.AddAccessGrantAsync(
            FunctionalAccessGrantTarget.Role(command.RoleId),
            command.OrganizationId,
            command.AccessClaimValue);
    }

    public Task HandleAsync(RevokeUserFunctionalAccessCommand command)
    {
        return store.DeleteAccessGrantAsync(
            FunctionalAccessGrantTarget.User(command.UserId),
            command.OrganizationId,
            command.AccessClaimValue);
    }

    public Task HandleAsync(RevokeRoleFunctionalAccessCommand command)
    {
        return store.DeleteAccessGrantAsync(
            FunctionalAccessGrantTarget.Role(command.RoleId),
            command.OrganizationId,
            command.AccessClaimValue);
    }

    public Task<FunctionalAccessGrant> HandleAsync(GetUserFunctionalAccessQuery query)
    {
        return store.GetAccessGrantAsync(
            FunctionalAccessGrantTarget.User(query.UserId),
            query.OrganizationId);
    }

    public Task<FunctionalAccessGrant> HandleAsync(GetRoleFunctionalAccessQuery query)
    {
        return store.GetAccessGrantAsync(
            FunctionalAccessGrantTarget.Role(query.RoleId),
            query.OrganizationId);
    }

    public async Task<IReadOnlyList<AccessClaim>> HandleAsync(GetEffectiveFunctionalAccessClaimsQuery query)
    {
        return await store.GetAccessClaimsForUserAsync(
            query.UserId,
            query.RoleIds,
            query.OrganizationId);
    }
}
