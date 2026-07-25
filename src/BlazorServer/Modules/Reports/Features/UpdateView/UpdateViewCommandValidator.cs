using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Reports.Features.UpdateView;

public sealed class UpdateViewCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(UpdateViewCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return
            [
                new ValidationError(
                    "view.name.required",
                    nameof(UpdateViewCommand.Name),
                    "View name is required.")
            ];
        }

        return Array.Empty<ValidationError>();
    }
}
