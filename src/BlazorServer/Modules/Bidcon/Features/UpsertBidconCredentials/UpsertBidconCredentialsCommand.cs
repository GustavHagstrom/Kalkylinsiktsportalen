using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;

public sealed record UpsertBidconCredentialsCommand(
    OrganizationId OrganizationId,
    string ApiKey,
    string ApiSecret) : ICommand<UpsertBidconCredentialsResult>;
