using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed class GetUserFunctionalAccessQueryValidator
{
    public IReadOnlyList<ValidationError> Validate(GetUserFunctionalAccessQuery query)
    {
        var errors = new List<ValidationError>();

        if (query.UserId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.user_id.required",
                nameof(GetUserFunctionalAccessQuery.UserId),
                "User id is required."));
        }

        if (query.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "access.organization_id.required",
                nameof(GetUserFunctionalAccessQuery.OrganizationId),
                "Organization id is required."));
        }

        return errors;
    }
}
