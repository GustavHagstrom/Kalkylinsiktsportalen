using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.GetEffectiveFunctionalAccessClaims;

public sealed class GetEffectiveFunctionalAccessClaimsQueryValidator
{
    public IReadOnlyList<ValidationError> Validate(GetEffectiveFunctionalAccessClaimsQuery query)
    {
        var errors = new List<ValidationError>();

        if (query.UserId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.user_id.required",
                nameof(GetEffectiveFunctionalAccessClaimsQuery.UserId),
                "User id is required."));
        }

        if (query.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.organization_id.required",
                nameof(GetEffectiveFunctionalAccessClaimsQuery.OrganizationId),
                "Organization id is required."));
        }

        if (query.RoleIds is null)
        {
            errors.Add(new ValidationError(
                "access.role_ids.required",
                nameof(GetEffectiveFunctionalAccessClaimsQuery.RoleIds),
                "Role ids are required."));
        }

        return errors;
    }
}
