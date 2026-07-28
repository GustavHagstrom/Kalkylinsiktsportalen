using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed class GrantUserFunctionalAccessCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(GrantUserFunctionalAccessCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.UserId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.user_id.required",
                nameof(GrantUserFunctionalAccessCommand.UserId),
                "User id is required."));
        }

        if (command.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.organization_id.required",
                nameof(GrantUserFunctionalAccessCommand.OrganizationId),
                "Organization id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.AccessClaimValue))
        {
            errors.Add(new ValidationError(
                "access.claim_value.required",
                nameof(GrantUserFunctionalAccessCommand.AccessClaimValue),
                "Access claim value is required."));
        }

        return errors;
    }
}
