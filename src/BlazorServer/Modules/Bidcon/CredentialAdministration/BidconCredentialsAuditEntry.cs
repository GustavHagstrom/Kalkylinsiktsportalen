using BlazorServer.Identity;

namespace BlazorServer.Modules.Bidcon.CredentialAdministration;

public sealed record BidconCredentialsAuditEntry(
    OrganizationId OrganizationId,
    UserId ActorId,
    DateTimeOffset OccurredAt,
    BidconCredentialsAuditAction Action,
    BidconCredentialsAuditResult Result,
    string Details);

public enum BidconCredentialsAuditAction
{
    Create,
    Replace,
    Remove,
    VerificationFailed
}

public enum BidconCredentialsAuditResult
{
    Success,
    Failure
}
