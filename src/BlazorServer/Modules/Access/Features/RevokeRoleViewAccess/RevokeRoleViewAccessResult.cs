using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.RevokeRoleViewAccess;

public abstract record RevokeRoleViewAccessResult;

public sealed record RoleViewAccessRevoked(
    ViewId ViewId,
    string OrganizationId,
    string RoleId,
    ViewAccessRight Right) : RevokeRoleViewAccessResult;

public sealed record InvalidRevokeRoleViewAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : RevokeRoleViewAccessResult;
