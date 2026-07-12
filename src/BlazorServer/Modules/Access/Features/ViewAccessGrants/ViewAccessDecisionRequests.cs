using System.Security.Claims;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record CanUseViewQuery(ClaimsPrincipal User, ViewId ViewId);

public sealed record CanEditViewQuery(ClaimsPrincipal User, ViewId ViewId);

public sealed record CanAdministerViewAccessQuery(ClaimsPrincipal User, ViewId ViewId);
