using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public abstract record RemoveBidconCredentialsResult;

public sealed record BidconCredentialsRemoved(
    OrganizationId OrganizationId) : RemoveBidconCredentialsResult
{
    public bool IsConfigured => false;
}

public sealed record InvalidRemoveBidconCredentialsRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : RemoveBidconCredentialsResult;
