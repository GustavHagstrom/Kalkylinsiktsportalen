namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record ViewAccessGrant(
    string ViewId,
    string OrganizationId,
    ViewAccessGrantTargetType TargetType,
    string TargetId,
    ViewAccessRight Right);
