using BlazorServer.AppShell.Settings;
using BlazorServer.Composition;
using BlazorServer.Infrastructure.MongoDb;
using BlazorServer.Modules.Access;
using BlazorServer.Modules.Access.Public.Authorization;
using BlazorServer.Modules.Bidcon;
using BlazorServer.Modules.Projects;
using BlazorServer.Modules.Reports;
using BlazorServer.UiKit.Rendering;
using MudBlazor.Services;

namespace BlazorServer;

public static class ServiceRegistrations
{
    public static WebApplicationBuilder AddKipServicesAndFeatures(this WebApplicationBuilder builder)
    {
        var registrationContext = builder.Services.AddAppRegistrationContext();

        builder.Services.AddAppAuthentication(builder.Configuration, registrationContext);
        builder.Services.AddSettingsShell(registrationContext);
        builder.Services.AddKipModules(builder.Configuration, registrationContext);

        builder.UseMongoDb();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddSingleton<IComponentHtmlRenderer, ComponentHtmlRenderer>();
        builder.Services.AddLocalization();
        builder.Services.AddMudServices();
        builder.Services.ApplyAppRegistrationContext(registrationContext);
        return builder;
    }

    public static IServiceCollection AddKipModules(
        this IServiceCollection services,
        IConfiguration configuration,
        AppRegistrationContext registrationContext)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(registrationContext, nameof(registrationContext));

        RequireApplicationAdminEvaluator(services);

        services.AddAccessModule(registrationContext); //this needs to be first so that the access module can register its policies before other modules are registered
        services.AddBidconModule(
            registrationContext,
            configuration["Bidcon:BaseUrl"]
                ?? throw new InvalidOperationException("Bidcon:BaseUrl configuration is missing"));
        services.AddProjectsModule(registrationContext);
        services.AddReportsModule(registrationContext);

        return services;
    }

    private static void RequireApplicationAdminEvaluator(IServiceCollection services)
    {
        if (services.Any(service => service.ServiceType == typeof(IApplicationAdminPrincipalEvaluator)))
        {
            return;
        }

        throw new InvalidOperationException(
            "Access authorization requires an IApplicationAdminPrincipalEvaluator. Register AppShell authentication before AddKipModules.");
    }

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
