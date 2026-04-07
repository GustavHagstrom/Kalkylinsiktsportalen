namespace BlazorServer.Features.BidconBrowser;

public static class FeatureConstants
{
    public static class Routes
    {
        public const string Base = "/BidconBrowser";
        //public const string Estimations = Base + "/estimations";
    }
    public static class Authorization
    {
        public const string PolicyName = "BrowserAccess";
        public const string AccessClaimValue = "BrowserAccess";
    }
}
