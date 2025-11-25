namespace BidconBrowserModule;
internal class Constants
{
    public static class ModuleRoutes
    {
        public const string BidconBrowser = "/bidcon-browser";
        public const string PreviewPage = "/bidcon-browser/preview";

    }
    public static class Authorization
    {
        public const string Policy = "BidconBrowserPolicy";
        public const string AccessClaimValue = "BidconBrowserAccess";
    }
    public static class LocalStorageKeys
    {
        public const string SelectedEstimationsIds = "SelectedEstimationIds";
    }
}
