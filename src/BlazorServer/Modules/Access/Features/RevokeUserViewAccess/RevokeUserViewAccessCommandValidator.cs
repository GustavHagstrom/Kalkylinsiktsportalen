using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.RevokeUserViewAccess;

public sealed class RevokeUserViewAccessCommandValidator
{
    public IReadOnlyList<ValidationError> Validate(RevokeUserViewAccessCommand command)
    {
        var errors = new List<ValidationError>();

        if (command.ViewId.IsEmpty)
        {
            errors.Add(new ValidationError("access.view_id.required", nameof(RevokeUserViewAccessCommand.ViewId), "View id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            errors.Add(new ValidationError("access.user_id.required", nameof(RevokeUserViewAccessCommand.UserId), "User id is required."));
        }

        if (string.IsNullOrWhiteSpace(command.OrganizationId))
        {
            errors.Add(new ValidationError("access.organization_id.required", nameof(RevokeUserViewAccessCommand.OrganizationId), "Organization id is required."));
        }

        return errors;
    }
}
