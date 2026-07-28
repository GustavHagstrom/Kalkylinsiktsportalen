using BlazorServer.Kernel.Validation;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.GrantUserViewAccess;

public abstract record GrantUserViewAccessResult;

public sealed record UserViewAccessGranted(
    ViewId ViewId,
    string OrganizationId,
    string UserId,
    ViewAccessRight Right) : GrantUserViewAccessResult;

public sealed record InvalidGrantUserViewAccessRequest(
    IReadOnlyList<ValidationError> ValidationErrors) : GrantUserViewAccessResult;
