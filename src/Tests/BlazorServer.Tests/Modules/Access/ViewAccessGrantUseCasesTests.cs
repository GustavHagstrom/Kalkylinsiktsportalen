using BlazorServer.Modules.Access.Features.ViewAccessGrants;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class ViewAccessGrantUseCasesTests
{
    [TestCase(ViewAccessRight.UseView)]
    [TestCase(ViewAccessRight.EditView)]
    [TestCase(ViewAccessRight.AdministerViewAccess)]
    public async Task DirectUserGrantAllowsEachViewRightIndependently(ViewAccessRight grantedRight)
    {
        var useCases = new ViewAccessGrantUseCases(new InMemoryViewAccessGrantStore());

        await useCases.GrantUserViewAccessAsync("view-1", "user-1", "org-1", grantedRight);

        var decisions = await DecisionsForAllRightsAsync(useCases, "view-1", "user-1", [], "org-1");

        AssertOnlyGrantedRightIsAllowed(decisions, grantedRight);
    }

    [TestCase(ViewAccessRight.UseView)]
    [TestCase(ViewAccessRight.EditView)]
    [TestCase(ViewAccessRight.AdministerViewAccess)]
    public async Task ExternalRoleGroupGrantAllowsEachViewRightIndependently(ViewAccessRight grantedRight)
    {
        var useCases = new ViewAccessGrantUseCases(new InMemoryViewAccessGrantStore());

        await useCases.GrantRoleViewAccessAsync("view-1", "role-1", "org-1", grantedRight);

        var decisions = await DecisionsForAllRightsAsync(useCases, "view-1", "user-1", ["role-1"], "org-1");

        AssertOnlyGrantedRightIsAllowed(decisions, grantedRight);
    }

    [Test]
    public async Task ViewAccessDecisionsCombineDirectUserAndExternalRoleGroupGrants()
    {
        var useCases = new ViewAccessGrantUseCases(new InMemoryViewAccessGrantStore());

        await useCases.GrantUserViewAccessAsync("view-1", "user-1", "org-1", ViewAccessRight.UseView);
        await useCases.GrantRoleViewAccessAsync("view-1", "role-1", "org-1", ViewAccessRight.EditView);

        var canUse = await useCases.HasViewAccessAsync("view-1", "user-1", ["role-1"], "org-1", ViewAccessRight.UseView);
        var canEdit = await useCases.HasViewAccessAsync("view-1", "user-1", ["role-1"], "org-1", ViewAccessRight.EditView);
        var canAdminister = await useCases.HasViewAccessAsync(
            "view-1",
            "user-1",
            ["role-1"],
            "org-1",
            ViewAccessRight.AdministerViewAccess);

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
        var useCases = new ViewAccessGrantUseCases(new InMemoryViewAccessGrantStore());

        await useCases.GrantUserViewAccessAsync("view-1", "user-1", "org-1", ViewAccessRight.UseView);
        await useCases.RevokeUserViewAccessAsync("view-1", "user-1", "org-1", ViewAccessRight.UseView);

        var canUse = await useCases.HasViewAccessAsync("view-1", "user-1", [], "org-1", ViewAccessRight.UseView);

        Assert.That(canUse, Is.False);
    }

    private static async Task<IReadOnlyDictionary<ViewAccessRight, bool>> DecisionsForAllRightsAsync(
        ViewAccessGrantUseCases useCases,
        string viewId,
        string userId,
        IReadOnlyCollection<string> roleIds,
        string organizationId)
    {
        var decisions = new Dictionary<ViewAccessRight, bool>();
        foreach (var right in Enum.GetValues<ViewAccessRight>())
        {
            decisions[right] = await useCases.HasViewAccessAsync(viewId, userId, roleIds, organizationId, right);
        }

        return decisions;
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
