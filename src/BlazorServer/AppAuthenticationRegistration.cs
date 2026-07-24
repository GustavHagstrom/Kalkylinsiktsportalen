using BlazorServer.AppShell.Authentication;
using BlazorServer.Composition;
using BlazorServer.Identity;
using BlazorServer.Modules.Access;

namespace BlazorServer;

public static class AppAuthenticationRegistration
{
    public static IServiceCollection AddAppAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        AppRegistrationContext registrationContext)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(registrationContext, nameof(registrationContext));

        var authenticationSelection = services.AddAppShellAuthentication(configuration, registrationContext);
        if (authenticationSelection.MockOrganizationId is { } mockOrganizationId)
        {
            services.AddMockAccessDirectory(new OrganizationId(mockOrganizationId));
        }
        else if (authenticationSelection.IsEntraId)
        {
            services.AddGraphAccessDirectory();
        }

        return services;
    }
}
