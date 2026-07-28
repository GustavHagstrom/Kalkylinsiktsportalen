using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.GetUserFunctionalAccess;

internal sealed class GetUserFunctionalAccessQueryHandler(
    IFunctionalAccessGrantStore store,
    GetUserFunctionalAccessQueryValidator? validator = null)
    : IQueryHandler<GetUserFunctionalAccessQuery, GetUserFunctionalAccessResult>
{
    private readonly GetUserFunctionalAccessQueryValidator _validator = validator ?? new GetUserFunctionalAccessQueryValidator();

    public async Task<GetUserFunctionalAccessResult> HandleAsync(
        GetUserFunctionalAccessQuery query,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(query);
        if (errors.Count > 0)
        {
            return new InvalidGetUserFunctionalAccessRequest(errors);
        }

        var grant = await store.GetAccessGrantAsync(
            FunctionalAccessGrantTarget.User(query.UserId),
            query.OrganizationId);

        return new UserFunctionalAccessLoaded(
            query.UserId,
            query.OrganizationId,
            grant.AccessClaimValues);
    }
}
