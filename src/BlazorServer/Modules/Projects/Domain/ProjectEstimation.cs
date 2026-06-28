namespace BlazorServer.Modules.Projects.Domain;

public sealed class ProjectEstimation
{
    private readonly List<EstimationVersion> estimationVersions;

    private ProjectEstimation(
        ProjectEstimationId id,
        ProjectId projectId,
        BidconEstimationId bidconEstimationId,
        EstimationVersionId activeEstimationVersionId,
        List<EstimationVersion> estimationVersions)
    {
        Id = id;
        ProjectId = projectId;
        BidconEstimationId = bidconEstimationId;
        ActiveEstimationVersionId = activeEstimationVersionId;
        this.estimationVersions = estimationVersions;
    }

    public ProjectEstimationId Id { get; }

    public ProjectId ProjectId { get; }

    public BidconEstimationId BidconEstimationId { get; }

    public EstimationVersionId ActiveEstimationVersionId { get; private set; }

    public bool IsArchived { get; private set; }

    public IReadOnlyList<EstimationVersion> EstimationVersions => estimationVersions;

    public static ProjectEstimation Create(
        ProjectEstimationId id,
        ProjectId projectId,
        BidconEstimationId bidconEstimationId,
        EstimationVersionId estimationVersionId,
        EstimationSnapshotId estimationSnapshotId,
        DateTimeOffset importedAt)
    {
        if (id.IsEmpty)
        {
            throw new ProjectDomainException("projectEstimation.id.required", "Project estimation id is required.");
        }

        if (projectId.IsEmpty)
        {
            throw new ProjectDomainException("projectEstimation.project.required", "Project id is required.");
        }

        if (bidconEstimationId.IsEmpty)
        {
            throw new ProjectDomainException("projectEstimation.bidconEstimation.required", "Bidcon estimation id is required.");
        }

        var initialVersion = EstimationVersion.Create(
            estimationVersionId,
            estimationSnapshotId,
            versionNumber: 1,
            importedAt);

        return new ProjectEstimation(
            id,
            projectId,
            bidconEstimationId,
            initialVersion.Id,
            [initialVersion]);
    }

    public EstimationVersion ImportNewVersion(
        EstimationVersionId estimationVersionId,
        EstimationSnapshotId estimationSnapshotId,
        DateTimeOffset importedAt)
    {
        EnsureNotArchived();

        var nextVersionNumber = estimationVersions.Count + 1;
        var version = EstimationVersion.Create(
            estimationVersionId,
            estimationSnapshotId,
            nextVersionNumber,
            importedAt);

        estimationVersions.Add(version);
        ActiveEstimationVersionId = version.Id;
        return version;
    }

    public void ActivateVersion(EstimationVersionId estimationVersionId)
    {
        EnsureNotArchived();

        if (estimationVersionId.IsEmpty)
        {
            throw new ProjectDomainException("projectEstimation.activeVersion.required", "Active estimation version id is required.");
        }

        if (estimationVersions.All(version => version.Id != estimationVersionId))
        {
            throw new ProjectDomainException(
                "projectEstimation.activeVersion.unknown",
                "The estimation version does not belong to this project estimation.");
        }

        ActiveEstimationVersionId = estimationVersionId;
    }

    public void Archive()
    {
        EnsureHasVersions();
        IsArchived = true;
    }

    public void Reactivate()
    {
        EnsureHasVersions();
        IsArchived = false;
    }

    private void EnsureNotArchived()
    {
        if (IsArchived)
        {
            throw new ProjectDomainException(
                "projectEstimation.archived",
                "Archived project estimations must be reactivated before they can change.");
        }
    }

    private void EnsureHasVersions()
    {
        if (estimationVersions.Count == 0)
        {
            throw new ProjectDomainException(
                "projectEstimation.versions.required",
                "Project estimation must contain at least one estimation version.");
        }
    }
}
