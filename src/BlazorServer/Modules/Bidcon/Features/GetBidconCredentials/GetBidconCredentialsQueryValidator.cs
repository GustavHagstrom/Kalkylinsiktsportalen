using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed class GetBidconCredentialsQueryValidator
{
    public IReadOnlyList<ValidationError> Validate(GetBidconCredentialsQuery query)
    {
        if (query.OrganizationId.IsEmpty)
        {
            return
            [
                new ValidationError(
                    "bidcon.credentials.organization.required",
                    nameof(GetBidconCredentialsQuery.OrganizationId),
                    "Organization id is required.")
            ];
        }

        return Array.Empty<ValidationError>();
    }
}
