namespace BlazorServer.AppShell.Composition;

public static class AppRegistrationServiceCollectionExtensions
{
    public static AppRegistrationContext AddAppRegistrationContext(this IServiceCollection services)
    {
        var context = new AppRegistrationContext();
        services.AddSingleton(context);
        return context;
    }

    public static IServiceCollection ApplyAppRegistrationContext(
        this IServiceCollection services,
        AppRegistrationContext registrationContext)
    {
        services.AddSingleton(registrationContext.Ui);
        services.AddSingleton(registrationContext.Ui.Navigation);
        services.AddSingleton(registrationContext.Ui.SettingsAreaRegistry);
        services.AddSingleton(registrationContext.AccessClaims);
        services.AddAuthorization(options =>
        {
            foreach (var policy in registrationContext.Policies.PolicyList)
            {
                options.AddPolicy(policy.Name, policy.Builder);
            }
        });

        return services;
    }
}
