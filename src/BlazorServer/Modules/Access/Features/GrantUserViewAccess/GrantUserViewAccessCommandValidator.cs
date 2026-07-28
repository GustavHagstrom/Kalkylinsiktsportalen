using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.GrantUserViewAccess;

public sealed class GrantUserViewAccessCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(GrantUserViewAccessCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.ViewId.IsEmpty)
        {
            errors.Add(new ValidationError("access.view_id.required", nameof(GrantUserViewAccessCommand.ViewId), "View id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            errors.Add(new ValidationError("access.user_id.required", nameof(GrantUserViewAccessCommand.UserId), "User id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.OrganizationId))
        {
            errors.Add(new ValidationError("access.organization_id.required", nameof(GrantUserViewAccessCommand.OrganizationId), "Organization id is required."));
        }

        return errors;
    }
}
