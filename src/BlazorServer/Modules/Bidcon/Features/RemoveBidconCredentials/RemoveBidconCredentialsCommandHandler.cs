using BlazorServer.Modules.Bidcon.CredentialAdministration;
using BlazorServer.Modules.Bidcon.AccessTokens;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed class RemoveBidconCredentialsCommandHandler
    : ICommandHandler<RemoveBidconCredentialsCommand, RemoveBidconCredentialsResult>
{
    private readonly IBidconApiCredentialsStore credentialsStore;
    private readonly IBidconAccessTokenCache accessTokenCache;
    private readonly IBidconCredentialsAuditLog auditLog;
    private readonly RemoveBidconCredentialsCommandValidator validator;

    public RemoveBidconCredentialsCommandHandler(
        IBidconApiCredentialsStore credentialsStore,
        IBidconAccessTokenCache accessTokenCache,
        IBidconCredentialsAuditLog auditLog,
        RemoveBidconCredentialsCommandValidator? validator = null)
    {
        this.credentialsStore = credentialsStore;
        this.accessTokenCache = accessTokenCache;
        this.auditLog = auditLog;
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

        await credentialsStore.DeleteAsync(command.OrganizationId);
        accessTokenCache.Invalidate(command.OrganizationId);

        await auditLog.WriteAsync(
            new BidconCredentialsAuditEntry(
                command.OrganizationId,
                command.ActorId,
                DateTimeOffset.UtcNow,
                BidconCredentialsAuditAction.Remove,
                BidconCredentialsAuditResult.Success,
                "Bidcon credentials were removed."),
            cancellationToken);

        return new BidconCredentialsRemoved(command.OrganizationId);
    }
}
