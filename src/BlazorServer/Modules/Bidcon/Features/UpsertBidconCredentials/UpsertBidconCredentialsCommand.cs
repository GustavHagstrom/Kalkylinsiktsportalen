using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed record UpsertBidconCredentialsCommand(
    OrganizationId OrganizationId,
    string ApiKey,
    string ApiSecret) : ICommand<UpsertBidconCredentialsResult>;
