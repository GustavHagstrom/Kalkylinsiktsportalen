using BlazorServer.Identity;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record GrantUserFunctionalAccessCommand(
    UserId UserId,
    OrganizationId OrganizationId,
    string AccessClaimValue);

public sealed record GrantRoleFunctionalAccessCommand(
    string RoleId,
    OrganizationId OrganizationId,
    string AccessClaimValue);

public sealed record RevokeUserFunctionalAccessCommand(
    UserId UserId,
    OrganizationId OrganizationId,
    string AccessClaimValue);

public sealed record RevokeRoleFunctionalAccessCommand(
    string RoleId,
    OrganizationId OrganizationId,
    string AccessClaimValue);

public sealed record GetUserFunctionalAccessQuery(UserId UserId, OrganizationId OrganizationId);

public sealed record GetRoleFunctionalAccessQuery(string RoleId, OrganizationId OrganizationId);

public sealed record GetEffectiveFunctionalAccessClaimsQuery(
    UserId UserId,
    IReadOnlyCollection<string> RoleIds,
    OrganizationId OrganizationId);
