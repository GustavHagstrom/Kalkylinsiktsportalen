using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

public sealed record GetEstimationListQuery(
    OrganizationId OrganizationId,
    string UserEmail) : IQuery<GetEstimationListResult>;
