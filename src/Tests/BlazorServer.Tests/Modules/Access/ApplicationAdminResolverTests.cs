using System.Security.Claims;
using BlazorServer.Identity;
using BlazorServer.Modules.Access.Application;
using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Infrastructure.Identity;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class ApplicationAdminResolverTests
{
    [Test]
    public async Task ClaimsTransformationAddsApplicationAdminClaimFromResolver()
    {
        var transform = new ClaimsTransformation(
            new FunctionalAccessGrantHandler(new EmptyFunctionalAccessGrantStore()),
            new StubApplicationAdminResolver(true),
            new MemoryAccessClaimsCache(new MemoryCache(new MemoryCacheOptions())));
        var principal = Principal("Admin", "org-1");

        var transformed = await transform.TransformAsync(principal);

        Assert.That(transformed.IsApplicationAdmin(), Is.True);
    }

    [Test]
    public async Task ClaimsTransformationDoesNotAddApplicationAdminClaimWhenResolverDenies()
    {
        var transform = new ClaimsTransformation(
            new FunctionalAccessGrantHandler(new EmptyFunctionalAccessGrantStore()),
            new StubApplicationAdminResolver(false),
            new MemoryAccessClaimsCache(new MemoryCache(new MemoryCacheOptions())));
        var principal = Principal("User", "org-1");

        var transformed = await transform.TransformAsync(principal);

        Assert.That(transformed.IsApplicationAdmin(), Is.False);
    }

    [Test]
    public async Task MockApplicationAdminResolverMatchesConfiguredOrganizationAndAdminUsers()
    {
        var resolver = new MockApplicationAdminResolver(
            new OrganizationId("org-1"),
            [new UserId("Admin")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await resolver.IsApplicationAdminAsync("Admin", "org-1"), Is.True);
            Assert.That(await resolver.IsApplicationAdminAsync("User", "org-1"), Is.False);
            Assert.That(await resolver.IsApplicationAdminAsync("Admin", "org-2"), Is.False);
        }
    }

    private static ClaimsPrincipal Principal(string userId, string organizationId)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(IdentityClaimTypes.UserId, userId),
            new Claim(IdentityClaimTypes.OrganizationId, organizationId)
        ], "test"));
    }

    private sealed class StubApplicationAdminResolver : IApplicationAdminResolver
    {
        private readonly bool isApplicationAdmin;

        public StubApplicationAdminResolver(bool isApplicationAdmin)
        {
            this.isApplicationAdmin = isApplicationAdmin;
        }

        public Task<bool> IsApplicationAdminAsync(
            string userId,
            string organizationId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(isApplicationAdmin);
        }
    }

    private sealed class EmptyFunctionalAccessGrantStore : IFunctionalAccessGrantStore
    {
        public Task AddAccessGrantAsync(
            FunctionalAccessGrantTarget target,
            OrganizationId orgId,
            string accessClaimValue)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAccessGrantAsync(
            FunctionalAccessGrantTarget target,
            OrganizationId orgId,
            string accessClaimValue)
        {
            return Task.CompletedTask;
        }

        public Task<FunctionalAccessGrant> GetAccessGrantAsync(
            FunctionalAccessGrantTarget target,
            OrganizationId orgId)
        {
            return Task.FromResult(new FunctionalAccessGrant(target, orgId, []));
        }

        public Task<List<AccessClaim>> GetAccessClaimsForUserAsync(
            UserId userId,
            IEnumerable<string> roleIds,
            OrganizationId orgId)
        {
            return Task.FromResult(new List<AccessClaim>());
        }
    }
}
