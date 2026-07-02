using System.Security.Claims;
using BlazorServer.Features.Settings;
using BlazorServer.Shared.Composition;
using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Tests.Features.Settings;

[TestFixture]
public class SettingsHubTests
{
    [Test]
    public async Task HubReadsVisibleSettingsAreasFromRegistry()
    {
        var registry = new UiCompositionRegistry();
        registry.AddSettingsArea(
            new UiSettingsArea(
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
            new UiSettingsArea(
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
        var registry = new UiCompositionRegistry();
        registry.AddSettingsNavigation(new UiNavigationItem("Legacy settings", "/settings/legacy", 10));

        var visibleAreas = await SettingsHub.GetVisibleAreasAsync(
            registry,
            new ClaimsPrincipal(new ClaimsIdentity("test")),
            new PolicyAuthorizationService());

        Assert.That(visibleAreas, Is.Empty);
    }

    [Test]
    public async Task HubHidesSettingsAreasWhenUserDoesNotSatisfyPolicy()
    {
        var registry = new UiCompositionRegistry();
        registry.AddSettingsArea(
            new UiSettingsArea(
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
