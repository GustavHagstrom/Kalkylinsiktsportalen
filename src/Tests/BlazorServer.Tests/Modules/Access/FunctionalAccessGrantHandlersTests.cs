using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class FunctionalAccessGrantHandlersTests
{
    [Test]
    public async Task UserAccessGrantsCanBeReadGrantedRevokedAndGrantedIdempotently()
    {
        var store = new RecordingAccessStore();
        var grant = new GrantUserFunctionalAccessHandler(store);
        var revoke = new RevokeUserFunctionalAccessHandler(store);
        var get = new GetUserFunctionalAccessHandler(store);

        await grant.HandleAsync(new GrantUserFunctionalAccessCommand("user-1", "org-1", "CanManageAccess"));
        await grant.HandleAsync(new GrantUserFunctionalAccessCommand("user-1", "org-1", "CanManageAccess"));

        var granted = await get.HandleAsync(new GetUserFunctionalAccessQuery("user-1", "org-1"));

        await revoke.HandleAsync(new RevokeUserFunctionalAccessCommand("user-1", "org-1", "CanManageAccess"));
        var revoked = await get.HandleAsync(new GetUserFunctionalAccessQuery("user-1", "org-1"));

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
        var grant = new GrantRoleFunctionalAccessHandler(store);
        var revoke = new RevokeRoleFunctionalAccessHandler(store);
        var get = new GetRoleFunctionalAccessHandler(store);

        await grant.HandleAsync(new GrantRoleFunctionalAccessCommand("role-1", "org-1", "CanManageAccess"));
        await grant.HandleAsync(new GrantRoleFunctionalAccessCommand("role-1", "org-1", "CanManageAccess"));

        var granted = await get.HandleAsync(new GetRoleFunctionalAccessQuery("role-1", "org-1"));

        await revoke.HandleAsync(new RevokeRoleFunctionalAccessCommand("role-1", "org-1", "CanManageAccess"));
        var revoked = await get.HandleAsync(new GetRoleFunctionalAccessQuery("role-1", "org-1"));

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
        var grantUser = new GrantUserFunctionalAccessHandler(store);
        var grantRole = new GrantRoleFunctionalAccessHandler(store);
        var getEffectiveClaims = new GetEffectiveFunctionalAccessClaimsHandler(store);

        await grantUser.HandleAsync(new GrantUserFunctionalAccessCommand("user-1", "org-1", "DirectAccess"));
        await grantRole.HandleAsync(new GrantRoleFunctionalAccessCommand("role-1", "org-1", "GroupAccess"));
        await grantRole.HandleAsync(new GrantRoleFunctionalAccessCommand("role-2", "org-1", "GroupAccess"));

        var effectiveClaims = await getEffectiveClaims.HandleAsync(new GetEffectiveFunctionalAccessClaimsQuery(
            "user-1",
            ["role-1", "role-2"],
            "org-1"));

        Assert.That(effectiveClaims.Select(claim => claim.Value), Is.EquivalentTo([
            "DirectAccess",
            "GroupAccess"
        ]));
    }

    private sealed class RecordingAccessStore : IFunctionalAccessGrantStore
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
