using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public sealed record CanUseViewQuery(AccessSubject Subject, ViewId ViewId);

public sealed record CanEditViewQuery(AccessSubject Subject, ViewId ViewId);

public sealed record CanAdministerViewAccessQuery(AccessSubject Subject, ViewId ViewId);
