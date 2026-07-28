using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.RevokeRoleViewAccess;

public sealed class RevokeRoleViewAccessCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(RevokeRoleViewAccessCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.ViewId.IsEmpty)
        {
            errors.Add(new ValidationError("access.view_id.required", nameof(RevokeRoleViewAccessCommand.ViewId), "View id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.RoleId))
        {
            errors.Add(new ValidationError("access.role_id.required", nameof(RevokeRoleViewAccessCommand.RoleId), "Role id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.OrganizationId))
        {
            errors.Add(new ValidationError("access.organization_id.required", nameof(RevokeRoleViewAccessCommand.OrganizationId), "Organization id is required."));
        }

        return errors;
    }
}
