using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Stores;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores.MongoDb;
using Microsoft.AspNetCore.Authentication;
using MudBlazor;

namespace BlazorServer.Modules.Access;

public static class AccessModule
{
    public static IServiceCollection AddAccessModule(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        services.AddMemoryCache();
        services.AddMongoCollection<MongoFunctionalAccessGrant>("FunctionalAccessGrants");
        services.AddSingleton<IFunctionalAccessGrantStore, MongoAccessStore>();
        services.AddSingleton<IAccessClaimsCache, MemoryAccessClaimsCache>();
        services.AddTransient<IClaimsTransformation, AccessClaimsTransformation>();
        services.AddTransient<FunctionalAccessGrantHandler>();
        services.AddSingleton<IViewAccessGrantStore, InMemoryViewAccessGrantStore>();
        services.AddTransient<ViewAccessGrantHandler>();
        services.AddTransient<ViewAccessDecisionHandler>();

        registrationContext.Policies.AddPolicy(Constants.Authorization.PolicyName, config =>
        {
            config.RequireAssertion(context =>
                context.User.HasClaim(
                    ApplicationClaimTypes.AccessClaim,
                    Constants.Authorization.AccessClaimValue) ||
                context.User.IsInRole(AuthConstants.AdminRoleClaimValue));
        });

        registrationContext.AccessClaims.Register(new AccessClaimInfo(
            Constants.Authorization.AccessClaimValue,
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
                Constants.Authorization.PolicyName),
            typeof(AccessModule).FullName ?? nameof(AccessModule));

        return services;
    }
}
