using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed class UpsertBidconCredentialsCommandHandler
    : ICommandHandler<UpsertBidconCredentialsCommand, UpsertBidconCredentialsResult>
{
    private readonly IBidconApiCredentialsStore credentialsStore;
    private readonly IBidconCredentialsVerifier credentialsVerifier;
    private readonly IBidconAccessTokenCache accessTokenCache;
    private readonly UpsertBidconCredentialsCommandValidator validator;

    public UpsertBidconCredentialsCommandHandler(
        IBidconApiCredentialsStore credentialsStore,
        IBidconCredentialsVerifier credentialsVerifier,
        IBidconAccessTokenCache accessTokenCache,
        UpsertBidconCredentialsCommandValidator? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.credentialsVerifier = credentialsVerifier;
        this.accessTokenCache = accessTokenCache;
        this.validator = validator ?? new UpsertBidconCredentialsCommandValidator();
    }

    public async Task<UpsertBidconCredentialsResult> HandleAsync(
        UpsertBidconCredentialsCommand command,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(command);
        if (errors.Count > 0)
        {
            return new InvalidUpsertBidconCredentialsRequest(errors);
        }

        var credentials = new BidconApiCredentials
        {
            OrganizationId = command.OrganizationId.Value.ToString(),
            ApiKey = command.ApiKey,
            ApiSecret = command.ApiSecret
        };

        var isVerified = await credentialsVerifier.VerifyAsync(credentials, cancellationToken);

        if (!isVerified)
        {
            return new BidconCredentialsVerificationFailed(
                command.OrganizationId,
                "Bidcon credentials could not be verified.");
        }

        await credentialsStore.UpsertAsync(credentials);
        accessTokenCache.Invalidate(command.OrganizationId);

        return new BidconCredentialsSaved(command.OrganizationId);
    }
}
