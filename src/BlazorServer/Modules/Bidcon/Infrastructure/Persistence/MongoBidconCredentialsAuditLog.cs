using BlazorServer.Modules.Bidcon.CredentialAdministration;
using MongoDB.Driver;

namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

public sealed class MongoBidconCredentialsAuditLog(
    IMongoCollection<MongoBidconCredentialsAuditEntry> collection) : IBidconCredentialsAuditLog
{
    public Task WriteAsync(
        BidconCredentialsAuditEntry entry,
        CancellationToken cancellationToken)
    {
        return collection.InsertOneAsync(
            new MongoBidconCredentialsAuditEntry
            {
                OrganizationId = entry.OrganizationId.Value,
                ActorId = entry.ActorId.Value,
                OccurredAt = entry.OccurredAt,
                Action = entry.Action.ToString(),
                Result = entry.Result.ToString(),
                Details = entry.Details
            },
            cancellationToken: cancellationToken);
    }
}
