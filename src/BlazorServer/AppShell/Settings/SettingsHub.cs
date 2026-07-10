using BlazorServer.AppShell.Composition.Settings;
using BlazorServer.AppShell.Composition.UI;
using System.Security.Claims;
using BlazorServer.AppShell.Composition;
using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.AppShell.Settings;

public static class SettingsHub
{
    public static async Task<IReadOnlyList<SettingsAreaContribution>> GetVisibleAreasAsync(
        UiRegistry ui,
        ClaimsPrincipal user,
        IAuthorizationService authorizationService)
    {
        var visibleAreas = new List<SettingsAreaContribution>();

        foreach (var area in ui.SettingsAreas)
        {
            if (area.Policy is null)
            {
                visibleAreas.Add(area);
                continue;
            }

            var authorizationResult = await authorizationService.AuthorizeAsync(user, null, area.Policy);
            if (authorizationResult.Succeeded)
            {
                visibleAreas.Add(area);
            }
        }

        return visibleAreas;
    }
}
