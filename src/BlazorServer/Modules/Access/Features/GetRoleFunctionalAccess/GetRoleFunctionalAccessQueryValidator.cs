using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.GetRoleFunctionalAccess;

public sealed class GetRoleFunctionalAccessQueryValidator
{
    public IReadOnlyList<ValidationError> Validate(GetRoleFunctionalAccessQuery query)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(query.RoleId))
        {
            errors.Add(new ValidationError(
                "access.role_id.required",
                nameof(GetRoleFunctionalAccessQuery.RoleId),
                "Role id is required."));
        }

        if (query.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.organization_id.required",
                nameof(GetRoleFunctionalAccessQuery.OrganizationId),
                "Organization id is required."));
        }

        return errors;
    }
}
