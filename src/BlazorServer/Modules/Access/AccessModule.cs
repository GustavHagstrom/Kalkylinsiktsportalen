using BlazorServer.Features.Access;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Models;
using MudBlazor;

namespace BlazorServer.Modules.Access;

public static class AccessModule
{
    public static IServiceCollection AddAccessModule(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        services.AddTransient<FunctionalAccessGrantUseCases>();
        services.AddSingleton<IViewAccessGrantStore, InMemoryViewAccessGrantStore>();
        services.AddTransient<ViewAccessGrantUseCases>();
        services.AddTransient<ViewAccessDecisionUseCases>();

        registrationContext.Policies.AddPolicy(FeatureConstants.Authorization.PolicyName, config =>
        {
            config.RequireAssertion(context =>
                context.User.HasClaim(
                    ApplicationClaimTypes.AccessClaim,
                    FeatureConstants.Authorization.AccessClaimValue) ||
                context.User.IsInRole(AuthConstants.AdminRoleClaimValue));
        });

        registrationContext.AccessClaims.Register(new AccessClaimInfo(
            FeatureConstants.Authorization.AccessClaimValue,
            "Rättigheter",
            "Skapa, ändra och ta bort användarrättigheter och roller."));

        registrationContext.UiComposition.AddSettingsArea(
            new UiSettingsArea(
                "access",
                "Rättigheter",
                "Hantera användarrättigheter och externa grupper.",
                "/settings/access",
                10,
                Icons.Material.Sharp.AdminPanelSettings,
                FeatureConstants.Authorization.PolicyName),
            typeof(AccessModule).FullName ?? nameof(AccessModule));

        return services;
    }
}
