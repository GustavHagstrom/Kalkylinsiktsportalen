using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed record GetBidconCredentialsQuery(OrganizationId OrganizationId)
    : IQuery<GetBidconCredentialsResult>;
