using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record GetUserFunctionalAccessQuery(UserId UserId, OrganizationId OrganizationId)
    : IQuery<GetUserFunctionalAccessResult>;
