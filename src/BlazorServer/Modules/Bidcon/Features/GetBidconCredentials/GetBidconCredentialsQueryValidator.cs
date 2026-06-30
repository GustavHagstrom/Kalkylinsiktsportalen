using BlazorServer.Shared.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed class GetBidconCredentialsQueryValidator : IValidator<GetBidconCredentialsQuery>
{
    public ValidationResult Validate(GetBidconCredentialsQuery query)
    {
        if (query.OrganizationId.IsEmpty)
        {
            return ValidationResult.Failure(
                new ValidationError(
                    "bidcon.credentials.organization.required",
                    nameof(GetBidconCredentialsQuery.OrganizationId),
                    "Organization id is required."));
        }

        return ValidationResult.Success();
    }
}
