using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.GetRoleFunctionalAccess;

internal sealed class GetRoleFunctionalAccessQueryHandler(
    IFunctionalAccessGrantStore store,
    GetRoleFunctionalAccessQueryValidator? validator = null)
    : IQueryHandler<GetRoleFunctionalAccessQuery, GetRoleFunctionalAccessResult>
{
    private readonly GetRoleFunctionalAccessQueryValidator _validator = validator ?? new GetRoleFunctionalAccessQueryValidator();

    public async Task<GetRoleFunctionalAccessResult> HandleAsync(
        GetRoleFunctionalAccessQuery query,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(query);
        if (errors.Count > 0)
        {
            return new InvalidGetRoleFunctionalAccessRequest(errors);
        }

        var grant = await store.GetAccessGrantAsync(
            FunctionalAccessGrantTarget.Role(query.RoleId),
            query.OrganizationId);

        return new RoleFunctionalAccessLoaded(
            query.RoleId,
            query.OrganizationId,
            grant.AccessClaimValues);
    }
}
