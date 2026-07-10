namespace BlazorServer.Modules.Reports.Public;

public static class ReportsRoutes
{
    public const string SettingsPage = "/settings/Views";

    public static string EditViewPage(string viewId) => $"/Settings/Views/{viewId}";
}
