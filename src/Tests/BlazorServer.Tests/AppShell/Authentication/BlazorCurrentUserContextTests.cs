using System.Security.Claims;
using BlazorServer.AppShell.Authentication;
using BlazorServer.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorServer.Tests.AppShell.Authentication;

[TestFixture]
public class BlazorCurrentUserContextTests
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
    public async Task CurrentUserReadsOnlyClaimsFromAuthenticationState()
    {
        var authProvider = new FixedAuthenticationStateProvider(new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(IdentityClaimTypes.UserId, "user-1"),
            new Claim(IdentityClaimTypes.OrganizationId, "org-1"),
            new Claim(ClaimTypes.Name, "Ada Lovelace"),
            new Claim(IdentityClaimTypes.Email, "ada@example.com")
        ], "test")));
        using var currentUserContext = new BlazorCurrentUserContext(authProvider);

        var user = await currentUserContext.GetCurrentUserAsync();

        Assert.Multiple(() =>
        {
            Assert.That(user?.DisplayName, Is.EqualTo("Ada Lovelace"));
            Assert.That(user?.Email, Is.EqualTo("ada@example.com"));
            Assert.That(user?.Avatar, Is.Null);
        });
    }

    [Test]
    public async Task CurrentUserCanBeAwaitedWhenAuthenticationStateIsAsynchronous()
    {
        var authProvider = new DeferredAuthenticationStateProvider();
        using var currentUserContext = new BlazorCurrentUserContext(authProvider);

        authProvider.Complete(Principal("user-1", "Ada Lovelace"));
        var user = await currentUserContext.GetCurrentUserAsync();

        Assert.That(user?.DisplayName, Is.EqualTo("Ada Lovelace"));
    }

    [Test]
    public async Task DisposedContextDoesNotObserveAuthenticationStateChanges()
    {
        var initialPrincipal = Principal("user-1", "Ada Lovelace");
        var changedPrincipal = Principal("user-2", "Grace Hopper");
        var authProvider = new MutableAuthenticationStateProvider(initialPrincipal);
        var currentUserContext = new BlazorCurrentUserContext(authProvider);

        currentUserContext.Dispose();
        await authProvider.SetPrincipalAsync(changedPrincipal);

        var user = await currentUserContext.GetCurrentUserAsync();

        Assert.That(user?.UserId.Value, Is.EqualTo("user-1"));
    }

    [Test]
    public void AuthenticationActionsNavigateToAuthRoutes()
    {
        var navigation = new TestNavigationManager();
        var actions = new AuthenticationActions(navigation);

        actions.Login();
        Assert.That(navigation.Uri, Is.EqualTo("https://example.test/auth/login"));

        actions.Logout();
        Assert.That(navigation.Uri, Is.EqualTo("https://example.test/auth/logout"));
    }

    private static ClaimsPrincipal Principal(string userId, string displayName)
        => new(new ClaimsIdentity(
        [
            new Claim(IdentityClaimTypes.UserId, userId),
            new Claim(IdentityClaimTypes.OrganizationId, "org-1"),
            new Claim(ClaimTypes.Name, displayName),
            new Claim(IdentityClaimTypes.Email, $"{userId}@example.com")
        ], "test"));

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

    private sealed class DeferredAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly TaskCompletionSource<AuthenticationState> state = new();

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return state.Task;
        }

        public void Complete(ClaimsPrincipal principal)
        {
            state.SetResult(new AuthenticationState(principal));
        }
    }

    private sealed class MutableAuthenticationStateProvider : AuthenticationStateProvider
    {
        private AuthenticationState state;

        public MutableAuthenticationStateProvider(ClaimsPrincipal principal)
        {
            state = new AuthenticationState(principal);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(state);
        }

        public async Task SetPrincipalAsync(ClaimsPrincipal principal)
        {
            state = new AuthenticationState(principal);
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            await Task.Yield();
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
