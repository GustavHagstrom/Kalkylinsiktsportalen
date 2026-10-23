using BlazorServer.Composition;
using BlazorServer.Composition.Settings;
using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Access.Public.Authorization;
using BlazorServer.Modules.Access.Public.Contracts;
using BlazorServer.Modules.Reports.Components;
using BlazorServer.Modules.Reports.Features.CreateView;
using BlazorServer.Modules.Reports.Features.GetView;
using BlazorServer.Modules.Reports.Features.ListViews;
using BlazorServer.Modules.Reports.Features.UpdateView;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Persistence;
using BlazorServer.Modules.Reports.Persistence.MongoDb;
using MongoDB.Driver;
using MudBlazor;

namespace BlazorServer.Modules.Reports;

public static class ReportsModule
{
    public static void AddReportsModule(this IServiceCollection services, AppRegistrationContext context)
    {
        context.AccessClaims.Register(new AccessClaimDefinition(Authorization.AccessClaimValue, "EditReportsGrant", "Ger användaren tillgång till att hantera och redigera vyer i rapportmodulen."));
        context.Policies.AddPolicy(Authorization.PolicyName, policy => policy.RequireAccessClaimOrApplicationAdmin(Authorization.AccessClaimValue));
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

        services.AddViewCollection();
        services.AddSingleton<IViewStore, MongoViewStore>();
        services.AddScoped<ICommandHandler<CreateViewCommand, CreateViewResult>, CreateViewCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateViewCommand, UpdateViewResult>, UpdateViewCommandHandler>();
        services.AddScoped<IQueryHandler<GetViewQuery, GetViewResult>, GetViewQueryHandler>();
        services.AddScoped<IQueryHandler<ListViewsQuery, ListViewsResult>, ListViewsQueryHandler>();
    }

    private static IServiceCollection AddViewCollection(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            const string collectionName = "Views";
            var names = database.ListCollectionNames().ToList();
            if (!names.Contains(collectionName))
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<MongoView>(collectionName);
        });
        return services;
    }

    public static class Authorization
    {
        public const string PolicyName = "ReportsAccessPolicy";
        public const string AccessClaimValue = "EditReportsGrant";
    }
}
