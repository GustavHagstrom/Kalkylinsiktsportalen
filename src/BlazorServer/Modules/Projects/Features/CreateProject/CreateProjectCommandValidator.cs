using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public sealed class CreateProjectCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(CreateProjectCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return
            [
                new ValidationError(
                    "project.name.required",
                    nameof(CreateProjectCommand.Name),
                    "Project name is required.")
            ];
        }

        return Array.Empty<ValidationError>();
    }
}
