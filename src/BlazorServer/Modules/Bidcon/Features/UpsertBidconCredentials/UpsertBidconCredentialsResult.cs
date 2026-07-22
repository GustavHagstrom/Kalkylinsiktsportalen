using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public abstract record UpsertBidconCredentialsResult;

public sealed record BidconCredentialsSaved(
    OrganizationId OrganizationId) : UpsertBidconCredentialsResult
{
    public bool WasSaved => true;
}

public sealed record BidconCredentialsVerificationFailed(
    OrganizationId OrganizationId,
    string Message) : UpsertBidconCredentialsResult;

public sealed record InvalidUpsertBidconCredentialsRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : UpsertBidconCredentialsResult;
