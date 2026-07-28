using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Domain;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

internal sealed class GrantUserFunctionalAccessCommandHandler(
    IFunctionalAccessGrantStore store,
    GrantUserFunctionalAccessCommandValidator? validator = null)
    : ICommandHandler<GrantUserFunctionalAccessCommand, GrantUserFunctionalAccessResult>
{
    private readonly GrantUserFunctionalAccessCommandValidator _validator = validator ?? new GrantUserFunctionalAccessCommandValidator();

    public async Task<GrantUserFunctionalAccessResult> HandleAsync(
        GrantUserFunctionalAccessCommand command,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidGrantUserFunctionalAccessRequest(errors);
        }

        var target = FunctionalAccessGrantTarget.User(command.UserId);
        await store.AddAccessGrantAsync(
            target,
            command.OrganizationId,
            command.AccessClaimValue);

        var grant = await store.GetAccessGrantAsync(target, command.OrganizationId);

        return new UserFunctionalAccessGranted(
            command.UserId,
            command.OrganizationId,
            grant.AccessClaimValues);
    }
}
