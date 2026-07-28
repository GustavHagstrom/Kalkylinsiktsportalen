using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.GrantRoleViewAccess;

public abstract record GrantRoleViewAccessResult;

public sealed record RoleViewAccessGranted(
    ViewId ViewId,
    string OrganizationId,
    string RoleId,
    ViewAccessRight Right) : GrantRoleViewAccessResult;

public sealed record InvalidGrantRoleViewAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GrantRoleViewAccessResult;
