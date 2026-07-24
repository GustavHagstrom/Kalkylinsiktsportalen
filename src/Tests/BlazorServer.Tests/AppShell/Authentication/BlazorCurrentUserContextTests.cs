using System.Security.Claims;
using BlazorServer.AppShell.Authentication;
using BlazorServer.AppShell.Authentication.Entra;
using BlazorServer.AppShell.Authentication.Mock;
using BlazorServer.Composition;
using BlazorServer.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;

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
        var currentUserContext = new BlazorCurrentUserContext(authProvider);

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
        var currentUserContext = new BlazorCurrentUserContext(authProvider);

        authProvider.Complete(Principal("user-1", "Ada Lovelace"));
        var user = await currentUserContext.GetCurrentUserAsync();

        Assert.That(user?.DisplayName, Is.EqualTo("Ada Lovelace"));
    }

    [Test]
    public async Task CurrentUserReflectsLatestAuthenticationStateProviderValue()
    {
        var initialPrincipal = Principal("user-1", "Ada Lovelace");
        var changedPrincipal = Principal("user-2", "Grace Hopper");
        var authProvider = new MutableAuthenticationStateProvider(initialPrincipal);
        var currentUserContext = new BlazorCurrentUserContext(authProvider);

        await authProvider.SetPrincipalAsync(changedPrincipal);

        var user = await currentUserContext.GetCurrentUserAsync();

        Assert.That(user?.UserId.Value, Is.EqualTo("user-2"));
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

    [Test]
    public void MockAuthenticationRegistrationKeepsMockServicesTogether()
    {
        var services = new ServiceCollection();
        services.AddSingleton<NavigationManager>(new TestNavigationManager());
        var registrationContext = new AppRegistrationContext();

        services.AddMockAuthentication("org-1", registrationContext);

        using var provider = services.BuildServiceProvider();
        var loginDisplay = provider.GetRequiredService<LoginDisplayRecord>();

        Assert.Multiple(() =>
        {
            Assert.That(loginDisplay.ComponentType, Is.EqualTo(typeof(MockedLoginDisplay)));
            Assert.That(provider.GetRequiredService<MockAuthenticationState>(), Is.Not.Null);
            Assert.That(provider.GetRequiredService<MockCurrentUserSwitcher>(), Is.Not.Null);
            Assert.That(provider.GetRequiredService<ICurrentUserContext>(), Is.TypeOf<MockCurrentUserContext>());
            Assert.That(provider.GetRequiredService<IAuthenticationActions>(), Is.TypeOf<AuthenticationActions>());
        });
    }

    [Test]
    public void EntraAuthenticationRegistrationKeepsEntraServicesTogether()
    {
        var services = new ServiceCollection();
        var registrationContext = new AppRegistrationContext();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureAd:Instance"] = "https://login.microsoftonline.com/",
                ["AzureAd:TenantId"] = "tenant",
                ["AzureAd:ClientId"] = "client",
                ["DownstreamApi:Scopes"] = "User.Read"
            })
            .Build();

        services.AddEntraAuthentication(configuration, registrationContext);

        var loginDisplay = services.Single(descriptor => descriptor.ServiceType == typeof(LoginDisplayRecord));
        var graphProvider = services.Single(descriptor => descriptor.ServiceType == typeof(IAuthenticationProvider));

        Assert.Multiple(() =>
        {
            Assert.That(((LoginDisplayRecord)loginDisplay.ImplementationInstance!).ComponentType, Is.EqualTo(typeof(LoginDisplay)));
            Assert.That(graphProvider.ImplementationType?.FullName, Is.EqualTo("BlazorServer.AppShell.Authentication.Entra.GraphClientAuthProvider"));
            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(BlazorCurrentUserContext)), Is.True);
            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(ICurrentUserContext)), Is.True);
            Assert.That(services.Any(descriptor => descriptor.ServiceType == typeof(IAuthenticationActions)), Is.True);
        });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("WindowsIntegrated")]
    public void AppShellAuthenticationRegistrationRejectsUnsupportedAuthenticationMode(string? mode)
    {
        var services = new ServiceCollection();
        var registrationContext = new AppRegistrationContext();
        var values = new Dictionary<string, string?>();
        if (mode is not null)
        {
            values["Auth:Mode"] = mode;
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddAppShellAuthentication(configuration, registrationContext));

        Assert.That(exception!.Message, Does.Contain("Unsupported authentication mode"));
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
