namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record AccessSubject(
    string UserId,
    string OrganizationId,
    IReadOnlySet<string> RoleIds,
    bool IsApplicationAdmin);
