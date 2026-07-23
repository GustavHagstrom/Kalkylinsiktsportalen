using System.Security.Claims;
using BlazorServer.Modules.Access.Public.Authorization;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class AccessAuthorizationOwnershipTests
{
    [Test]
    public void AccessOwnsApplicationAdminAndAccessClaimChecks()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(AccessAuthorizationConstants.AccessClaimType, "CanDoThing"),
            new Claim(
                AccessAuthorizationConstants.ExternalDirectoryRoleClaimType,
                AccessAuthorizationConstants.ApplicationAdminRoleClaimValue)
        ], "test"));

        Assert.Multiple(() =>
        {
            Assert.That(principal.HasAccessClaimOrApplicationAdmin("CanDoThing"), Is.True);
            Assert.That(principal.IsApplicationAdmin(), Is.True);
        });
    }
}
