using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed class UpsertBidconCredentialsCommandHandler
    : ICommandHandler<UpsertBidconCredentialsCommand, UpsertBidconCredentialsResult>
{
    private readonly IBidconApiCredentialsStore credentialsStore;
    private readonly IBidconCredentialsVerifier credentialsVerifier;
    private readonly IBidconAccessTokenCache accessTokenCache;
    private readonly IValidator<UpsertBidconCredentialsCommand> validator;

    public UpsertBidconCredentialsCommandHandler(
        IBidconApiCredentialsStore credentialsStore,
        IBidconCredentialsVerifier credentialsVerifier,
        IBidconAccessTokenCache accessTokenCache,
        IValidator<UpsertBidconCredentialsCommand>? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.credentialsVerifier = credentialsVerifier;
        this.accessTokenCache = accessTokenCache;
        this.validator = validator ?? new UpsertBidconCredentialsCommandValidator();
    }

    public async Task<CommandResult<UpsertBidconCredentialsResult>> HandleAsync(
        UpsertBidconCredentialsCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return CommandResult<UpsertBidconCredentialsResult>.Invalid(validationResult.Errors.ToArray());
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
            return CommandResult<UpsertBidconCredentialsResult>.Invalid(
                new ValidationError(
                    "bidcon.credentials.verification.failed",
                    nameof(UpsertBidconCredentialsCommand.ApiKey),
                    "Bidcon credentials could not be verified."));
        }

        await credentialsStore.UpsertAsync(credentials);
        accessTokenCache.Invalidate(command.OrganizationId);

        return CommandResult<UpsertBidconCredentialsResult>.Success(
            new UpsertBidconCredentialsResult(command.OrganizationId, true));
    }
}
