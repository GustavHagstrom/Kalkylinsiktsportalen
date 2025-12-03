namespace BlazorServer.Features.Access;
public static class FeatureConstants
{
    public static class Routes
    {
        public const string UserAccessEdit = "/Settings/rights/UserEdit";
        public const string RoleAccessEdit = "/Settings/rights/RoleEdit";
        public const string SettingsPage = "/Settings/rights";

    }
    public static class Authorization
    {
        public const string PolicyName = "UserRightSettingsPolicy";
        public const string AccessClaimValue = "UserRightSettingsAccess";
    }
}
