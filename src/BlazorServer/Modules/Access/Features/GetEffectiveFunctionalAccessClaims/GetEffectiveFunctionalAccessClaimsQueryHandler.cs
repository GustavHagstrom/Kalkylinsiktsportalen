using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.GetEffectiveFunctionalAccessClaims;

public sealed class GetEffectiveFunctionalAccessClaimsQueryHandler(
    IFunctionalAccessGrantStore store,
    GetEffectiveFunctionalAccessClaimsQueryValidator? validator = null)
    : IQueryHandler<GetEffectiveFunctionalAccessClaimsQuery, GetEffectiveFunctionalAccessClaimsResult>
{
    private readonly GetEffectiveFunctionalAccessClaimsQueryValidator _validator =
        validator ?? new GetEffectiveFunctionalAccessClaimsQueryValidator();

    public async Task<GetEffectiveFunctionalAccessClaimsResult> HandleAsync(
        GetEffectiveFunctionalAccessClaimsQuery query,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(query);
        if (errors.Count > 0)
        {
            return new InvalidGetEffectiveFunctionalAccessClaimsRequest(errors);
        }

        var accessClaims = await store.GetAccessClaimsForUserAsync(
            query.UserId,
            query.RoleIds,
            query.OrganizationId);

        return new EffectiveFunctionalAccessClaimsLoaded(
            query.UserId,
            query.OrganizationId,
            accessClaims);
    }
}
