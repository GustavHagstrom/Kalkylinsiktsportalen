using BlazorServer.Modules.Bidcon.CredentialAdministration;
using BlazorServer.Modules.Bidcon.AccessTokens;
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
    private readonly IBidconCredentialsAuditLog auditLog;
    private readonly UpsertBidconCredentialsCommandValidator validator;

    public UpsertBidconCredentialsCommandHandler(
        IBidconApiCredentialsStore credentialsStore,
        IBidconCredentialsVerifier credentialsVerifier,
        IBidconAccessTokenCache accessTokenCache,
        IBidconCredentialsAuditLog auditLog,
        UpsertBidconCredentialsCommandValidator? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.credentialsVerifier = credentialsVerifier;
        this.accessTokenCache = accessTokenCache;
        this.auditLog = auditLog;
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

        var existingCredentials = await credentialsStore.GetAsync(command.OrganizationId);
        var credentials = new BidconApiCredentials
        {
            OrganizationId = command.OrganizationId,
            ApiKey = command.ApiKey,
            ApiSecret = command.ApiSecret
        };

        var isVerified = await credentialsVerifier.VerifyAsync(credentials, cancellationToken);

        if (!isVerified)
        {
            await WriteAuditEntryAsync(
                command,
                BidconCredentialsAuditAction.VerificationFailed,
                BidconCredentialsAuditResult.Failure,
                "Bidcon credentials verification failed.",
                cancellationToken);

            return new BidconCredentialsVerificationFailed(
                command.OrganizationId,
                "Bidcon credentials could not be verified.");
        }

        await credentialsStore.UpsertAsync(credentials);
        accessTokenCache.Invalidate(command.OrganizationId);

        await WriteAuditEntryAsync(
            command,
            existingCredentials is null
                ? BidconCredentialsAuditAction.Create
                : BidconCredentialsAuditAction.Replace,
            BidconCredentialsAuditResult.Success,
            existingCredentials is null
                ? "Bidcon credentials were created."
                : "Bidcon credentials were replaced.",
            cancellationToken);

        return new BidconCredentialsSaved(command.OrganizationId);
    }

    private Task WriteAuditEntryAsync(
        UpsertBidconCredentialsCommand command,
        BidconCredentialsAuditAction action,
        BidconCredentialsAuditResult result,
        string details,
        CancellationToken cancellationToken)
    {
        return auditLog.WriteAsync(
            new BidconCredentialsAuditEntry(
                command.OrganizationId,
                command.ActorId,
                DateTimeOffset.UtcNow,
                action,
                result,
                details),
            cancellationToken);
    }
}
