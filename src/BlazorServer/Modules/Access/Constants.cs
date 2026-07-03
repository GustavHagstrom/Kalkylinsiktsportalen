namespace BlazorServer.Modules.Access;
public static class Constants
{
    public static class Routes
    {
        public const string UserAccessEdit = "/settings/access/users";
        public const string RoleAccessEdit = "/settings/access/roles";
        public const string SettingsPage = "/settings/access";
    }

    public static class Authorization
    {
        public const string PolicyName = "UserRightSettingsPolicy";
        public const string AccessClaimValue = "UserRightSettingsAccess";
    }
}
