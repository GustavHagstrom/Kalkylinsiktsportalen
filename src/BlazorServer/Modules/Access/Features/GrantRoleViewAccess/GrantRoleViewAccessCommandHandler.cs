using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.GrantRoleViewAccess;

internal sealed class GrantRoleViewAccessCommandHandler(
    IViewAccessGrantStore store,
    GrantRoleViewAccessCommandValidator? validator = null)
    : ICommandHandler<GrantRoleViewAccessCommand, GrantRoleViewAccessResult>
{
    private readonly GrantRoleViewAccessCommandValidator _validator = validator ?? new GrantRoleViewAccessCommandValidator();

    public async Task<GrantRoleViewAccessResult> HandleAsync(GrantRoleViewAccessCommand command, CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidGrantRoleViewAccessRequest(errors);
        }

        await store.GrantAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.Role,
            command.RoleId,
            command.Right));

        return new RoleViewAccessGranted(command.ViewId, command.OrganizationId, command.RoleId, command.Right);
    }
}
