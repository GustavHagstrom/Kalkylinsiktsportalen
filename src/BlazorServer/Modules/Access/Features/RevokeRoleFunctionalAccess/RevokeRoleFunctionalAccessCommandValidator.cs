using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.RevokeRoleFunctionalAccess;

public sealed class RevokeRoleFunctionalAccessCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(RevokeRoleFunctionalAccessCommand command)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(command.RoleId))
        {
            errors.Add(new ValidationError(
                "access.role_id.required",
                nameof(RevokeRoleFunctionalAccessCommand.RoleId),
                "Role id is required."));
        }

        if (command.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.organization_id.required",
                nameof(RevokeRoleFunctionalAccessCommand.OrganizationId),
                "Organization id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.AccessClaimValue))
        {
            errors.Add(new ValidationError(
                "access.claim_value.required",
                nameof(RevokeRoleFunctionalAccessCommand.AccessClaimValue),
                "Access claim value is required."));
        }

        return errors;
    }
}
