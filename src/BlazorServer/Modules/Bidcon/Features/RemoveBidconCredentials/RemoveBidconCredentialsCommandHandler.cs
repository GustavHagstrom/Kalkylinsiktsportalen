using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed class RemoveBidconCredentialsCommandHandler
    : ICommandHandler<RemoveBidconCredentialsCommand, RemoveBidconCredentialsResult>
{
    private readonly IBidconApiCredentialsStore credentialsStore;
    private readonly IBidconAccessTokenCache accessTokenCache;
    private readonly RemoveBidconCredentialsCommandValidator validator;

    public RemoveBidconCredentialsCommandHandler(
        IBidconApiCredentialsStore credentialsStore,
        IBidconAccessTokenCache accessTokenCache,
        RemoveBidconCredentialsCommandValidator? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.accessTokenCache = accessTokenCache;
        this.validator = validator ?? new RemoveBidconCredentialsCommandValidator();
    }

    public async Task<RemoveBidconCredentialsResult> HandleAsync(
        RemoveBidconCredentialsCommand command,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidRemoveBidconCredentialsRequest(errors);
        }

        await credentialsStore.DeleteAsync(command.OrganizationId.Value.ToString());
        accessTokenCache.Invalidate(command.OrganizationId);

        return new BidconCredentialsRemoved(command.OrganizationId);
    }
}
