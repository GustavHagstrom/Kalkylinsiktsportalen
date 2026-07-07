using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Domain;

public sealed class EstimationSnapshot
{
    private EstimationSnapshot(
        EstimationSnapshotId id,
        OrganizationId organizationId,
        BidconEstimationId bidconEstimationId,
        SnapshotSchemaVersion schemaVersion,
        DateTime sourceEditDate,
        DateTimeOffset importedAt,
        EstimationSnapshotPayloadV1 payload)
    {
        Id = id;
        OrganizationId = organizationId;
        BidconEstimationId = bidconEstimationId;
        SchemaVersion = schemaVersion;
        SourceEditDate = sourceEditDate;
        ImportedAt = importedAt;
        Payload = payload;
    }

    public EstimationSnapshotId Id { get; }

    public OrganizationId OrganizationId { get; }

    public BidconEstimationId BidconEstimationId { get; }

    public SnapshotSchemaVersion SchemaVersion { get; }

    public DateTime SourceEditDate { get; }

    public DateTimeOffset ImportedAt { get; }

    public EstimationSnapshotPayloadV1 Payload { get; }

    public static EstimationSnapshot Create(
        EstimationSnapshotId id,
        OrganizationId organizationId,
        BidconEstimationId bidconEstimationId,
        DateTime sourceEditDate,
        DateTimeOffset importedAt,
        EstimationSnapshotPayloadV1 payload)
    {
        if (id.IsEmpty)
        {
            throw new BidconDomainException("estimationSnapshot.id.required", "Estimation snapshot id is required.");
        }

        if (organizationId.IsEmpty)
        {
            throw new BidconDomainException("estimationSnapshot.organization.required", "Organization id is required.");
        }

        if (bidconEstimationId.IsEmpty)
        {
            throw new BidconDomainException("estimationSnapshot.bidconEstimation.required", "Bidcon estimation id is required.");
        }

        if (sourceEditDate == default)
        {
            throw new BidconDomainException("estimationSnapshot.sourceEditDate.required", "Source edit date is required.");
        }

        ArgumentNullException.ThrowIfNull(payload);

        return new EstimationSnapshot(
            id,
            organizationId,
            bidconEstimationId,
            SnapshotSchemaVersion.V1,
            sourceEditDate,
            importedAt,
            payload);
    }
}

public enum SnapshotSchemaVersion
{
    V1 = 1
}

public sealed class BidconDomainException : Exception
{
    public BidconDomainException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
