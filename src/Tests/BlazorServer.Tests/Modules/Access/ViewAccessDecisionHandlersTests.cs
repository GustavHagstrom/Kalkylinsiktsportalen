using BlazorServer.Identity;
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
        var decisionService = new ViewAccessDecisionHandler(store);
        var subject = Subject(userId: "user-1", organizationId: "org-1", roleIds: ["role-1"]);

        await grantHandler.HandleAsync(new GrantUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView));
        await grantHandler.HandleAsync(new GrantRoleViewAccessCommand("view-1", "role-1", "org-1", ViewAccessRight.EditView));

        var canUse = await decisionService.CanUseViewAsync(subject, "view-1");
        var canEdit = await decisionService.CanEditViewAsync(subject, "view-1");
        var canAdminister = await decisionService.CanAdministerViewAccessAsync(subject, "view-1");

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
        var decisionService = new ViewAccessDecisionHandler(store);
        var subject = Subject(userId: "user-1", organizationId: "org-1");

        var canUse = await decisionService.CanUseViewAsync(subject, "view-1");
        var canEdit = await decisionService.CanEditViewAsync(subject, "view-1");
        var canAdminister = await decisionService.CanAdministerViewAccessAsync(subject, "view-1");

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
        var decisionService = new ViewAccessDecisionHandler(store);
        var admin = Subject(
            userId: "admin-1",
            organizationId: "org-1",
            isAdmin: true);

        var canUse = await decisionService.CanUseViewAsync(admin, "view-1");
        var canEdit = await decisionService.CanEditViewAsync(admin, "view-1");
        var canAdminister = await decisionService.CanAdministerViewAccessAsync(admin, "view-1");

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
        string[]? roleIds = null,
        bool isAdmin = false)
        => new(
            new UserId(userId),
            new OrganizationId(organizationId),
            (roleIds ?? []).ToHashSet(StringComparer.Ordinal),
            isAdmin);
}
