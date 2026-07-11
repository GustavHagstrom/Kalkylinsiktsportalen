using System.Security.Claims;
using BlazorServer.AppShell.Authentication;
using Microsoft.Identity.Web;

namespace BlazorServer.Tests.AppShell.Authentication;

[TestFixture]
public class CurrentUserSessionTests
{
    [Test]
    public void IdentityClaimsMapToCurrentUserProjection()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimConstants.ObjectId, "user-1"),
            new Claim(ClaimConstants.TenantId, "org-1"),
            new Claim(ClaimTypes.Name, "Ada Lovelace"),
            new Claim(CurrentUserSessionClaimTypes.Email, "ada@example.com")
        ], "test"));

        var currentUser = principal.ToCurrentUser();

        Assert.Multiple(() =>
        {
            Assert.That(currentUser, Is.Not.Null);
            Assert.That(currentUser!.UserId, Is.EqualTo("user-1"));
            Assert.That(currentUser.DisplayName, Is.EqualTo("Ada Lovelace"));
            Assert.That(currentUser.Email, Is.EqualTo("ada@example.com"));
            Assert.That(principal.GetOrganizationId(), Is.EqualTo("org-1"));
        });
    }
}
