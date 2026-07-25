using BlazorServer.Composition.Navigation;
using BlazorServer.Composition.Settings;
using System.Security.Claims;
using BlazorServer.Modules.Access;
using BlazorServer.Modules.Access.Public.Authorization;
using BlazorServer.AppShell.Settings;
using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Modules.Bidcon;
using BlazorServer.Modules.Bidcon.AccessTokens;
using BlazorServer.Modules.Bidcon.Components;
using BlazorServer.Modules.Bidcon.CredentialAdministration;
using BlazorServer.Modules.Bidcon.Features.GetEstimationList;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Authentication;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Modules.Bidcon.Public;
using BlazorServer.Modules.Bidcon.Public.Components;
using BlazorServer.Modules.Bidcon.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using BlazorServer.Composition;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class BidconModuleRegistrationTests
{
    [Test]
    public void ServicesCanRegisterBidconModule()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();

        services.AddBidconModule(registrationContext);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(services.Any(service => service.ServiceType == typeof(UpsertBidconCredentialsCommandHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(GetEstimationListQueryHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(ImportEstimationSnapshotCommandHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(BidconEstimationClient)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(IBidconAccessTokenCache)), Is.True);
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IBidconCredentialsVerifier) &&
                service.ImplementationType == typeof(BidconCredentialsVerifier) &&
                service.Lifetime == ServiceLifetime.Transient));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IBidconCredentialsAuditLog) &&
                service.ImplementationType == typeof(MongoBidconCredentialsAuditLog) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IMongoCollection<MongoBidconApiCredentials>) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IMongoCollection<MongoBidconCredentialsAuditEntry>) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IBidconApiCredentialsStore) &&
                service.ImplementationType == typeof(MongoBidconApiCredentialsStore) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IMongoCollection<MongoEstimationSnapshot>) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(IEstimationSnapshotStore) &&
                service.ImplementationType == typeof(MongoEstimationSnapshotStore) &&
                service.Lifetime == ServiceLifetime.Singleton));
            Assert.That(registrationContext.AccessClaims.Claims, Has.Member(new AccessClaimDefinition(
                "BrowserAccess",
                "Bidconutforskaren",
                "Använda kalkylutforskaren")));
            Assert.That(registrationContext.Ui.TopNavigation, Has.One.EqualTo(
                new NavigationItem("Kalkyler", BidconRoutes.Browser, 1)));
            Assert.That(registrationContext.Ui.SettingsNavigation, Is.Empty);
            Assert.That(registrationContext.Ui.SettingsAreas, Has.One.EqualTo(
                new SettingsAreaContribution(
                    "bidcon",
                    "Bidcon API",
                    "Configure Bidcon API credentials.",
                    BidconRoutes.Settings,
                    20,
                    MudBlazor.Icons.Material.Sharp.Api,
                    "ApiPolicy")));
        };
    }

    [Test]
    public void EstimationExplorerIsReusableComponentWithoutRoute()
    {
        var routes = typeof(EstimationExplorer)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Select(attribute => attribute.Template);

        Assert.That(routes, Is.Empty);
    }

    [Test]
    public void EstimationExplorerPageUsesLegacyRoute()
    {
        var routes = typeof(EstimationExplorerPage)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Select(attribute => attribute.Template);

        Assert.That(routes, Has.One.EqualTo("/BidconBrowser"));
    }

    [Test]
    public void BidconSettingsPageUsesModuleOwnedRouteAndAuthorizationPolicy()
    {
        var routes = typeof(BidconSettingsPage)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>()
            .Select(attribute => attribute.Template);
        var policies = typeof(BidconSettingsPage)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false)
            .Cast<AuthorizeAttribute>()
            .Select(attribute => attribute.Policy);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(routes, Has.One.EqualTo("/settings/bidcon"));
            Assert.That(policies, Has.One.EqualTo("ApiPolicy"));
        }
    }

    [Test]
    public async Task BidconSettingsAreaIsVisibleOnlyWhenUserSatisfiesPolicy()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();
        services.AddBidconModule(registrationContext);

        var authorizedAreas = await SettingsHub.GetVisibleAreasAsync(
            registrationContext.Ui,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            new PolicyAuthorizationService("ApiPolicy"));
        var unauthorizedAreas = await SettingsHub.GetVisibleAreasAsync(
            registrationContext.Ui,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            new PolicyAuthorizationService());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(authorizedAreas.Select(area => area.Id), Has.One.EqualTo("bidcon"));
            Assert.That(unauthorizedAreas, Is.Empty);
        };
    }

    [Test]
    public async Task BidconApiPolicyAllowsConfiguredClaimOrAdminRole()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();
        services.AddAccessModule(registrationContext);
        services.AddBidconModule(registrationContext);
        services.AddLogging();
        services.AddSingleton<IApplicationAdminPrincipalEvaluator>(
            new ClaimTypeApplicationAdminPrincipalEvaluator("external-admin"));
        services.ApplyAppRegistrationContext(registrationContext);

        using var provider = services.BuildServiceProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();

        var authorizedByClaim = await authorizationService.AuthorizeAsync(
            PrincipalWithBidconApiClaim(),
            resource: null,
            "ApiPolicy");
        var authorizedByAdmin = await authorizationService.AuthorizeAsync(
            PrincipalInAdminRole(),
            resource: null,
            "ApiPolicy");
        var unauthorized = await authorizationService.AuthorizeAsync(
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            resource: null,
            "ApiPolicy");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(authorizedByClaim.Succeeded, Is.True);
            Assert.That(authorizedByAdmin.Succeeded, Is.True);
            Assert.That(unauthorized.Succeeded, Is.False);
        };
    }

    private static ClaimsPrincipal PrincipalWithBidconApiClaim()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(AccessAuthorizationConstants.AccessClaimType, "BidconApiWritePermission")
        ], "test"));
    }

    private static ClaimsPrincipal PrincipalInAdminRole()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(
                "external-admin",
                "true")
        ], "test"));
    }

    private sealed class ClaimTypeApplicationAdminPrincipalEvaluator(string claimType) : IApplicationAdminPrincipalEvaluator
    {
        public bool IsApplicationAdmin(ClaimsPrincipal principal)
            => principal.HasClaim(claimType, "true");
    }

    private sealed class PolicyAuthorizationService(params string[] allowedPolicies) : IAuthorizationService
    {
        private readonly HashSet<string> _allowedPolicies = allowedPolicies.ToHashSet(StringComparer.Ordinal);

        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(AuthorizationResult.Success());
        }

        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            string policyName)
        {
            return Task.FromResult(
                _allowedPolicies.Contains(policyName)
                    ? AuthorizationResult.Success()
                    : AuthorizationResult.Failed());
        }
    }
}
