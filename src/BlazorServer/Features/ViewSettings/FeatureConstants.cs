namespace BlazorServer.Features.ViewSettings;
public static class FeatureConstants
{
    public static class Routes
    {
        public const string SettingsPage = "/Settings/Views";
        public static string EditViewPage(string viewId) => $"/Settings/Views/{viewId}";

    }
    public static class Authorization
    {
        public const string PolicyName = "ViewSettingsPolicy";
        public const string AccessClaimValue = "ViewSettingsAccess";
    }
}
