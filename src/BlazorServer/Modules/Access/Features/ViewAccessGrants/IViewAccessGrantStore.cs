namespace BlazorServer.Modules.Access.Features.ViewAccessGrants;

public interface IViewAccessGrantStore
{
    Task<IReadOnlyList<ViewAccessGrant>> GetGrantsForViewAsync(string viewId, string organizationId);
    Task GrantAsync(ViewAccessGrant grant);
    Task RevokeAsync(ViewAccessGrant grant);
}
