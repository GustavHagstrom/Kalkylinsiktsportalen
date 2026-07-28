using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.GrantUserViewAccess;

internal sealed class GrantUserViewAccessCommandHandler(
    IViewAccessGrantStore store,
    GrantUserViewAccessCommandValidator? validator = null)
    : ICommandHandler<GrantUserViewAccessCommand, GrantUserViewAccessResult>
{
    private readonly GrantUserViewAccessCommandValidator _validator = validator ?? new GrantUserViewAccessCommandValidator();

    public async Task<GrantUserViewAccessResult> HandleAsync(GrantUserViewAccessCommand command, CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidGrantUserViewAccessRequest(errors);
        }

        await store.GrantAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.User,
            command.UserId,
            command.Right));

        return new UserViewAccessGranted(command.ViewId, command.OrganizationId, command.UserId, command.Right);
    }
}
