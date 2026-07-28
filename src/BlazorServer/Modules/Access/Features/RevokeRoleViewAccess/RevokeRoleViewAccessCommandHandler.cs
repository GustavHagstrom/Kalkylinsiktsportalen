using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.RevokeRoleViewAccess;

internal sealed class RevokeRoleViewAccessCommandHandler(
    IViewAccessGrantStore store,
    RevokeRoleViewAccessCommandValidator? validator = null)
    : ICommandHandler<RevokeRoleViewAccessCommand, RevokeRoleViewAccessResult>
{
    private readonly RevokeRoleViewAccessCommandValidator _validator = validator ?? new RevokeRoleViewAccessCommandValidator();

    public async Task<RevokeRoleViewAccessResult> HandleAsync(RevokeRoleViewAccessCommand command, CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidRevokeRoleViewAccessRequest(errors);
        }

        await store.RevokeAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.Role,
            command.RoleId,
            command.Right));

        return new RoleViewAccessRevoked(command.ViewId, command.OrganizationId, command.RoleId, command.Right);
    }
}
