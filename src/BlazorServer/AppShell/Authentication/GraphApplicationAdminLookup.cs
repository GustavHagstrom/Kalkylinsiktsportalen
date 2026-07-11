using BlazorServer.Modules.Access.Authentication;
using Microsoft.Graph;

namespace BlazorServer.AppShell.Authentication;

public class GraphApplicationAdminLookup : IApplicationAdminLookup
{
    private readonly GraphServiceClient graphClient;
    private readonly IApplicationAdminCache cache;

    public GraphApplicationAdminLookup(GraphServiceClient graphClient, IApplicationAdminCache cache)
    {
        this.graphClient = graphClient;
        this.cache = cache;
    }

    public async Task<bool> IsApplicationAdminAsync(string userId, string organizationId)
    {
        if (cache.TryGetIsAdminRole(userId, organizationId, out var isAdmin))
        {
            return isAdmin;
        }

        var roleResult = await graphClient.Users[userId].MemberOf.GetAsync();
        var adminRole = roleResult?.Value?
            .OfType<Microsoft.Graph.Models.DirectoryRole>()
            .FirstOrDefault(role => role.RoleTemplateId == AccessPolicies.ApplicationAdminRoleClaimValue);

        if (adminRole is not null)
        {
            cache.SetIsAdminRole(userId, organizationId);
            return true;
        }

        cache.ClearIsAdminRole(userId, organizationId);
        return false;
    }
}
