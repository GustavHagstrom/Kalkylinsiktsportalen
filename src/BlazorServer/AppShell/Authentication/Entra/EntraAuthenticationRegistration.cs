using BlazorServer.Composition;
using BlazorServer.Identity;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Kiota.Abstractions.Authentication;

namespace BlazorServer.AppShell.Authentication.Entra;

public static class EntraAuthenticationRegistration
{
    public static IServiceCollection AddEntraAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        AppRegistrationContext registrationContext)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(registrationContext, nameof(registrationContext));

        services.SetLoginDisplay<LoginDisplay>();
        services.AddScoped<BlazorCurrentUserContext>();
        services.AddScoped<IAuthenticationActions, AuthenticationActions>();
        services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<BlazorCurrentUserContext>());
        services.AddSingleton<IApplicationAdminPrincipalEvaluator, EntraApplicationAdminPrincipalEvaluator>();
        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                var section = configuration.GetSection("AzureAd");
                section.Bind(options);
                options.Scope.Add("email");
            })
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();
        services.AddControllersWithViews().AddMicrosoftIdentityUI();
        services.AddTransient<IAuthenticationProvider, GraphClientAuthProvider>();
        services.AddScoped(sp => new GraphServiceClient(sp.GetRequiredService<IAuthenticationProvider>()));

        return services;
    }
}
