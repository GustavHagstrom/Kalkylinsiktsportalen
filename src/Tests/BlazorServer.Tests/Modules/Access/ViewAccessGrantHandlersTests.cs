using BlazorServer.Identity;
using BlazorServer.Modules.Access.Features.GrantRoleViewAccess;
using BlazorServer.Modules.Access.Features.GrantUserViewAccess;
using BlazorServer.Modules.Access.Features.RevokeRoleViewAccess;
using BlazorServer.Modules.Access.Features.RevokeUserViewAccess;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Infrastructure.InMemory;
using BlazorServer.Modules.Access.Persistence;
using BlazorServer.Modules.Access.Public.Authorization;

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
        var grantHandler = new GrantUserViewAccessCommandHandler(store);

        var result = await grantHandler.HandleAsync(
            new GrantUserViewAccessCommand("view-1", "user-1", "org-1", grantedRight),
            CancellationToken.None);

        var decisions = await DecisionsForAllRightsAsync(store, Subject("user-1", "org-1"), "view-1");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<UserViewAccessGranted>());
            AssertOnlyGrantedRightIsAllowed(decisions, grantedRight);
        });
    }

    [TestCase(ViewAccessRight.UseView)]
    [TestCase(ViewAccessRight.EditView)]
    [TestCase(ViewAccessRight.AdministerViewAccess)]
    public async Task ExternalRoleGroupGrantAllowsEachViewRightIndependently(ViewAccessRight grantedRight)
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantHandler = new GrantRoleViewAccessCommandHandler(store);

        var result = await grantHandler.HandleAsync(
            new GrantRoleViewAccessCommand("view-1", "role-1", "org-1", grantedRight),
            CancellationToken.None);

        var decisions = await DecisionsForAllRightsAsync(
            store,
            Subject("user-1", "org-1", ["role-1"]),
            "view-1");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<RoleViewAccessGranted>());
            AssertOnlyGrantedRightIsAllowed(decisions, grantedRight);
        });
    }

    [Test]
    public async Task ViewAccessDecisionsCombineDirectUserAndExternalRoleGroupGrants()
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantUserHandler = new GrantUserViewAccessCommandHandler(store);
        var grantRoleHandler = new GrantRoleViewAccessCommandHandler(store);
        var decisionService = new ViewAccessDecisionHandler(store);
        var subject = Subject("user-1", "org-1", ["role-1"]);

        await grantUserHandler.HandleAsync(
            new GrantUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView),
            CancellationToken.None);
        await grantRoleHandler.HandleAsync(
            new GrantRoleViewAccessCommand("view-1", "role-1", "org-1", ViewAccessRight.EditView),
            CancellationToken.None);

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
    public async Task RevokedViewAccessNoLongerAllowsDecision()
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantHandler = new GrantUserViewAccessCommandHandler(store);
        var revokeHandler = new RevokeUserViewAccessCommandHandler(store);
        var decisionService = new ViewAccessDecisionHandler(store);
        var subject = Subject("user-1", "org-1");

        await grantHandler.HandleAsync(
            new GrantUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView),
            CancellationToken.None);
        await revokeHandler.HandleAsync(
            new RevokeUserViewAccessCommand("view-1", "user-1", "org-1", ViewAccessRight.UseView),
            CancellationToken.None);

        var canUse = await decisionService.CanUseViewAsync(subject, "view-1");

        Assert.That(canUse, Is.False);
    }

    [Test]
    public async Task RevokedRoleViewAccessNoLongerAllowsDecision()
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantHandler = new GrantRoleViewAccessCommandHandler(store);
        var revokeHandler = new RevokeRoleViewAccessCommandHandler(store);
        var decisionService = new ViewAccessDecisionHandler(store);
        var subject = Subject("user-1", "org-1", ["role-1"]);

        await grantHandler.HandleAsync(
            new GrantRoleViewAccessCommand("view-1", "role-1", "org-1", ViewAccessRight.UseView),
            CancellationToken.None);
        var revokeResult = await revokeHandler.HandleAsync(
            new RevokeRoleViewAccessCommand("view-1", "role-1", "org-1", ViewAccessRight.UseView),
            CancellationToken.None);

        var canUse = await decisionService.CanUseViewAsync(subject, "view-1");

        Assert.Multiple(() =>
        {
            Assert.That(revokeResult, Is.TypeOf<RoleViewAccessRevoked>());
            Assert.That(canUse, Is.False);
        });
    }

    [Test]
    public async Task ViewGrantCommandsValidateInputBeforeStoreMutation()
    {
        var store = new InMemoryViewAccessGrantStore();
        var grantUserHandler = new GrantUserViewAccessCommandHandler(store);
        var grantRoleHandler = new GrantRoleViewAccessCommandHandler(store);
        var revokeUserHandler = new RevokeUserViewAccessCommandHandler(store);
        var revokeRoleHandler = new RevokeRoleViewAccessCommandHandler(store);

        var grantUserResult = await grantUserHandler.HandleAsync(
            new GrantUserViewAccessCommand("", "user-1", "org-1", ViewAccessRight.UseView),
            CancellationToken.None);
        var grantRoleResult = await grantRoleHandler.HandleAsync(
            new GrantRoleViewAccessCommand("view-1", "", "org-1", ViewAccessRight.UseView),
            CancellationToken.None);
        var revokeUserResult = await revokeUserHandler.HandleAsync(
            new RevokeUserViewAccessCommand("view-1", "", "org-1", ViewAccessRight.UseView),
            CancellationToken.None);
        var revokeRoleResult = await revokeRoleHandler.HandleAsync(
            new RevokeRoleViewAccessCommand("view-1", "role-1", "", ViewAccessRight.UseView),
            CancellationToken.None);

        var grants = await store.GetGrantsForViewAsync("view-1", "org-1");

        Assert.Multiple(() =>
        {
            Assert.That(grantUserResult, Is.TypeOf<InvalidGrantUserViewAccessRequest>());
            Assert.That(grantRoleResult, Is.TypeOf<InvalidGrantRoleViewAccessRequest>());
            Assert.That(revokeUserResult, Is.TypeOf<InvalidRevokeUserViewAccessRequest>());
            Assert.That(revokeRoleResult, Is.TypeOf<InvalidRevokeRoleViewAccessRequest>());
            Assert.That(grants, Is.Empty);
        });
    }

    private static async Task<IReadOnlyDictionary<ViewAccessRight, bool>> DecisionsForAllRightsAsync(
        IViewAccessGrantStore store,
        AccessSubject subject,
        string viewId)
    {
        var decisionService = new ViewAccessDecisionHandler(store);
        return new Dictionary<ViewAccessRight, bool>
        {
            [ViewAccessRight.UseView] = await decisionService.CanUseViewAsync(subject, viewId),
            [ViewAccessRight.EditView] = await decisionService.CanEditViewAsync(subject, viewId),
            [ViewAccessRight.AdministerViewAccess] =
                await decisionService.CanAdministerViewAccessAsync(subject, viewId)
        };
    }

    private static AccessSubject Subject(
        string userId,
        string organizationId,
        string[]? roleIds = null)
        => new(
            new UserId(userId),
            new OrganizationId(organizationId),
            (roleIds ?? []).ToHashSet(StringComparer.Ordinal),
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
