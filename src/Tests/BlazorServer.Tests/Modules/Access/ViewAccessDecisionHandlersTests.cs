using BlazorServer.Modules.Access;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Public.Authorization;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class ViewAccessDecisionHandlersTests
{
    [Test]
    public async Task ViewFlowAllowsUseEditAndAdministerOnlyWhenMatchingViewGrantExists()
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantHandler = new ViewAccessGrantHandler(store);
        var decisionHandler = new ViewAccessDecisionHandler(store);
        var subject = Subject(userId: "user-1", organizationId: "org-1", roleIds: ["role-1"]);

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
    public async Task GlobalViewSettingsFeatureAccessDoesNotImplyPerViewAccess()
    {
        var store = new InMemoryViewAccessGrantStore();
        var decisionHandler = new ViewAccessDecisionHandler(store);
        var subject = Subject(userId: "user-1", organizationId: "org-1");

        var canUse = await decisionHandler.HandleAsync(new CanUseViewQuery(subject, "view-1"));
        var canEdit = await decisionHandler.HandleAsync(new CanEditViewQuery(subject, "view-1"));
        var canAdminister = await decisionHandler.HandleAsync(new CanAdministerViewAccessQuery(subject, "view-1"));

        Assert.Multiple(() =>
        {
            Assert.That(canUse, Is.False);
            Assert.That(canEdit, Is.False);
            Assert.That(canAdminister, Is.False);
        });
    }

    [Test]
    public async Task AdminBypassExplicitlyAllowsViewFlowsWithoutStoredGrant()
    {
        var store = new InMemoryViewAccessGrantStore();
        var decisionHandler = new ViewAccessDecisionHandler(store);
        var admin = Subject(
            userId: "admin-1",
            organizationId: "org-1",
            isAdmin: true);

        var canUse = await decisionHandler.HandleAsync(new CanUseViewQuery(admin, "view-1"));
        var canEdit = await decisionHandler.HandleAsync(new CanEditViewQuery(admin, "view-1"));
        var canAdminister = await decisionHandler.HandleAsync(new CanAdministerViewAccessQuery(admin, "view-1"));

        Assert.Multiple(() =>
        {
            Assert.That(canUse, Is.True);
            Assert.That(canEdit, Is.True);
            Assert.That(canAdminister, Is.True);
        });
    }

    private static AccessSubject Subject(
        string userId,
        string organizationId,
        IReadOnlySet<string>? roleIds = null,
        bool isAdmin = false)
        => new(
            userId,
            organizationId,
            roleIds ?? new HashSet<string>(StringComparer.Ordinal),
            isAdmin);
}
