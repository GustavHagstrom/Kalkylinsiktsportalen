using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed record RemoveBidconCredentialsCommand(
    OrganizationId OrganizationId) : ICommand<RemoveBidconCredentialsResult>;
