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
        var grantHandler = new ViewAccessGrantHandler(store);

        await grantHandler.HandleAsync(new GrantUserViewAccessCommand("view-1", "user-1", "org-1", grantedRight));

        var decisions = await DecisionsForAllRightsAsync(store, Subject("user-1", "org-1"), "view-1");

        AssertOnlyGrantedRightIsAllowed(decisions, grantedRight);
    }

    [TestCase(ViewAccessRight.UseView)]
    [TestCase(ViewAccessRight.EditView)]
    [TestCase(ViewAccessRight.AdministerViewAccess)]
    public async Task ExternalRoleGroupGrantAllowsEachViewRightIndependently(ViewAccessRight grantedRight)
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantHandler = new ViewAccessGrantHandler(store);

        await grantHandler.HandleAsync(new GrantRoleViewAccessCommand("view-1", "role-1", "org-1", grantedRight));

        var decisions = await DecisionsForAllRightsAsync(
            store,
            Subject("user-1", "org-1", ["role-1"]),
            "view-1");

        AssertOnlyGrantedRightIsAllowed(decisions, grantedRight);
    }

    [Test]
    public async Task ViewAccessDecisionsCombineDirectUserAndExternalRoleGroupGrants()
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantHandler = new ViewAccessGrantHandler(store);
        var decisionHandler = new ViewAccessDecisionHandler(store);
        var subject = Subject("user-1", "org-1", ["role-1"]);

        await grantHandler.HandleAsync(new GrantUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView));
        await grantHandler.HandleAsync(new GrantRoleViewAccessCommand("view-1", "role-1", "org-1", ViewAccessRight.EditView));

        var canUse = await decisionHandler.HandleAsync(new CanUseViewQuery(subject, "view-1"));
        var canEdit = await decisionHandler.HandleAsync(new CanEditViewQuery(subject, "view-1"));
        var canAdminister = await decisionHandler.HandleAsync(new CanAdministerViewAccessQuery(subject, "view-1"));

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
        var grantHandler = new ViewAccessGrantHandler(store);
        var decisionHandler = new ViewAccessDecisionHandler(store);
        var subject = Subject("user-1", "org-1");

        await grantHandler.HandleAsync(new GrantUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView));
        await grantHandler.HandleAsync(new RevokeUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView));

        var canUse = await decisionHandler.HandleAsync(new CanUseViewQuery(subject, "view-1"));

        Assert.That(canUse, Is.False);
    }

    private static async Task<IReadOnlyDictionary<ViewAccessRight, bool>> DecisionsForAllRightsAsync(
        IViewAccessGrantStore store,
        AccessSubject subject,
        string viewId)
    {
        var decisionHandler = new ViewAccessDecisionHandler(store);
        var decisions = new Dictionary<ViewAccessRight, bool>
        {
            [ViewAccessRight.UseView] = await decisionHandler.HandleAsync(new CanUseViewQuery(subject, viewId)),
            [ViewAccessRight.EditView] = await decisionHandler.HandleAsync(new CanEditViewQuery(subject, viewId)),
            [ViewAccessRight.AdministerViewAccess] =
                await decisionHandler.HandleAsync(new CanAdministerViewAccessQuery(subject, viewId))
        };

        return decisions;
    }

    private static AccessSubject Subject(
        string userId,
        string organizationId,
        IReadOnlySet<string>? roleIds = null)
        => new(
            userId,
            organizationId,
            roleIds ?? new HashSet<string>(StringComparer.Ordinal),
            false);

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
