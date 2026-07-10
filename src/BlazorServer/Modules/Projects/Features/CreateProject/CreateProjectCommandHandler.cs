using BlazorServer.Modules.Projects.Domain;
using BlazorServer.Modules.Projects.Infrastructure;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public sealed class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, CreateProjectResult>
{
    private readonly IProjectStore projectStore;
    private readonly CreateProjectCommandValidator validator;

    public CreateProjectCommandHandler(
        IProjectStore projectStore,
        CreateProjectCommandValidator? validator = null)
    {
        this.projectStore = projectStore;
        this.validator = validator ?? new CreateProjectCommandValidator();
    }

    public async Task<CreateProjectResult> HandleAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(command);

        if (errors.Count > 0)
        {
            return CreateProjectResult.Invalid([.. errors]);
        }

        var project = Project.Create(
            command.ProjectId,
            command.OrganizationId,
            command.CreatedBy,
            command.CreatedAt,
            command.Name);

        await projectStore.AddAsync(project, cancellationToken);

        return CreateProjectResult.Success(project.Id, project.OrganizationId, project.Name);
    }
}
