using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed record GetBidconCredentialsQuery(OrganizationId OrganizationId)
    : IQuery<GetBidconCredentialsResult>;
