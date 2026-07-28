using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Access.Features.GetRoleFunctionalAccess;

public sealed record GetRoleFunctionalAccessQuery(string RoleId, OrganizationId OrganizationId)
    : IQuery<GetRoleFunctionalAccessResult>;
