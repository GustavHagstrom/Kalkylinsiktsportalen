using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.RevokeRoleFunctionalAccess;

public abstract record RevokeRoleFunctionalAccessResult;

public sealed record RoleFunctionalAccessRevoked(
    string RoleId,
    OrganizationId OrganizationId,
    IReadOnlyList<string> AccessClaimValues) : RevokeRoleFunctionalAccessResult;

public sealed record InvalidRevokeRoleFunctionalAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : RevokeRoleFunctionalAccessResult;
