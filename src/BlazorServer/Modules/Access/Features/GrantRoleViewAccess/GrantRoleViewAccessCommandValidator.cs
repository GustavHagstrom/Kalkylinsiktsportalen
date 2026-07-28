using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.GrantRoleViewAccess;

public sealed class GrantRoleViewAccessCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(GrantRoleViewAccessCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.ViewId.IsEmpty)
        {
            errors.Add(new ValidationError("access.view_id.required", nameof(GrantRoleViewAccessCommand.ViewId), "View id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.RoleId))
        {
            errors.Add(new ValidationError("access.role_id.required", nameof(GrantRoleViewAccessCommand.RoleId), "Role id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.OrganizationId))
        {
            errors.Add(new ValidationError("access.organization_id.required", nameof(GrantRoleViewAccessCommand.OrganizationId), "Organization id is required."));
        }

        return errors;
    }
}
