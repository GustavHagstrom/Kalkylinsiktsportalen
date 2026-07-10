using BlazorServer.Modules.Projects.Domain;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public sealed record CreateProjectResult(
    ProjectId? ProjectId,
    OrganizationId? OrganizationId,
    string? Name,
    IReadOnlyList<ValidationError> Errors) : IValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public bool IsSuccess => IsValid;

    public static CreateProjectResult Success(ProjectId projectId, OrganizationId organizationId, string name)
        => new(projectId, organizationId, name, Array.Empty<ValidationError>());

    public static CreateProjectResult Invalid(params ValidationError[] errors)
        => new(null, null, null, errors);
}
