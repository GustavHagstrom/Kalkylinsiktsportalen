using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.RevokeUserFunctionalAccess;

internal sealed class RevokeUserFunctionalAccessCommandHandler(
    IFunctionalAccessGrantStore store,
    RevokeUserFunctionalAccessCommandValidator? validator = null)
    : ICommandHandler<RevokeUserFunctionalAccessCommand, RevokeUserFunctionalAccessResult>
{
    private readonly RevokeUserFunctionalAccessCommandValidator _validator = validator ?? new RevokeUserFunctionalAccessCommandValidator();

    public async Task<RevokeUserFunctionalAccessResult> HandleAsync(
        RevokeUserFunctionalAccessCommand command,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidRevokeUserFunctionalAccessRequest(errors);
        }

        var target = FunctionalAccessGrantTarget.User(command.UserId);
        await store.DeleteAccessGrantAsync(
            target,
            command.OrganizationId,
            command.AccessClaimValue);

        var grant = await store.GetAccessGrantAsync(target, command.OrganizationId);

        return new UserFunctionalAccessRevoked(
            command.UserId,
            command.OrganizationId,
            grant.AccessClaimValues);
    }
}
