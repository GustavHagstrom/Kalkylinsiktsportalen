using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;

namespace BlazorServer.Modules.Access.Persistence;

public interface IViewAccessGrantStore
{
    Task<IReadOnlyList<ViewAccessGrant>> GetGrantsForViewAsync(ViewId viewId, string organizationId);
    Task GrantAsync(ViewAccessGrant grant);
    Task RevokeAsync(ViewAccessGrant grant);
}
