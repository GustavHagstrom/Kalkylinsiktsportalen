using BlazorServer.Identity;
using Microsoft.AspNetCore.Authentication;

namespace BlazorServer.AppShell.Authentication.Mock;

public static class MockAuthenticationRegistration
{
    public static IServiceCollection AddMockAuthentication(
        this IServiceCollection services,
        string organizationId)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId, nameof(organizationId));

        services.SetLoginDisplay<MockedLoginDisplay>();
        services.AddScoped<MockCurrentUserContext>();
        services.AddScoped<MockCurrentUserSwitcher>();
        services.AddScoped<IAuthenticationActions, AuthenticationActions>();
        services.AddSingleton<MockAuthenticationState>();
        services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<MockCurrentUserContext>());
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthSchemes.Mock, null);
        services.AddSingleton(new MockAuthDefaults(organizationId));
        services.AddMemoryCache();

        return services;
    }
}
