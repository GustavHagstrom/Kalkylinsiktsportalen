using BlazorServer.Modules.Access.Features.ViewAccessGrants;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class ViewAccessGrantHandlersTests
{
    [TestCase(ViewAccessRight.UseView)]
    [TestCase(ViewAccessRight.EditView)]
    [TestCase(ViewAccessRight.AdministerViewAccess)]
    public async Task DirectUserGrantAllowsEachViewRightIndependently(ViewAccessRight grantedRight)
    {
        var store = new InMemoryViewAccessGrantStore();
        var grant = new GrantUserViewAccessHandler(store);

        await grant.HandleAsync(new GrantUserViewAccessCommand("view-1", "user-1", "org-1", grantedRight));

        var decisions = await DecisionsForAllRightsAsync(store, Principal("user-1", "org-1"), "view-1");

        AssertOnlyGrantedRightIsAllowed(decisions, grantedRight);
    }

    [TestCase(ViewAccessRight.UseView)]
    [TestCase(ViewAccessRight.EditView)]
    [TestCase(ViewAccessRight.AdministerViewAccess)]
    public async Task ExternalRoleGroupGrantAllowsEachViewRightIndependently(ViewAccessRight grantedRight)
    {
        var store = new InMemoryViewAccessGrantStore();
        var grant = new GrantRoleViewAccessHandler(store);

        await grant.HandleAsync(new GrantRoleViewAccessCommand("view-1", "role-1", "org-1", grantedRight));

        var decisions = await DecisionsForAllRightsAsync(store, Principal("user-1", "org-1", ["role-1"]), "view-1");

        AssertOnlyGrantedRightIsAllowed(decisions, grantedRight);
    }

    [Test]
    public async Task ViewAccessDecisionsCombineDirectUserAndExternalRoleGroupGrants()
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantUser = new GrantUserViewAccessHandler(store);
        var grantRole = new GrantRoleViewAccessHandler(store);
        var canUseHandler = new CanUseViewHandler(store);
        var canEditHandler = new CanEditViewHandler(store);
        var canAdministerHandler = new CanAdministerViewAccessHandler(store);
        var user = Principal("user-1", "org-1", ["role-1"]);

        await grantUser.HandleAsync(new GrantUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView));
        await grantRole.HandleAsync(new GrantRoleViewAccessCommand("view-1", "role-1", "org-1", ViewAccessRight.EditView));

        var canUse = await canUseHandler.HandleAsync(new CanUseViewQuery(user, "view-1"));
        var canEdit = await canEditHandler.HandleAsync(new CanEditViewQuery(user, "view-1"));
        var canAdminister = await canAdministerHandler.HandleAsync(new CanAdministerViewAccessQuery(user, "view-1"));

        Assert.Multiple(() =>
        {
            Assert.That(canUse, Is.True);
            Assert.That(canEdit, Is.True);
            Assert.That(canAdminister, Is.False);
        });
    }

    [Test]
    public async Task RevokedViewAccessNoLongerAllowsDecision()
    {
        var store = new InMemoryViewAccessGrantStore();
        var grant = new GrantUserViewAccessHandler(store);
        var revoke = new RevokeUserViewAccessHandler(store);
        var canUseHandler = new CanUseViewHandler(store);
        var user = Principal("user-1", "org-1");

        await grant.HandleAsync(new GrantUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView));
        await revoke.HandleAsync(new RevokeUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView));

        var canUse = await canUseHandler.HandleAsync(new CanUseViewQuery(user, "view-1"));

        Assert.That(canUse, Is.False);
    }

    private static async Task<IReadOnlyDictionary<ViewAccessRight, bool>> DecisionsForAllRightsAsync(
        IViewAccessGrantStore store,
        System.Security.Claims.ClaimsPrincipal user,
        string viewId)
    {
        var canUseHandler = new CanUseViewHandler(store);
        var canEditHandler = new CanEditViewHandler(store);
        var canAdministerHandler = new CanAdministerViewAccessHandler(store);
        var decisions = new Dictionary<ViewAccessRight, bool>();
        decisions[ViewAccessRight.UseView] = await canUseHandler.HandleAsync(new CanUseViewQuery(user, viewId));
        decisions[ViewAccessRight.EditView] = await canEditHandler.HandleAsync(new CanEditViewQuery(user, viewId));
        decisions[ViewAccessRight.AdministerViewAccess] =
            await canAdministerHandler.HandleAsync(new CanAdministerViewAccessQuery(user, viewId));

        return decisions;
    }

    private static System.Security.Claims.ClaimsPrincipal Principal(
        string userId,
        string organizationId,
        IReadOnlyCollection<string>? roleIds = null)
    {
        var claims = new List<System.Security.Claims.Claim>
        {
            new(Microsoft.Identity.Web.ClaimConstants.ObjectId, userId),
            new(Microsoft.Identity.Web.ClaimConstants.TenantId, organizationId)
        };

        claims.AddRange((roleIds ?? []).Select(roleId => new System.Security.Claims.Claim("groups", roleId)));

        return new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims, "test"));
    }

    private static void AssertOnlyGrantedRightIsAllowed(
        IReadOnlyDictionary<ViewAccessRight, bool> decisions,
        ViewAccessRight grantedRight)
    {
        Assert.Multiple(() =>
        {
            foreach (var (right, isAllowed) in decisions)
            {
                Assert.That(isAllowed, Is.EqualTo(right == grantedRight), right.ToString());
            }
        });
    }
}
