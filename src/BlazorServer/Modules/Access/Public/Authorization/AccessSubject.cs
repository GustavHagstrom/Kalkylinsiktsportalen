using BlazorServer.Identity;

namespace BlazorServer.Modules.Access.Public.Authorization;

public sealed record AccessSubject(
    UserId UserId,
    OrganizationId OrganizationId,
    IReadOnlySet<string> RoleIds,
    bool IsApplicationAdmin);
