using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.RevokeUserViewAccess;

public abstract record RevokeUserViewAccessResult;

public sealed record UserViewAccessRevoked(
    ViewId ViewId,
    string OrganizationId,
    string UserId,
    ViewAccessRight Right) : RevokeUserViewAccessResult;

public sealed record InvalidRevokeUserViewAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : RevokeUserViewAccessResult;
