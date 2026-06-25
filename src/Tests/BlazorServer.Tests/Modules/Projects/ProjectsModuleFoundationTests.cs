using BlazorServer.Modules.Projects.Domain;
using BlazorServer.Modules.Projects.Features.CreateProject;
using BlazorServer.Modules.Projects.Infrastructure.InMemory;
using BlazorServer.Shared.Kernel.Identity;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Tests.Modules.Projects;

[TestFixture]
public class ProjectsModuleFoundationTests
{
    [Test]
    public async Task CreateProject_CreatesProjectThroughInMemoryCommandHandler()
    {
        var store = new InMemoryProjectStore();
        var commandHandler = new CreateProjectCommandHandler(store);
        var organizationId = new OrganizationId(Guid.Parse("1c928774-3e22-4a94-9146-8784fb54d67a"));
        var projectId = new ProjectId(Guid.Parse("0c67ee33-0924-4515-937f-8dd77efc1dc5"));
        var createdBy = new UserId(Guid.Parse("9178ba3d-461f-440e-9388-6deef941767b"));
        var createdAt = new DateTimeOffset(2026, 06, 25, 13, 30, 00, TimeSpan.Zero);

        var result = await commandHandler.HandleAsync(
            new CreateProjectCommand(organizationId, projectId, createdBy, createdAt, "Kvarteret Linden"),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.ProjectId, Is.EqualTo(projectId));
            Assert.That(result.Value.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(result.Value.Name, Is.EqualTo("Kvarteret Linden"));
        });

        var storedProject = await store.GetAsync(projectId, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(storedProject, Is.Not.Null);
            Assert.That(storedProject!.CreatedBy, Is.EqualTo(createdBy));
            Assert.That(storedProject.CreatedAt, Is.EqualTo(createdAt));
        });
    }

    [Test]
    public async Task CreateProject_ReturnsValidationError_WhenNameIsMissing()
    {
        var commandHandler = new CreateProjectCommandHandler(new InMemoryProjectStore());

        var result = await commandHandler.HandleAsync(
            new CreateProjectCommand(
                new OrganizationId(Guid.NewGuid()),
                new ProjectId(Guid.NewGuid()),
                new UserId(Guid.NewGuid()),
                DateTimeOffset.UtcNow,
                " "),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Errors, Has.One.Matches<ValidationError>(error =>
                error.Code == "project.name.required" &&
                error.Field == nameof(CreateProjectCommand.Name)));
        });
    }

    [Test]
    public void ProjectCreate_ThrowsDomainException_WhenOrganizationIdIsEmpty()
    {
        var exception = Assert.Throws<ProjectDomainException>(() =>
            Project.Create(
                new ProjectId(Guid.NewGuid()),
                new OrganizationId(Guid.Empty),
                new UserId(Guid.NewGuid()),
                DateTimeOffset.UtcNow,
                "Kvarteret Linden"));

        Assert.That(exception!.Code, Is.EqualTo("project.organization.required"));
    }
}
