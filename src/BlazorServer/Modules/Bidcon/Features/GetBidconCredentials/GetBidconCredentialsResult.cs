using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public abstract record GetBidconCredentialsResult;

public sealed record BidconCredentialsFound(
    OrganizationId OrganizationId,
    string ApiKey,
    string ApiSecret) : GetBidconCredentialsResult
{
    public bool IsConfigured => true;
}

public sealed record BidconCredentialsNotConfigured(
    OrganizationId OrganizationId) : GetBidconCredentialsResult
{
    public string ApiKey => string.Empty;

    public string ApiSecret => string.Empty;

    public bool IsConfigured => false;
}

public sealed record InvalidGetBidconCredentialsRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GetBidconCredentialsResult;
