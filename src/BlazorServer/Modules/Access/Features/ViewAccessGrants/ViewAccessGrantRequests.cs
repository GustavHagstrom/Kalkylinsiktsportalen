namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record GrantUserViewAccessCommand(
    string ViewId,
    string UserId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed record GrantRoleViewAccessCommand(
    string ViewId,
    string RoleId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed record RevokeUserViewAccessCommand(
    string ViewId,
    string UserId,
    string OrganizationId,
    ViewAccessRight Right);

public sealed record RevokeRoleViewAccessCommand(
    string ViewId,
    string RoleId,
    string OrganizationId,
    ViewAccessRight Right);
