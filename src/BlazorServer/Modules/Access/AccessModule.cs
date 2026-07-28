using BlazorServer.Composition;
using BlazorServer.Composition.Settings;
using BlazorServer.Identity;
using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Modules.Access.Features.GetEffectiveFunctionalAccessClaims;
using BlazorServer.Modules.Access.Features.GetRoleFunctionalAccess;
using BlazorServer.Modules.Access.Features.GetUserFunctionalAccess;
using BlazorServer.Modules.Access.Features.GrantRoleFunctionalAccess;
using BlazorServer.Modules.Access.Features.GrantRoleViewAccess;
using BlazorServer.Modules.Access.Features.GrantUserFunctionalAccess;
using BlazorServer.Modules.Access.Features.GrantUserViewAccess;
using BlazorServer.Modules.Access.Features.RevokeRoleFunctionalAccess;
using BlazorServer.Modules.Access.Features.RevokeRoleViewAccess;
using BlazorServer.Modules.Access.Features.RevokeUserFunctionalAccess;
using BlazorServer.Modules.Access.Features.RevokeUserViewAccess;
using BlazorServer.Modules.Access.Features.ViewAccessGrants;
using BlazorServer.Modules.Access.Infrastructure.InMemory;
using BlazorServer.Modules.Access.Infrastructure.Identity;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Stores;
using BlazorServer.Modules.Access.Persistence;
using BlazorServer.Modules.Access.Public.Authorization;
using BlazorServer.Modules.Access.Public.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using MudBlazor;

namespace BlazorServer.Modules.Access;

public static class AccessModule
{
    public static IServiceCollection AddAccessModule(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        services.AddMemoryCache();
        services.AddFunctionalAccessGrantCollection();
        services.AddSingleton<IFunctionalAccessGrantStore, MongoAccessStore>();
        services.AddSingleton<IAccessClaimsCache, MemoryAccessClaimsCache>();
        services.AddTransient<IAccessSubjectFactory, AccessSubjectFactory>();
        services.AddTransient<IClaimsTransformation, ClaimsTransformation>();
        services.AddTransient<IAuthorizationHandler, AccessClaimOrApplicationAdminHandler>();
        services.AddTransient<GetUserFunctionalAccessQueryHandler>();
        services.AddTransient<GrantUserFunctionalAccessCommandHandler>();
        services.AddTransient<RevokeUserFunctionalAccessCommandHandler>();
        services.AddTransient<GetRoleFunctionalAccessQueryHandler>();
        services.AddTransient<GrantRoleFunctionalAccessCommandHandler>();
        services.AddTransient<RevokeRoleFunctionalAccessCommandHandler>();
        services.AddTransient<GetEffectiveFunctionalAccessClaimsQueryHandler>();
        services.AddSingleton<IViewAccessGrantStore, InMemoryViewAccessGrantStore>();
        services.AddTransient<GrantUserViewAccessCommandHandler>();
        services.AddTransient<GrantRoleViewAccessCommandHandler>();
        services.AddTransient<RevokeUserViewAccessCommandHandler>();
        services.AddTransient<RevokeRoleViewAccessCommandHandler>();
        services.AddTransient<ViewAccessDecisionHandler>();
        services.AddTransient<IViewAccessDecisionService>(sp => sp.GetRequiredService<ViewAccessDecisionHandler>());

        registrationContext.Policies.AddPolicy(Constants.Authorization.PolicyName, config =>
        {
            config.RequireAccessClaimOrApplicationAdmin(Constants.Authorization.AccessClaimValue);
        });

        registrationContext.AccessClaims.Register(new AccessClaimDefinition(
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

    public static IServiceCollection AddMockAccessDirectory(this IServiceCollection services, OrganizationId organizationId)
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
