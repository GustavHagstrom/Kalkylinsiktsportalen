using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.GetBidconCredentials;

public sealed record GetBidconCredentialsQuery(OrganizationId OrganizationId)
    : IQuery<GetBidconCredentialsResult>;
