using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Persistence;

namespace BlazorServer.Modules.Access.Infrastructure.InMemory;

public sealed class InMemoryViewAccessGrantStore : IViewAccessGrantStore
{
    private readonly HashSet<ViewAccessGrant> grants = [];

    public Task<IReadOnlyList<ViewAccessGrant>> GetGrantsForViewAsync(ViewId viewId, string organizationId)
    {
        var matchingGrants = grants
            .Where(grant =>
                grant.ViewId == viewId &&
                grant.OrganizationId == organizationId)
            .ToArray();

        return Task.FromResult<IReadOnlyList<ViewAccessGrant>>(matchingGrants);
    }

    public Task GrantAsync(ViewAccessGrant grant)
    {
        grants.Add(grant);
        return Task.CompletedTask;
    }

    public Task RevokeAsync(ViewAccessGrant grant)
    {
        grants.Remove(grant);
        return Task.CompletedTask;
    }
}
