using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Features.RemoveBidconCredentials;

public sealed record RemoveBidconCredentialsCommand(
    OrganizationId OrganizationId) : ICommand<RemoveBidconCredentialsResult>;
