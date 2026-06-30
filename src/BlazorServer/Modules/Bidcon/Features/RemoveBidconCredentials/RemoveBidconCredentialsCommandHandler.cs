using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed class RemoveBidconCredentialsCommandHandler
    : ICommandHandler<RemoveBidconCredentialsCommand, RemoveBidconCredentialsResult>
{
    private readonly IBidconApiCredentialsStore credentialsStore;
    private readonly IBidconAccessTokenCache accessTokenCache;
    private readonly IValidator<RemoveBidconCredentialsCommand> validator;

    public RemoveBidconCredentialsCommandHandler(
        IBidconApiCredentialsStore credentialsStore,
        IBidconAccessTokenCache accessTokenCache,
        IValidator<RemoveBidconCredentialsCommand>? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.accessTokenCache = accessTokenCache;
        this.validator = validator ?? new RemoveBidconCredentialsCommandValidator();
    }

    public async Task<CommandResult<RemoveBidconCredentialsResult>> HandleAsync(
        RemoveBidconCredentialsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return CommandResult<RemoveBidconCredentialsResult>.Invalid(validationResult.Errors.ToArray());
        }

        await credentialsStore.DeleteAsync(command.OrganizationId.Value.ToString());
        accessTokenCache.Invalidate(command.OrganizationId);

        return CommandResult<RemoveBidconCredentialsResult>.Success(
            new RemoveBidconCredentialsResult(command.OrganizationId, false));
    }
}
