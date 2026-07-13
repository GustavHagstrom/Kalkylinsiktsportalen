using System.Security.Claims;
using BlazorServer.AppShell.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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

        Assert.That(currentUser, Is.Not.Null);
        Assert.That(currentUser!.Id, Is.EqualTo("user-1"));
        Assert.That(currentUser.Name, Is.EqualTo("Ada Lovelace"));
        Assert.That(currentUser.Email, Is.EqualTo("ada@example.com"));
    }

    [Test]
    public async Task RefreshAsyncUpdatesCurrentSessionProjection()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimConstants.ObjectId, "user-1"),
            new Claim(ClaimConstants.TenantId, "org-1"),
            new Claim(ClaimTypes.Name, "Ada Lovelace")
        ], "test"));
        var authenticationStateProvider = new TestAuthenticationStateProvider(principal);
        var profileLookup = new TestCurrentUserProfileLookup();
        var session = new CurrentUserSession(authenticationStateProvider, new TestNavigationManager(), profileLookup);

        var currentUser = await session.RefreshAsync();

        Assert.That(currentUser, Is.Not.Null);
        Assert.That(session.OrganizationId, Is.EqualTo("org-1"));
        Assert.That(session.IsAuthenticated, Is.True);
        Assert.That(session.User?.Id, Is.EqualTo("user-1"));
    }

    private sealed class TestAuthenticationStateProvider(ClaimsPrincipal principal) : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(principal));
    }

    private sealed class TestCurrentUserProfileLookup : ICurrentUserProfileLookup
    {
        public Task<SessionUser?> GetProfileAsync(CurrentUser user)
            => Task.FromResult<SessionUser?>(new SessionUser(user.Id, user.Name, user.Email, []));
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("https://localhost/", "https://localhost/");
        }

        protected override void NavigateToCore(string uri, NavigationOptions options)
        {
        }
    }
}
