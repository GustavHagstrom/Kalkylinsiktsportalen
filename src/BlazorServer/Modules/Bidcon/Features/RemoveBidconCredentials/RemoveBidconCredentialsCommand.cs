using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed record RemoveBidconCredentialsCommand(
    OrganizationId OrganizationId) : ICommand<RemoveBidconCredentialsResult>;
