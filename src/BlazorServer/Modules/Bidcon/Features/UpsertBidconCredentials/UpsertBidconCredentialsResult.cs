using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed record UpsertBidconCredentialsResult(OrganizationId OrganizationId, bool IsConfigured);
