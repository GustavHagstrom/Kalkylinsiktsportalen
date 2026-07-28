using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.GetUserFunctionalAccess;

public abstract record GetUserFunctionalAccessResult;

public sealed record UserFunctionalAccessLoaded(
    UserId UserId,
    OrganizationId OrganizationId,
    IReadOnlyList<string> AccessClaimValues) : GetUserFunctionalAccessResult;

public sealed record InvalidGetUserFunctionalAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GetUserFunctionalAccessResult;
