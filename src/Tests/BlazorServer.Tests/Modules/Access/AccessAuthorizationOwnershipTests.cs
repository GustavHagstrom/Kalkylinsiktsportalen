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
            new Claim(AccessAuthorizationClaims.AccessClaimType, "CanDoThing"),
            new Claim(
                AccessAuthorizationClaims.ExternalDirectoryRoleType,
                AccessAuthorizationClaims.ApplicationAdminRoleClaimValue)
        ], "test"));

        Assert.Multiple(() =>
        {
            Assert.That(principal.HasAccessClaimOrApplicationAdmin("CanDoThing"), Is.True);
            Assert.That(principal.IsApplicationAdmin(), Is.True);
        });
    }
}
