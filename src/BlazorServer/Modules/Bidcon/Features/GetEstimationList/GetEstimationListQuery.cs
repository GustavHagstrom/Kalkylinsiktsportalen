using BlazorServer.Shared.Kernel.CQRS;
using BlazorServer.Shared.Kernel.Identity;

namespace BlazorServer.Modules.Bidcon.Features.GetEstimationList;

public sealed record GetEstimationListQuery(
    OrganizationId OrganizationId,
    string UserEmail) : IQuery<GetEstimationListResult>;
