using BlazorServer.AppShell.Composition.Settings;
using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.AppShell.Composition;
using BlazorServer.Modules.Access.Authentication;
using MudBlazor;

namespace BlazorServer.Modules.Reports;

public static class ReportsModule
{
    public static void AddReportsModule(this IServiceCollection services, AppRegistrationContext context)
    {
        context.AccessClaims.Register(new AccessClaimInfo(Authorization.AccessClaimValue, "EditReportsGrant", "Ger användaren tillgång till att hantera och redigera vyer i rapportmodulen."));
        context.Policies.AddPolicy(Authorization.PolicyName, policy => policy.RequireAssertion(context => context.User.HasAccessClaimOrApplicationAdmin(Authorization.AccessClaimValue)));
        context.Ui.AddSettingsArea(
            new SettingsAreaContribution(
                "reports",
                "Vyer", 
                "Hantera vyer", 
                ReportsRoutes.SettingsPage,
                30,
                Icons.Material.Sharp.Article,
                Authorization.PolicyName), 
            "reportsModule");
    }
    public static class Authorization
    {
        public const string PolicyName = "ReportsAccessPolicy";
        public const string AccessClaimValue = "EditReportsGrant";
    }
}
