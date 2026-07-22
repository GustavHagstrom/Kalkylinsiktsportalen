using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed record UpsertBidconCredentialsCommand(
    OrganizationId OrganizationId,
    UserId ActorId,
    string ApiKey,
    string ApiSecret) : ICommand<UpsertBidconCredentialsResult>;
