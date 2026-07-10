using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Shared.Composition;
using BlazorServer.AppShell.Authentication;
using MudBlazor;

namespace BlazorServer.Modules.Reports;

public static class ReportsModule
{
    public static void AddReportsModule(this IServiceCollection services, AppRegistrationContext context)
    {
        context.AccessClaims.Register(new AccessClaimInfo(Authorization.AccessClaimValue, "EditReportsGrant", "Ger användaren tillgång till att hantera och redigera vyer i rapportmodulen."));
        context.Policies.AddPolicy(Authorization.PolicyName, policy => policy.RequireAssertion(context => context.User.HasAccessClaimOrApplicationAdmin(Authorization.AccessClaimValue)));
        context.UiComposition.AddSettingsArea(
            new UiSettingsArea(
                "reports",
                "Vyer", 
                "Hantera vyer", 
                Routes.SettingsPage,
                30,
                Icons.Material.Sharp.Article,
                Authorization.PolicyName), 
            "reportsModule");
    }
    public static class Routes
    {
        public const string SettingsPage = "/settings/Views";
        public static string EditViewPage(string viewId) => $"/Settings/Views/{viewId}";

    }
    public static class Authorization
    {
        public const string PolicyName = "ReportsAccessPolicy";
        public const string AccessClaimValue = "EditReportsGrant";
    }
}
