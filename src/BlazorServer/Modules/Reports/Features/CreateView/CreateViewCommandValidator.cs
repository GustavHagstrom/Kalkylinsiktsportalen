using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Reports.Features.CreateView;

public sealed class CreateViewCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(CreateViewCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return
            [
                new ValidationError(
                    "view.name.required",
                    nameof(CreateViewCommand.Name),
                    "View name is required.")
            ];
        }

        return Array.Empty<ValidationError>();
    }
}
