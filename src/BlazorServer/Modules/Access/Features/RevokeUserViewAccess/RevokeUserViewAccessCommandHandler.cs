using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Features.RevokeUserViewAccess;

internal sealed class RevokeUserViewAccessCommandHandler(
    IViewAccessGrantStore store,
    RevokeUserViewAccessCommandValidator? validator = null)
    : ICommandHandler<RevokeUserViewAccessCommand, RevokeUserViewAccessResult>
{
    private readonly RevokeUserViewAccessCommandValidator _validator = validator ?? new RevokeUserViewAccessCommandValidator();

    public async Task<RevokeUserViewAccessResult> HandleAsync(RevokeUserViewAccessCommand command, CancellationToken cancellationToken)
    {
        var errors = _validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidRevokeUserViewAccessRequest(errors);
        }

        await store.RevokeAsync(new ViewAccessGrant(
            command.ViewId,
            command.OrganizationId,
            ViewAccessGrantTargetType.User,
            command.UserId,
            command.Right));

        return new UserViewAccessRevoked(command.ViewId, command.OrganizationId, command.UserId, command.Right);
    }
}
