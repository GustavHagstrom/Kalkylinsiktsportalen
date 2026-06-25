using BlazorServer.Modules.Projects.Domain;
using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public sealed class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, CreateProjectResult>
{
    private readonly IProjectStore projectStore;
    private readonly IValidator<CreateProjectCommand> validator;

    public CreateProjectCommandHandler(
        IProjectStore projectStore,
        IValidator<CreateProjectCommand>? validator = null)
    {
        this.projectStore = projectStore;
        this.validator = validator ?? new CreateProjectCommandValidator();
    }

    public async Task<CommandResult<CreateProjectResult>> HandleAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return CommandResult<CreateProjectResult>.Invalid(validationResult.Errors.ToArray());
        }

        var project = Project.Create(
            command.ProjectId,
            command.OrganizationId,
            command.CreatedBy,
            command.CreatedAt,
            command.Name);

        await projectStore.AddAsync(project, cancellationToken);

        return CommandResult<CreateProjectResult>.Success(
            new CreateProjectResult(project.Id, project.OrganizationId, project.Name));
    }
}
