using System.Security.Claims;
using BlazorServer.Modules.Access;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Shared.Constants;
using Microsoft.Identity.Web;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class ViewAccessDecisionUseCasesTests
{
    [Test]
    public async Task ViewFlowAllowsUseEditAndAdministerOnlyWhenMatchingViewGrantExists()
    {
        var grants = new ViewAccessGrantUseCases(new InMemoryViewAccessGrantStore());
        var decisions = new ViewAccessDecisionUseCases(grants);
        var user = Principal(userId: "user-1", organizationId: "org-1", roleIds: ["role-1"]);

        await grants.GrantUserViewAccessAsync("view-1", "user-1", "org-1", ViewAccessRight.UseView);
        await grants.GrantRoleViewAccessAsync("view-1", "role-1", "org-1", ViewAccessRight.EditView);

        var canUse = await decisions.CanUseViewAsync(user, "view-1");
        var canEdit = await decisions.CanEditViewAsync(user, "view-1");
        var canAdminister = await decisions.CanAdministerViewAccessAsync(user, "view-1");

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
        var grants = new ViewAccessGrantUseCases(new InMemoryViewAccessGrantStore());
        var decisions = new ViewAccessDecisionUseCases(grants);
        var user = Principal(
            userId: "user-1",
            organizationId: "org-1",
            accessClaims: [BlazorServer.Modules.Access.Constants.Authorization.AccessClaimValue]);

        var canUse = await decisions.CanUseViewAsync(user, "view-1");
        var canEdit = await decisions.CanEditViewAsync(user, "view-1");
        var canAdminister = await decisions.CanAdministerViewAccessAsync(user, "view-1");

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
        var grants = new ViewAccessGrantUseCases(new InMemoryViewAccessGrantStore());
        var decisions = new ViewAccessDecisionUseCases(grants);
        var admin = Principal(
            userId: "admin-1",
            organizationId: "org-1",
            roleIds: [],
            appRoles: [AuthConstants.AdminRoleClaimValue]);

        var canUse = await decisions.CanUseViewAsync(admin, "view-1");
        var canEdit = await decisions.CanEditViewAsync(admin, "view-1");
        var canAdminister = await decisions.CanAdministerViewAccessAsync(admin, "view-1");

        Assert.Multiple(() =>
        {
            Assert.That(canUse, Is.True);
            Assert.That(canEdit, Is.True);
            Assert.That(canAdminister, Is.True);
        });
    }

    private static ClaimsPrincipal Principal(
        string userId,
        string organizationId,
        IReadOnlyCollection<string>? roleIds = null,
        IReadOnlyCollection<string>? accessClaims = null,
        IReadOnlyCollection<string>? appRoles = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimConstants.ObjectId, userId),
            new(ClaimConstants.TenantId, organizationId)
        };

        claims.AddRange((roleIds ?? []).Select(roleId => new Claim("groups", roleId)));
        claims.AddRange((accessClaims ?? []).Select(value => new Claim(ApplicationClaimTypes.AccessClaim, value)));
        claims.AddRange((appRoles ?? []).Select(role => new Claim(ClaimTypes.Role, role)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }
}
