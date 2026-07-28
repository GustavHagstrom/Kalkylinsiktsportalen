using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.RevokeUserFunctionalAccess;

public abstract record RevokeUserFunctionalAccessResult;

public sealed record UserFunctionalAccessRevoked(
    UserId UserId,
    OrganizationId OrganizationId,
    IReadOnlyList<string> AccessClaimValues) : RevokeUserFunctionalAccessResult;

public sealed record InvalidRevokeUserFunctionalAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : RevokeUserFunctionalAccessResult;
