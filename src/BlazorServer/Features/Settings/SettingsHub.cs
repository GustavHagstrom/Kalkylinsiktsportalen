using System.Security.Claims;
using BlazorServer.Shared.Composition;
using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Features.Settings;

public static class SettingsHub
{
    public static async Task<IReadOnlyList<UiSettingsArea>> GetVisibleAreasAsync(
        UiCompositionRegistry uiComposition,
        ClaimsPrincipal user,
        IAuthorizationService authorizationService)
    {
        var visibleAreas = new List<UiSettingsArea>();

        foreach (var area in uiComposition.SettingsAreas)
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
