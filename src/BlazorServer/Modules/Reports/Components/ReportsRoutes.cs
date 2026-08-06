using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Reports.Components;

internal static class ReportsRoutes
{
    public const string SettingsPage = "/settings/Views";

    public static string EditViewPage(ViewId viewId) => $"/Settings/Views/{viewId}";
}
