using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Projects.Features.CreateProject;

public sealed class CreateProjectCommandValidator : IValidator<CreateProjectCommand>
{
    public ValidationResult Validate(CreateProjectCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return ValidationResult.Failure(
                new ValidationError(
                    "project.name.required",
                    nameof(CreateProjectCommand.Name),
                    "Project name is required."));
        }

        return ValidationResult.Success();
    }
}
