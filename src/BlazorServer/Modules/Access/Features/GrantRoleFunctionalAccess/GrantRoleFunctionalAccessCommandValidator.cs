using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.GrantRoleFunctionalAccess;

public sealed class GrantRoleFunctionalAccessCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(GrantRoleFunctionalAccessCommand command)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(command.RoleId))
        {
            errors.Add(new ValidationError(
                "access.role_id.required",
                nameof(GrantRoleFunctionalAccessCommand.RoleId),
                "Role id is required."));
        }

        if (command.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.organization_id.required",
                nameof(GrantRoleFunctionalAccessCommand.OrganizationId),
                "Organization id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.AccessClaimValue))
        {
            errors.Add(new ValidationError(
                "access.claim_value.required",
                nameof(GrantRoleFunctionalAccessCommand.AccessClaimValue),
                "Access claim value is required."));
        }

        return errors;
    }
}
