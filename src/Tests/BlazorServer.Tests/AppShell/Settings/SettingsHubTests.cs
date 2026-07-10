using BlazorServer.AppShell.Composition.Navigation;
using BlazorServer.AppShell.Composition.Settings;
using BlazorServer.AppShell.Composition.UI;
using System.Security.Claims;
using BlazorServer.AppShell.Settings;
using BlazorServer.AppShell.Composition;
using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Tests.AppShell.Settings;

[TestFixture]
public class SettingsHubTests
{
    [Test]
    public async Task HubReadsVisibleSettingsAreasFromRegistry()
    {
        var registry = new UiRegistry();
        registry.AddSettingsArea(
            new SettingsAreaContribution(
                "bidcon",
                "Bidcon",
                "Configure Bidcon integration.",
                "/settings/bidcon",
                20,
                "settings",
                "CanManageBidcon"),
            "BidconModule");

        var visibleAreas = await SettingsHub.GetVisibleAreasAsync(
            registry,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            new PolicyAuthorizationService("CanManageBidcon"));

        Assert.That(visibleAreas, Has.One.EqualTo(
            new SettingsAreaContribution(
                "bidcon",
                "Bidcon",
                "Configure Bidcon integration.",
                "/settings/bidcon",
                20,
                "settings",
                "CanManageBidcon")));
    }

    [Test]
    public async Task HubDoesNotFallbackToOldSettingsNavigation()
    {
        var registry = new UiRegistry();
        registry.AddSettingsNavigation(new NavigationItem("Legacy settings", "/settings/legacy", 10));

        var visibleAreas = await SettingsHub.GetVisibleAreasAsync(
            registry,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            new PolicyAuthorizationService());

        Assert.That(visibleAreas, Is.Empty);
    }

    [Test]
    public async Task HubHidesSettingsAreasWhenUserDoesNotSatisfyPolicy()
    {
        var registry = new UiRegistry();
        registry.AddSettingsArea(
            new SettingsAreaContribution(
                "access",
                "Access",
                "Manage rights.",
                "/settings/access",
                10,
                Policy: "CanManageAccess"),
            "AccessModule");

        var visibleAreas = await SettingsHub.GetVisibleAreasAsync(
            registry,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            new PolicyAuthorizationService());

        Assert.That(visibleAreas, Is.Empty);
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
