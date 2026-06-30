using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed class RemoveBidconCredentialsCommandValidator : IValidator<RemoveBidconCredentialsCommand>
{
    public ValidationResult Validate(RemoveBidconCredentialsCommand command)
    {
        if (command.OrganizationId.IsEmpty)
        {
            return ValidationResult.Failure(
                new ValidationError(
                    "bidcon.credentials.organization.required",
                    nameof(RemoveBidconCredentialsCommand.OrganizationId),
                    "Organization id is required."));
        }

        return ValidationResult.Success();
    }
}
