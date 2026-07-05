using System.Security.Claims;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record CanUseViewQuery(ClaimsPrincipal User, string ViewId);

public sealed record CanEditViewQuery(ClaimsPrincipal User, string ViewId);

public sealed record CanAdministerViewAccessQuery(ClaimsPrincipal User, string ViewId);
