using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.GrantRoleFunctionalAccess;

internal sealed class GrantRoleFunctionalAccessCommandHandler(
    IFunctionalAccessGrantStore store,
    GrantRoleFunctionalAccessCommandValidator? validator = null)
    : ICommandHandler<GrantRoleFunctionalAccessCommand, GrantRoleFunctionalAccessResult>
{
    private readonly GrantRoleFunctionalAccessCommandValidator _validator = validator ?? new GrantRoleFunctionalAccessCommandValidator();

    public async Task<GrantRoleFunctionalAccessResult> HandleAsync(
        GrantRoleFunctionalAccessCommand command,
        CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidGrantRoleFunctionalAccessRequest(errors);
        }

        var target = FunctionalAccessGrantTarget.Role(command.RoleId);
        await store.AddAccessGrantAsync(
            target,
            command.OrganizationId,
            command.AccessClaimValue);

        var grant = await store.GetAccessGrantAsync(target, command.OrganizationId);

        return new RoleFunctionalAccessGranted(
            command.RoleId,
            command.OrganizationId,
            grant.AccessClaimValues);
    }
}
