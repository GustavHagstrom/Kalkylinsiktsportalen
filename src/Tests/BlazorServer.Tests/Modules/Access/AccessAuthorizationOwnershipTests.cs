using BlazorServer.Modules.Access.Authentication;
using System.Security.Claims;

namespace BlazorServer.Tests.Modules.Access;

[TestFixture]
public class AccessAuthorizationOwnershipTests
{
    [Test]
    public void AccessOwnsApplicationAdminAndAccessClaimChecks()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ApplicationClaimTypes.AccessClaim, "CanDoThing"),
            new Claim(ClaimTypes.Role, AccessPolicies.ApplicationAdminRoleClaimValue)
        ], "test"));

        Assert.Multiple(() =>
        {
            Assert.That(principal.HasAccessClaimOrApplicationAdmin("CanDoThing"), Is.True);
            Assert.That(principal.IsApplicationAdmin(), Is.True);
        });
    }
}
