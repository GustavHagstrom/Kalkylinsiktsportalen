using BlazorServer.Modules.Access.Authentication;
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
        var handler = new FunctionalAccessGrantHandler(store);

        await handler.HandleAsync(new GrantUserFunctionalAccessCommand("user-1", "org-1", "CanManageAccess"));
        await handler.HandleAsync(new GrantUserFunctionalAccessCommand("user-1", "org-1", "CanManageAccess"));

        var granted = await handler.HandleAsync(new GetUserFunctionalAccessQuery("user-1", "org-1"));

        await handler.HandleAsync(new RevokeUserFunctionalAccessCommand("user-1", "org-1", "CanManageAccess"));
        var revoked = await handler.HandleAsync(new GetUserFunctionalAccessQuery("user-1", "org-1"));

        Assert.Multiple(() =>
        {
            Assert.That(granted.AccessClaimValues, Is.EqualTo(new[] { "CanManageAccess" }));
            Assert.That(granted.Target, Is.EqualTo(FunctionalAccessGrantTarget.User("user-1")));
            Assert.That(revoked.AccessClaimValues, Is.Empty);
            Assert.That(store.GrantCalls, Is.EqualTo(2));
            Assert.That(store.RevokeCalls, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task RoleAccessGrantsCanBeReadGrantedRevokedAndGrantedIdempotently()
    {
        var store = new RecordingAccessStore();
        var handler = new FunctionalAccessGrantHandler(store);

        await handler.HandleAsync(new GrantRoleFunctionalAccessCommand("role-1", "org-1", "CanManageAccess"));
        await handler.HandleAsync(new GrantRoleFunctionalAccessCommand("role-1", "org-1", "CanManageAccess"));

        var granted = await handler.HandleAsync(new GetRoleFunctionalAccessQuery("role-1", "org-1"));

        await handler.HandleAsync(new RevokeRoleFunctionalAccessCommand("role-1", "org-1", "CanManageAccess"));
        var revoked = await handler.HandleAsync(new GetRoleFunctionalAccessQuery("role-1", "org-1"));

        Assert.Multiple(() =>
        {
            Assert.That(granted.AccessClaimValues, Is.EqualTo(new[] { "CanManageAccess" }));
            Assert.That(granted.Target, Is.EqualTo(FunctionalAccessGrantTarget.Role("role-1")));
            Assert.That(revoked.AccessClaimValues, Is.Empty);
            Assert.That(store.GrantCalls, Is.EqualTo(2));
            Assert.That(store.RevokeCalls, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task RuntimeAccessCombinesDirectUserAndExternalRoleGroupGrants()
    {
        var store = new RecordingAccessStore();
        var handler = new FunctionalAccessGrantHandler(store);

        await handler.HandleAsync(new GrantUserFunctionalAccessCommand("user-1", "org-1", "DirectAccess"));
        await handler.HandleAsync(new GrantRoleFunctionalAccessCommand("role-1", "org-1", "GroupAccess"));
        await handler.HandleAsync(new GrantRoleFunctionalAccessCommand("role-2", "org-1", "GroupAccess"));

        var effectiveClaims = await handler.HandleAsync(new GetEffectiveFunctionalAccessClaimsQuery(
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
        private readonly Dictionary<(FunctionalAccessGrantTarget Target, string OrgId), HashSet<string>> accessGrants = [];

        public int GrantCalls { get; private set; }
        public int RevokeCalls { get; private set; }

        public Task<FunctionalAccessGrant> GetAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId)
        {
            return Task.FromResult(new FunctionalAccessGrant(
                target,
                orgId,
                ValuesFor(accessGrants, (target, orgId))));
        }

        public Task<List<AccessClaim>> GetAccessClaimsForUserAsync(string userId, IEnumerable<string> roleIds, string orgId)
        {
            var values = ValuesFor(accessGrants, (FunctionalAccessGrantTarget.User(userId), orgId))
                .ToHashSet(StringComparer.Ordinal);
            foreach (var roleId in roleIds)
            {
                values.UnionWith(ValuesFor(accessGrants, (FunctionalAccessGrantTarget.Role(roleId), orgId)));
            }

            return Task.FromResult(values.Select(value => new AccessClaim(value)).ToList());
        }

        public Task AddAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId, string accessClaimValue)
        {
            GrantCalls++;
            ValuesForMutation(accessGrants, (target, orgId)).Add(accessClaimValue);
            return Task.CompletedTask;
        }

        public Task DeleteAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId, string accessClaimValue)
        {
            RevokeCalls++;
            ValuesForMutation(accessGrants, (target, orgId)).Remove(accessClaimValue);
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
