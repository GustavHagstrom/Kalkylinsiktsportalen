using BlazorServer.AppShell.Composition;
using BlazorServer.AppShell.Composition.Settings;
using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Access.Authentication.Mock;
using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Infrastructure.Identity;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Stores;
using BlazorServer.Modules.Access.Public.Contracts;
using Microsoft.AspNetCore.Authentication;
using MongoDB.Driver;
using MudBlazor;

namespace BlazorServer.Modules.Access;

public static class AccessModule
{
    public static IServiceCollection AddAccessModule(
        this IServiceCollection services,
        AppRegistrationContext registrationContext,
        IConfiguration configuration)
    {
        if (configuration["Auth:Mode"] == "Mock")
        {
            services.AddSingleton<MockedAuthSession>();
            services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthSchemes.Mock, null);
        }
        else if (configuration["Auth:Mode"] == "Graph")
        {

        }
        services.AddMemoryCache();
        services.AddFunctionalAccessGrantCollection();
        services.AddSingleton<IFunctionalAccessGrantStore, MongoAccessStore>();
        services.AddSingleton<IAccessClaimsCache, MemoryAccessClaimsCache>();
        services.AddTransient<IClaimsTransformation, ClaimsTransformation>();
        services.AddTransient<FunctionalAccessGrantHandler>();
        services.AddSingleton<IViewAccessGrantStore, InMemoryViewAccessGrantStore>();
        services.AddTransient<ViewAccessGrantHandler>();
        services.AddTransient<ViewAccessDecisionHandler>();

        registrationContext.Policies.AddPolicy(Constants.Authorization.PolicyName, config =>
        {
            config.RequireAssertion(context =>
                context.User.HasAccessClaimOrApplicationAdmin(Constants.Authorization.AccessClaimValue));
        });

        registrationContext.AccessClaims.Register(new AccessClaimInfo(
            Constants.Authorization.AccessClaimValue,
            "Rättigheter",
            "Skapa, ändra och ta bort användarrättigheter och roller."));

        registrationContext.Ui.AddSettingsArea(
            new SettingsAreaContribution(
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

    public static IServiceCollection AddMockAccessDirectory(this IServiceCollection services, string organizationId)
    {
        return services.AddSingleton<IAccessDirectory>(_ => new MockAccessDirectory(organizationId));
    }

    public static IServiceCollection AddGraphAccessDirectory(this IServiceCollection services)
    {
        return services.AddTransient<IAccessDirectory, GraphAccessDirectory>();
    }

    private static IServiceCollection AddFunctionalAccessGrantCollection(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            const string collectionName = "FunctionalAccessGrants";
            var names = database.ListCollectionNames().ToList();
            if (!names.Contains(collectionName))
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<MongoFunctionalAccessGrant>(collectionName);
        });
        return services;
    }
}
