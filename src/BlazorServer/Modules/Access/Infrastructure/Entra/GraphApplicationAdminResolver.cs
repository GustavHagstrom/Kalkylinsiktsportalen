using BlazorServer.Modules.Access.Application;
using BlazorServer.Modules.Access.Infrastructure.Caching;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace BlazorServer.Modules.Access.Infrastructure.Entra;

internal sealed class GraphApplicationAdminResolver : IApplicationAdminResolver
{
    private readonly GraphServiceClient graphClient;
    private readonly IApplicationAdminCache cache;

    public GraphApplicationAdminResolver(
        GraphServiceClient graphClient,
        IApplicationAdminCache cache)
    {
        this.graphClient = graphClient;
        this.cache = cache;
    }

    public async Task<bool> IsApplicationAdminAsync(
        string userId,
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        if (cache.TryGetApplicationAdminStatus(userId, organizationId, out var cachedIsApplicationAdmin))
        {
            return cachedIsApplicationAdmin;
        }

        var memberOf = await graphClient.Users[userId].MemberOf.GetAsync(
            cancellationToken: cancellationToken);
        var isApplicationAdmin = memberOf?.Value?.Any(IsApplicationAdminDirectoryRole) == true;

        cache.SetApplicationAdminStatus(userId, organizationId, isApplicationAdmin);
        return isApplicationAdmin;
    }

    private static bool IsApplicationAdminDirectoryRole(DirectoryObject directoryObject)
    {
        return directoryObject is DirectoryRole role
            && string.Equals(
                role.RoleTemplateId,
                AccessAuthorizationConstants.ApplicationAdminRoleClaimValue,
                StringComparison.OrdinalIgnoreCase);
    }
}
