using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed class RemoveBidconCredentialsCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(RemoveBidconCredentialsCommand command)
    {
        if (command.OrganizationId.IsEmpty)
        {
            return
            [
                new ValidationError(
                    "bidcon.credentials.organization.required",
                    nameof(RemoveBidconCredentialsCommand.OrganizationId),
                    "Organization id is required.")
            ];
        }

        return Array.Empty<ValidationError>();
    }
}
