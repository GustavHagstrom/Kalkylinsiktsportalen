using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed class UpsertBidconCredentialsCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(UpsertBidconCredentialsCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "bidcon.credentials.organization.required",
                nameof(UpsertBidconCredentialsCommand.OrganizationId),
                "Organization id is required."));
        }

        if (command.ActorId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "bidcon.credentials.actor.required",
                nameof(UpsertBidconCredentialsCommand.ActorId),
                "Actor id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.ApiKey))
        {
            errors.Add(new ValidationError(
                "bidcon.credentials.apiKey.required",
                nameof(UpsertBidconCredentialsCommand.ApiKey),
                "Bidcon API key is required."));
        }

        if (string.IsNullOrWhiteSpace(command.ApiSecret))
        {
            errors.Add(new ValidationError(
                "bidcon.credentials.apiSecret.required",
                nameof(UpsertBidconCredentialsCommand.ApiSecret),
                "Bidcon API secret is required."));
        }

        return errors;
    }
}
