using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public interface IViewAccessGrantStore
{
    Task<IReadOnlyList<ViewAccessGrant>> GetGrantsForViewAsync(ViewId viewId, string organizationId);
    Task GrantAsync(ViewAccessGrant grant);
    Task RevokeAsync(ViewAccessGrant grant);
}
