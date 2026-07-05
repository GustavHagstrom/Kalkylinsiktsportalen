using BlazorServer.Modules.Access.Domain;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public sealed record GrantUserFunctionalAccessCommand(
    string UserId,
    string OrganizationId,
    string AccessClaimValue);

public sealed record GrantRoleFunctionalAccessCommand(
    string RoleId,
    string OrganizationId,
    string AccessClaimValue);

public sealed record RevokeUserFunctionalAccessCommand(
    string UserId,
    string OrganizationId,
    string AccessClaimValue);

public sealed record RevokeRoleFunctionalAccessCommand(
    string RoleId,
    string OrganizationId,
    string AccessClaimValue);

public sealed record GetUserFunctionalAccessQuery(string UserId, string OrganizationId);

public sealed record GetRoleFunctionalAccessQuery(string RoleId, string OrganizationId);

public sealed record GetEffectiveFunctionalAccessClaimsQuery(
    string UserId,
    IReadOnlyCollection<string> RoleIds,
    string OrganizationId);
