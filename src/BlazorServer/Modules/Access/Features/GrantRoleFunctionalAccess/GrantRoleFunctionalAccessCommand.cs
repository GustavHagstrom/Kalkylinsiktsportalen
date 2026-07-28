using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Access.Features.GrantRoleFunctionalAccess;

public sealed record GrantRoleFunctionalAccessCommand(
    string RoleId,
    OrganizationId OrganizationId,
    string AccessClaimValue) : ICommand<GrantRoleFunctionalAccessResult>;
