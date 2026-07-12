using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record ViewAccessGrant(
    ViewId ViewId,
    string OrganizationId,
    ViewAccessGrantTargetType TargetType,
    string TargetId,
    ViewAccessRight Right);
