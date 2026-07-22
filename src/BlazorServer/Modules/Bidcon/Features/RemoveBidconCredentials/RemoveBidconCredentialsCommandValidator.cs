using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed class RemoveBidconCredentialsCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(RemoveBidconCredentialsCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "bidcon.credentials.organization.required",
                nameof(RemoveBidconCredentialsCommand.OrganizationId),
                "Organization id is required."));
        }

        if (command.ActorId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "bidcon.credentials.actor.required",
                nameof(RemoveBidconCredentialsCommand.ActorId),
                "Actor id is required."));
        }

        return errors;
    }
}
