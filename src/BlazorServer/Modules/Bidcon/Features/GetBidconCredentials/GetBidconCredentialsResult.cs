using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed record GetBidconCredentialsResult(
    OrganizationId OrganizationId,
    string ApiKey,
    string ApiSecret,
    bool IsConfigured);
