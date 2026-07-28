using BlazorServer.Identity;
using BlazorServer.Kernel.Validation;

namespace BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

public abstract record GrantUserFunctionalAccessResult;

public sealed record UserFunctionalAccessGranted(
    UserId UserId,
    OrganizationId OrganizationId,
    IReadOnlyList<string> AccessClaimValues) : GrantUserFunctionalAccessResult;

public sealed record InvalidGrantUserFunctionalAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GrantUserFunctionalAccessResult;
