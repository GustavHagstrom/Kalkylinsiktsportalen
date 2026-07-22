namespace BlazorServer.Modules.Bidcon.CredentialAdministration;

public interface IBidconCredentialsAuditLog
{
    Task WriteAsync(
        BidconCredentialsAuditEntry entry,
        CancellationToken cancellationToken);
}
