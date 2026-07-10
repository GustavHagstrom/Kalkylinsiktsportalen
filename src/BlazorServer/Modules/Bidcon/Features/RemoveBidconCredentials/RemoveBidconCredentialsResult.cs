using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed record RemoveBidconCredentialsResult(
    OrganizationId? OrganizationId,
    bool IsConfigured,
    IReadOnlyList<ValidationError> Errors) : IValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public bool IsSuccess => IsValid;

    public static RemoveBidconCredentialsResult Success(OrganizationId organizationId, bool isConfigured)
        => new(organizationId, isConfigured, Array.Empty<ValidationError>());

    public static RemoveBidconCredentialsResult Invalid(params ValidationError[] errors)
        => new(null, false, errors);
}
