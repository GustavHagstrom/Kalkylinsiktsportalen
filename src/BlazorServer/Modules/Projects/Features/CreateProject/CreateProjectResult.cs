using BlazorServer.Modules.Projects.Domain;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public abstract record CreateProjectResult;

public sealed record ProjectCreated(
    ProjectId ProjectId,
    OrganizationId OrganizationId,
    string Name) : CreateProjectResult
{
    public bool WasCreated => true;
}

public sealed record InvalidCreateProjectRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : CreateProjectResult;
