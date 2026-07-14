using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

public sealed record GetEstimationListQuery(
    OrganizationId OrganizationId,
    string UserEmail) : IQuery<GetEstimationListResult>;
