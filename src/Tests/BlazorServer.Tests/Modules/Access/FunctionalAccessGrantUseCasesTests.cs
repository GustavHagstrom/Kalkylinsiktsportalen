using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class FunctionalAccessGrantUseCasesTests
{
    [Test]
    public async Task UserAccessGrantsCanBeReadGrantedRevokedAndGrantedIdempotently()
    {
        var store = new RecordingAccessStore();
        var useCases = new FunctionalAccessGrantUseCases(store);

        await useCases.GrantUserAccessAsync("user-1", "org-1", "CanManageAccess");
        await useCases.GrantUserAccessAsync("user-1", "org-1", "CanManageAccess");

        var granted = await useCases.GetUserAccessAsync("user-1", "org-1");

        await useCases.RevokeUserAccessAsync("user-1", "org-1", "CanManageAccess");
        var revoked = await useCases.GetUserAccessAsync("user-1", "org-1");

        Assert.Multiple(() =>
        {
            Assert.That(granted.AccessClaimValues, Is.EqualTo(new[] { "CanManageAccess" }));
            Assert.That(revoked.AccessClaimValues, Is.Empty);
            Assert.That(store.UserGrantCalls, Is.EqualTo(2));
            Assert.That(store.UserRevokeCalls, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task RoleAccessGrantsCanBeReadGrantedRevokedAndGrantedIdempotently()
    {
        var store = new RecordingAccessStore();
        var useCases = new FunctionalAccessGrantUseCases(store);

        await useCases.GrantRoleAccessAsync("role-1", "org-1", "CanManageAccess");
        await useCases.GrantRoleAccessAsync("role-1", "org-1", "CanManageAccess");

        var granted = await useCases.GetRoleAccessAsync("role-1", "org-1");

        await useCases.RevokeRoleAccessAsync("role-1", "org-1", "CanManageAccess");
        var revoked = await useCases.GetRoleAccessAsync("role-1", "org-1");

        Assert.Multiple(() =>
        {
            Assert.That(granted.AccessClaimValues, Is.EqualTo(new[] { "CanManageAccess" }));
            Assert.That(revoked.AccessClaimValues, Is.Empty);
            Assert.That(store.RoleGrantCalls, Is.EqualTo(2));
            Assert.That(store.RoleRevokeCalls, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task RuntimeAccessCombinesDirectUserAndExternalRoleGroupGrants()
    {
        var store = new RecordingAccessStore();
        var useCases = new FunctionalAccessGrantUseCases(store);

        await useCases.GrantUserAccessAsync("user-1", "org-1", "DirectAccess");
        await useCases.GrantRoleAccessAsync("role-1", "org-1", "GroupAccess");
        await useCases.GrantRoleAccessAsync("role-2", "org-1", "GroupAccess");

        var effectiveClaims = await useCases.GetAccessClaimsForUserAsync(
            "user-1",
            ["role-1", "role-2"],
            "org-1");

        Assert.That(effectiveClaims.Select(claim => claim.Value), Is.EquivalentTo([
            "DirectAccess",
            "GroupAccess"
        ]));
    }

    private sealed class RecordingAccessStore : IAccessStore
    {
        private readonly Dictionary<(string UserId, string OrgId), HashSet<string>> userAccess = [];
        private readonly Dictionary<(string RoleId, string OrgId), HashSet<string>> roleAccess = [];

        public int UserGrantCalls { get; private set; }
        public int RoleGrantCalls { get; private set; }
        public int UserRevokeCalls { get; private set; }
        public int RoleRevokeCalls { get; private set; }

        public Task<UserAccess> GetUserAccessAsync(string userId, string orgId)
        {
            return Task.FromResult(new UserAccess
            {
                UserId = userId,
                OrganizationId = orgId,
                AccessClaimValues = ValuesFor(userAccess, (userId, orgId))
            });
        }

        public Task<RoleAccess> GetRoleAccessAsync(string roleId, string orgId)
        {
            return Task.FromResult(new RoleAccess
            {
                RoleId = roleId,
                OrgId = orgId,
                AccessClaimValues = ValuesFor(roleAccess, (roleId, orgId))
            });
        }

        public Task<List<AccessClaim>> GetAccessClaimsForUserAsync(string userId, IEnumerable<string> roleIds, string orgId)
        {
            var values = ValuesFor(userAccess, (userId, orgId)).ToHashSet(StringComparer.Ordinal);
            foreach (var roleId in roleIds)
            {
                values.UnionWith(ValuesFor(roleAccess, (roleId, orgId)));
            }

            return Task.FromResult(values.Select(value => new AccessClaim(value)).ToList());
        }

        public Task AddUserAccessAsync(string userId, string orgId, string accessClaimValue)
        {
            UserGrantCalls++;
            ValuesForMutation(userAccess, (userId, orgId)).Add(accessClaimValue);
            return Task.CompletedTask;
        }

        public Task AddRoleAccessAsync(string roleId, string orgId, string accessClaimValue)
        {
            RoleGrantCalls++;
            ValuesForMutation(roleAccess, (roleId, orgId)).Add(accessClaimValue);
            return Task.CompletedTask;
        }

        public Task DeleteUserAccessAsync(string userId, string orgId, string accessClaimValue)
        {
            UserRevokeCalls++;
            ValuesForMutation(userAccess, (userId, orgId)).Remove(accessClaimValue);
            return Task.CompletedTask;
        }

        public Task DeleteRoleAccessAsync(string roleId, string orgId, string accessClaimValue)
        {
            RoleRevokeCalls++;
            ValuesForMutation(roleAccess, (roleId, orgId)).Remove(accessClaimValue);
            return Task.CompletedTask;
        }

        private static List<string> ValuesFor<TKey>(
            Dictionary<TKey, HashSet<string>> valuesByKey,
            TKey key)
            where TKey : notnull
        {
            return valuesByKey.TryGetValue(key, out var values)
                ? values.Order(StringComparer.Ordinal).ToList()
                : [];
        }

        private static HashSet<string> ValuesForMutation<TKey>(
            Dictionary<TKey, HashSet<string>> valuesByKey,
            TKey key)
            where TKey : notnull
        {
            if (!valuesByKey.TryGetValue(key, out var values))
            {
                values = new HashSet<string>(StringComparer.Ordinal);
                valuesByKey[key] = values;
            }

            return values;
        }
    }
}
