namespace BlazorServer.Features.BidconAccess;
public static class FeatureConstants
{
    public static class Routes
    {
        public const string SettingsPage = "/Settings/api";

    }
    public static class Authorization
    {
        public const string PolicyName = "ApiPolicy";
        public const string AccessClaimValue = "ApiSettingsAccess";
    }
}
