using BlazorServer.Identity;
using BlazorServer.Kernel.CQRS;

namespace BlazorServer.Modules.Access.Features.RevokeRoleFunctionalAccess;

public sealed record RevokeRoleFunctionalAccessCommand(
    string RoleId,
    OrganizationId OrganizationId,
    string AccessClaimValue) : ICommand<RevokeRoleFunctionalAccessResult>;
