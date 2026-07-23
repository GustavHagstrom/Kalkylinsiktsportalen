using System.Security.Claims;
using BlazorServer.AppShell.Authentication;
using BlazorServer.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorServer.Tests.AppShell.Authentication;

[TestFixture]
public class CurrentUserSessionTests
{
    [Test]
    public void IdentityClaimsMapToCurrentUserProjection()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(IdentityClaimTypes.UserId, "user-1"),
            new Claim(IdentityClaimTypes.OrganizationId, "org-1"),
            new Claim(ClaimTypes.Name, "Ada Lovelace"),
            new Claim(IdentityClaimTypes.Email, "ada@example.com")
        ], "test"));

        var currentUser = principal.ToCurrentUser();

        Assert.Multiple(() =>
        {
            Assert.That(currentUser, Is.Not.Null);
            Assert.That(currentUser!.UserId.Value, Is.EqualTo("user-1"));
            Assert.That(currentUser.DisplayName, Is.EqualTo("Ada Lovelace"));
            Assert.That(currentUser.Email, Is.EqualTo("ada@example.com"));
            Assert.That(principal.GetOrganizationId(), Is.EqualTo("org-1"));
        });
    }

    [Test]
    public async Task CurrentUserSessionUsesProfileLookupForAvatar()
    {
        var authProvider = new FixedAuthenticationStateProvider(new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(IdentityClaimTypes.UserId, "user-1"),
            new Claim(IdentityClaimTypes.OrganizationId, "org-1"),
            new Claim(ClaimTypes.Name, "Ada Lovelace"),
            new Claim(IdentityClaimTypes.Email, "ada@example.com")
        ], "test")));
        var profileLookup = new FixedProfileLookup(new CurrentUser(
            new("user-1"),
            new("org-1"), 
            "Ada Lovelace",
            "ada@example.com",
            "photo-base64"));
        using var session = new CurrentUserSession(
            authProvider,
            new TestNavigationManager(),
            profileLookup);

        var user = await session.RefreshAsync();

        Assert.Multiple(() =>
        {
            Assert.That(user?.DisplayName, Is.EqualTo("Ada Lovelace"));
            Assert.That(user?.Avatar, Is.EqualTo("photo-base64"));
        });
    }

    private sealed class FixedAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationState state;

        public FixedAuthenticationStateProvider(ClaimsPrincipal principal)
        {
            state = new AuthenticationState(principal);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(state);
        }
    }

    private sealed class FixedProfileLookup : ICurrentUserProfileLookup
    {
        private readonly CurrentUser user;

        public FixedProfileLookup(CurrentUser user)
        {
            this.user = user;
        }

        public Task<CurrentUser> GetProfileAsync(CurrentUser currentUser)
        {
            return Task.FromResult(user);
        }
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("https://example.test/", "https://example.test/");
        }

        protected override void NavigateToCore(string uri, NavigationOptions options)
        {
            Uri = ToAbsoluteUri(uri).ToString();
        }
    }
}
