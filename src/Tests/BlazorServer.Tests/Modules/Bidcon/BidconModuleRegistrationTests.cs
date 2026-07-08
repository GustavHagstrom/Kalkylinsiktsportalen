using System.Security.Claims;
using BlazorServer.AppShell.Settings;
using BlazorServer.Modules.Bidcon;
using BlazorServer.Modules.Bidcon.Components;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Features.UpsertBidconCredentials;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Modules.Bidcon.Public.Components;
using BlazorServer.Modules.Bidcon.Settings;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        Assert.Multiple(() =>
        {
            Assert.That(services.Any(service => service.ServiceType == typeof(UpsertBidconCredentialsCommandHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(ImportEstimationSnapshotCommandHandler)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(IBidconAccessTokenCache)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(IBidconApiCredentialsStore)), Is.True);
            Assert.That(services.Any(service => service.ServiceType == typeof(IEstimationSnapshotStore)), Is.True);
            Assert.That(services, Has.One.Matches<ServiceDescriptor>(service =>
                service.ServiceType == typeof(EstimationListState) &&
                service.Lifetime == ServiceLifetime.Scoped));
            Assert.That(registrationContext.AccessClaims.Claims, Has.Member(new AccessClaimInfo(
                "BrowserAccess",
                "Bidconutforskaren",
                "Använda kalkylutforskaren")));
            Assert.That(registrationContext.UiComposition.TopNavigation, Has.One.EqualTo(
                new UiNavigationItem("Kalkyler", "/BidconBrowser", 1)));
            Assert.That(registrationContext.UiComposition.SettingsNavigation, Is.Empty);
            Assert.That(registrationContext.UiComposition.SettingsAreas, Has.One.EqualTo(
                new UiSettingsArea(
                    "bidcon",
                    "Bidcon API",
                    "Configure Bidcon API credentials.",
                    "/settings/bidcon",
                    20,
                    MudBlazor.Icons.Material.Sharp.Api,
                    "ApiPolicy")));
        });
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

        Assert.Multiple(() =>
        {
            Assert.That(routes, Has.One.EqualTo("/settings/bidcon"));
            Assert.That(policies, Has.One.EqualTo("ApiPolicy"));
        });
    }

    [Test]
    public async Task BidconSettingsAreaIsVisibleOnlyWhenUserSatisfiesPolicy()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();
        services.AddBidconModule(registrationContext);

        var authorizedAreas = await SettingsHub.GetVisibleAreasAsync(
            registrationContext.UiComposition,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            new PolicyAuthorizationService("ApiPolicy"));
        var unauthorizedAreas = await SettingsHub.GetVisibleAreasAsync(
            registrationContext.UiComposition,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            new PolicyAuthorizationService());

        Assert.Multiple(() =>
        {
            Assert.That(authorizedAreas.Select(area => area.Id), Has.One.EqualTo("bidcon"));
            Assert.That(unauthorizedAreas, Is.Empty);
        });
    }

    [Test]
    public async Task BidconApiPolicyAllowsConfiguredClaimOrAdminRole()
    {
        var services = new ServiceCollection();
        var registrationContext = services.AddAppRegistrationContext();
        services.AddBidconModule(registrationContext);
        services.AddLogging();
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

        Assert.Multiple(() =>
        {
            Assert.That(authorizedByClaim.Succeeded, Is.True);
            Assert.That(authorizedByAdmin.Succeeded, Is.True);
            Assert.That(unauthorized.Succeeded, Is.False);
        });
    }

    private static ClaimsPrincipal PrincipalWithBidconApiClaim()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ApplicationClaimTypes.AccessClaim, "BidconApiWritePermission")
        ], "test"));
    }

    private static ClaimsPrincipal PrincipalInAdminRole()
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, AuthConstants.AdminRoleClaimValue)
        ], "test"));
    }

    private sealed class PolicyAuthorizationService(params string[] allowedPolicies) : IAuthorizationService
    {
        private readonly HashSet<string> allowedPolicies = allowedPolicies.ToHashSet(StringComparer.Ordinal);

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
                allowedPolicies.Contains(policyName)
                    ? AuthorizationResult.Success()
                    : AuthorizationResult.Failed());
        }
    }
}
