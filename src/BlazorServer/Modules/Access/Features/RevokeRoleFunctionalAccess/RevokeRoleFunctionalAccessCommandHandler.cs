using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.RevokeRoleFunctionalAccess;

internal sealed class RevokeRoleFunctionalAccessCommandHandler(
    IFunctionalAccessGrantStore store,
    RevokeRoleFunctionalAccessCommandValidator? validator = null)
    : ICommandHandler<RevokeRoleFunctionalAccessCommand, RevokeRoleFunctionalAccessResult>
{
    private readonly RevokeRoleFunctionalAccessCommandValidator _validator = validator ?? new RevokeRoleFunctionalAccessCommandValidator();

    public async Task<RevokeRoleFunctionalAccessResult> HandleAsync(
        RevokeRoleFunctionalAccessCommand command,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidRevokeRoleFunctionalAccessRequest(errors);
        }

        var target = FunctionalAccessGrantTarget.Role(command.RoleId);
        await store.DeleteAccessGrantAsync(
            target,
            command.OrganizationId,
            command.AccessClaimValue);

        var grant = await store.GetAccessGrantAsync(target, command.OrganizationId);

        return new RoleFunctionalAccessRevoked(
            command.RoleId,
            command.OrganizationId,
            grant.AccessClaimValues);
    }
}
