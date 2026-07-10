using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

public sealed class GetEstimationListQueryValidator
{
    public IReadOnlyList<ValidationError> Validate(GetEstimationListQuery query)
    {
        var errors = new List<ValidationError>();

        if (query.OrganizationId.IsEmpty)
        {
            errors.Add(new ValidationError(
                "bidcon.estimationList.organization.required",
                nameof(GetEstimationListQuery.OrganizationId),
                "Organization id is required."));
        }

        if (string.IsNullOrWhiteSpace(query.UserEmail))
        {
            errors.Add(new ValidationError(
                "bidcon.estimationList.userEmail.required",
                nameof(GetEstimationListQuery.UserEmail),
                "User email is required."));
        }

        return errors;
    }
}
