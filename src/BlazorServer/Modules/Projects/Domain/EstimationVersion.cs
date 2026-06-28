namespace BlazorServer.Modules.Projects.Domain;

public sealed class EstimationVersion
{
    private EstimationVersion(
        EstimationVersionId id,
        EstimationSnapshotId estimationSnapshotId,
        int versionNumber,
        DateTimeOffset importedAt)
    {
        Id = id;
        EstimationSnapshotId = estimationSnapshotId;
        VersionNumber = versionNumber;
        ImportedAt = importedAt;
    }

    public EstimationVersionId Id { get; }

    public EstimationSnapshotId EstimationSnapshotId { get; }

    public int VersionNumber { get; }

    public DateTimeOffset ImportedAt { get; }

    internal static EstimationVersion Create(
        EstimationVersionId id,
        EstimationSnapshotId estimationSnapshotId,
        int versionNumber,
        DateTimeOffset importedAt)
    {
        if (id.IsEmpty)
        {
            throw new ProjectDomainException("estimationVersion.id.required", "Estimation version id is required.");
        }

        if (estimationSnapshotId.IsEmpty)
        {
            throw new ProjectDomainException("estimationVersion.snapshot.required", "Estimation snapshot id is required.");
        }

        if (versionNumber <= 0)
        {
            throw new ProjectDomainException("estimationVersion.number.invalid", "Estimation version number must be greater than zero.");
        }

        return new EstimationVersion(id, estimationSnapshotId, versionNumber, importedAt);
    }
}
