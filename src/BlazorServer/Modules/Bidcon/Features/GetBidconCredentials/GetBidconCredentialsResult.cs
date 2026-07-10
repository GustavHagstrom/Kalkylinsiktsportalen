using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed record GetBidconCredentialsResult(
    OrganizationId? OrganizationId,
    string? ApiKey,
    string? ApiSecret,
    bool IsConfigured,
    IReadOnlyList<ValidationError> Errors) : IValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public bool IsSuccess => IsValid;

    public static GetBidconCredentialsResult Success(
        OrganizationId organizationId,
        string apiKey,
        string apiSecret,
        bool isConfigured)
        => new(organizationId, apiKey, apiSecret, isConfigured, Array.Empty<ValidationError>());

    public static GetBidconCredentialsResult Invalid(params ValidationError[] errors)
        => new(null, null, null, false, errors);
}
