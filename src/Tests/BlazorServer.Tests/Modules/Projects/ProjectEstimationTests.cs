using BlazorServer.Modules.Projects.Domain;

namespace BlazorServer.Tests.Modules.Projects;

[TestFixture]
public class ProjectEstimationTests
{
    [Test]
    public void Create_SetsInitialVersionAsActive()
    {
        var projectEstimationId = new ProjectEstimationId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var projectId = new ProjectId(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        var bidconEstimationId = new BidconEstimationId("EST-100");
        var estimationVersionId = new EstimationVersionId(Guid.Parse("33333333-3333-3333-3333-333333333333"));
        var estimationSnapshotId = new EstimationSnapshotId(Guid.Parse("44444444-4444-4444-4444-444444444444"));
        var importedAt = new DateTimeOffset(2026, 06, 28, 12, 00, 00, TimeSpan.Zero);

        var projectEstimation = ProjectEstimation.Create(
            projectEstimationId,
            projectId,
            bidconEstimationId,
            estimationVersionId,
            estimationSnapshotId,
            importedAt);

        Assert.Multiple(() =>
        {
            Assert.That(projectEstimation.Id, Is.EqualTo(projectEstimationId));
            Assert.That(projectEstimation.ProjectId, Is.EqualTo(projectId));
            Assert.That(projectEstimation.BidconEstimationId, Is.EqualTo(bidconEstimationId));
            Assert.That(projectEstimation.ActiveEstimationVersionId, Is.EqualTo(estimationVersionId));
            Assert.That(projectEstimation.IsArchived, Is.False);
            Assert.That(projectEstimation.EstimationVersions, Has.Count.EqualTo(1));
            Assert.That(projectEstimation.EstimationVersions[0].VersionNumber, Is.EqualTo(1));
            Assert.That(projectEstimation.EstimationVersions[0].EstimationSnapshotId, Is.EqualTo(estimationSnapshotId));
        });
    }

    [Test]
    public void ImportNewVersion_AddsVersionAndActivatesIt()
    {
        var projectEstimation = CreateProjectEstimation();
        var newVersionId = new EstimationVersionId(Guid.Parse("55555555-5555-5555-5555-555555555555"));
        var newSnapshotId = new EstimationSnapshotId(Guid.Parse("66666666-6666-6666-6666-666666666666"));
        var importedAt = new DateTimeOffset(2026, 06, 29, 08, 30, 00, TimeSpan.Zero);

        var version = projectEstimation.ImportNewVersion(newVersionId, newSnapshotId, importedAt);

        Assert.Multiple(() =>
        {
            Assert.That(version.Id, Is.EqualTo(newVersionId));
            Assert.That(version.VersionNumber, Is.EqualTo(2));
            Assert.That(projectEstimation.ActiveEstimationVersionId, Is.EqualTo(newVersionId));
            Assert.That(projectEstimation.EstimationVersions, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void ActivateVersion_ChangesActiveVersionToExistingOlderVersion()
    {
        var projectEstimation = CreateProjectEstimation();
        var newerVersionId = new EstimationVersionId(Guid.Parse("55555555-5555-5555-5555-555555555555"));

        projectEstimation.ImportNewVersion(
            newerVersionId,
            new EstimationSnapshotId(Guid.Parse("66666666-6666-6666-6666-666666666666")),
            new DateTimeOffset(2026, 06, 29, 08, 30, 00, TimeSpan.Zero));

        var originalVersionId = projectEstimation.EstimationVersions[0].Id;

        projectEstimation.ActivateVersion(originalVersionId);

        Assert.That(projectEstimation.ActiveEstimationVersionId, Is.EqualTo(originalVersionId));
    }

    [Test]
    public void ActivateVersion_ThrowsDomainException_WhenVersionDoesNotBelongToProjectEstimation()
    {
        var projectEstimation = CreateProjectEstimation();

        var exception = Assert.Throws<ProjectDomainException>(() =>
            projectEstimation.ActivateVersion(new EstimationVersionId(Guid.NewGuid())));

        Assert.That(exception!.Code, Is.EqualTo("projectEstimation.activeVersion.unknown"));
    }

    [Test]
    public void ImportNewVersion_ThrowsDomainException_WhenProjectEstimationIsArchived()
    {
        var projectEstimation = CreateProjectEstimation();
        projectEstimation.Archive();

        var exception = Assert.Throws<ProjectDomainException>(() =>
            projectEstimation.ImportNewVersion(
                new EstimationVersionId(Guid.NewGuid()),
                new EstimationSnapshotId(Guid.NewGuid()),
                DateTimeOffset.UtcNow));

        Assert.That(exception!.Code, Is.EqualTo("projectEstimation.archived"));
    }

    [Test]
    public void Reactivate_AllowsImportingNewVersionAgain()
    {
        var projectEstimation = CreateProjectEstimation();

        projectEstimation.Archive();
        projectEstimation.Reactivate();

        var version = projectEstimation.ImportNewVersion(
            new EstimationVersionId(Guid.Parse("55555555-5555-5555-5555-555555555555")),
            new EstimationSnapshotId(Guid.Parse("66666666-6666-6666-6666-666666666666")),
            new DateTimeOffset(2026, 06, 29, 08, 30, 00, TimeSpan.Zero));

        Assert.Multiple(() =>
        {
            Assert.That(projectEstimation.IsArchived, Is.False);
            Assert.That(projectEstimation.ActiveEstimationVersionId, Is.EqualTo(version.Id));
            Assert.That(projectEstimation.EstimationVersions, Has.Count.EqualTo(2));
        });
    }

    private static ProjectEstimation CreateProjectEstimation()
    {
        return ProjectEstimation.Create(
            new ProjectEstimationId(Guid.Parse("11111111-1111-1111-1111-111111111111")),
            new ProjectId(Guid.Parse("22222222-2222-2222-2222-222222222222")),
            new BidconEstimationId("EST-100"),
            new EstimationVersionId(Guid.Parse("33333333-3333-3333-3333-333333333333")),
            new EstimationSnapshotId(Guid.Parse("44444444-4444-4444-4444-444444444444")),
            new DateTimeOffset(2026, 06, 28, 12, 00, 00, TimeSpan.Zero));
    }
}
