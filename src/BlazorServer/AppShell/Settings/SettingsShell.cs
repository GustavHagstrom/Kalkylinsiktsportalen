using BlazorServer.Composition.Navigation;
using BlazorServer.AppShell.Routes;
using BlazorServer.AppShell.Settings.Infrastructure.MongoDb.Entities;
using BlazorServer.AppShell.Settings.Infrastructure.MongoDb.Stores;
using BlazorServer.Composition;
using MongoDB.Driver;

namespace BlazorServer.AppShell.Settings;

public static class SettingsShell
{
    public static IServiceCollection AddSettingsShell(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        registrationContext.Ui.AddTopNavigation(
            new NavigationItem(
                "Inställningar",
                AppShellRoutes.Settings,
                10));

        services.AddUserSettingsCollection();
        services.AddSingleton<IUserSettingsStore, MongoUserSettingsStore>();
        services.AddSingleton<IDarkmodeStore, MongoUserSettingsStore>();

        return services;
    }

    private static IServiceCollection AddUserSettingsCollection(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            const string collectionName = "UserSettings";
            var names = database.ListCollectionNames().ToList();
            if (!names.Contains(collectionName))
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<MongoUserSettings>(collectionName);
        });
        return services;
    }
}
