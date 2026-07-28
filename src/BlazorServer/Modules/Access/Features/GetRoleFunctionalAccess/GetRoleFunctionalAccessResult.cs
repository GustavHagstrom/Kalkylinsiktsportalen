using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.GetRoleFunctionalAccess;

public abstract record GetRoleFunctionalAccessResult;

public sealed record RoleFunctionalAccessLoaded(
    string RoleId,
    OrganizationId OrganizationId,
    IReadOnlyList<string> AccessClaimValues) : GetRoleFunctionalAccessResult;

public sealed record InvalidGetRoleFunctionalAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GetRoleFunctionalAccessResult;
