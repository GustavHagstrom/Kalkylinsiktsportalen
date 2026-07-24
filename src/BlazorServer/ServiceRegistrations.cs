using BlazorServer.AppShell.Authentication;
using BlazorServer.AppShell.Settings;
using BlazorServer.Composition;
using BlazorServer.Identity;
using BlazorServer.Infrastructure.MongoDb;
using BlazorServer.Modules.Access;
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

        builder.Services.AddSettingsShell(registrationContext);
        builder.Services.AddAccessModule(registrationContext);
        builder.Services.AddBidconModule(
            registrationContext,
            builder.Configuration["Bidcon:BaseUrl"]
                ?? throw new InvalidOperationException("Bidcon:BaseUrl configuration is missing"));
        builder.Services.AddProjectsModule(registrationContext);
        builder.Services.AddReportsModule(registrationContext);

        var authenticationSelection = builder.Services.AddAppShellAuthentication(builder.Configuration);
        if (authenticationSelection.MockOrganizationId is { } mockOrganizationId)
        {
            builder.Services.AddMockAccessDirectory(new OrganizationId(mockOrganizationId));
        }
        else if (authenticationSelection.IsEntraId)
        {
            builder.Services.AddGraphAccessDirectory();
        }

        builder.UseMongoDb();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddSingleton<IComponentHtmlRenderer, ComponentHtmlRenderer>();
        builder.Services.AddLocalization();
        builder.Services.AddMudServices();
        builder.Services.ApplyAppRegistrationContext(registrationContext);
        return builder;
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
