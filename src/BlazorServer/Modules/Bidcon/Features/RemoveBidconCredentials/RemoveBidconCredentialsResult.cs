using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed record RemoveBidconCredentialsResult(OrganizationId OrganizationId, bool IsConfigured);
