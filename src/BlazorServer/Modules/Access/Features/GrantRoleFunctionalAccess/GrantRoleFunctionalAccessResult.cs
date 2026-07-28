using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.GrantRoleFunctionalAccess;

public abstract record GrantRoleFunctionalAccessResult;

public sealed record RoleFunctionalAccessGranted(
    string RoleId,
    OrganizationId OrganizationId,
    IReadOnlyList<string> AccessClaimValues) : GrantRoleFunctionalAccessResult;

public sealed record InvalidGrantRoleFunctionalAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GrantRoleFunctionalAccessResult;
