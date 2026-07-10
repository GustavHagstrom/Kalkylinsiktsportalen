using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed record UpsertBidconCredentialsResult(
    OrganizationId? OrganizationId,
    bool IsConfigured,
    IReadOnlyList<ValidationError> Errors) : IValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public bool IsSuccess => IsValid;

    public static UpsertBidconCredentialsResult Success(OrganizationId organizationId, bool isConfigured)
        => new(organizationId, isConfigured, Array.Empty<ValidationError>());

    public static UpsertBidconCredentialsResult Invalid(params ValidationError[] errors)
        => new(null, false, errors);
}
