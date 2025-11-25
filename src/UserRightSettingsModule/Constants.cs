namespace UserRightSettingsModule;
internal static class Constants
{
    public static class ModuleRoutes
    {
        public const string UserAccessEdit = "/Settings/rights/UserEdit";
        public const string RoleAccessEdit = "/Settings/rights/RoleEdit";
        public const string SettingsPage = "/Settings/rights";

    }
    public static class AuthorizationConstants
    {
        public const string Policy = "UserRightSettingsPolicy";
        public const string AccessClaimValue = "UserRightSettingsAccess";
    }
}
