using BlazorServer.Identity;
using BlazorServer.Modules.Access.Application;

namespace BlazorServer.Modules.Access.Infrastructure.Identity;

public sealed class MockApplicationAdminResolver : IApplicationAdminResolver
{
    private readonly OrganizationId organizationId;
    private readonly HashSet<string> applicationAdminUserIds;

    public MockApplicationAdminResolver(
        OrganizationId organizationId,
        IEnumerable<UserId> applicationAdminUserIds)
    {
        this.organizationId = organizationId;
        this.applicationAdminUserIds = applicationAdminUserIds
            .Select(userId => userId.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public Task<bool> IsApplicationAdminAsync(
        string userId,
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        var isApplicationAdmin = this.organizationId.Value.Equals(organizationId, StringComparison.Ordinal)
            && applicationAdminUserIds.Contains(userId);

        return Task.FromResult(isApplicationAdmin);
    }
}
