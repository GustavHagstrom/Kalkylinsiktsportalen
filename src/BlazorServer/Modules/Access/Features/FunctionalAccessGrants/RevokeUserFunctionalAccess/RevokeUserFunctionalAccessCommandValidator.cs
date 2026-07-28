using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed class RevokeUserFunctionalAccessCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(RevokeUserFunctionalAccessCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.UserId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.user_id.required",
                nameof(RevokeUserFunctionalAccessCommand.UserId),
                "User id is required."));
        }

        if (command.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.organization_id.required",
                nameof(RevokeUserFunctionalAccessCommand.OrganizationId),
                "Organization id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.AccessClaimValue))
        {
            errors.Add(new ValidationError(
                "access.claim_value.required",
                nameof(RevokeUserFunctionalAccessCommand.AccessClaimValue),
                "Access claim value is required."));
        }

        return errors;
    }
}
