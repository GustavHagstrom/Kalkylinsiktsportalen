using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record GrantUserViewAccessCommand(
    ViewId ViewId,
    string UserId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed record GrantRoleViewAccessCommand(
    ViewId ViewId,
    string RoleId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed record RevokeUserViewAccessCommand(
    ViewId ViewId,
    string UserId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed record RevokeRoleViewAccessCommand(
    ViewId ViewId,
    string RoleId,
    string OrganizationId,
    ViewAccessRight Right);
