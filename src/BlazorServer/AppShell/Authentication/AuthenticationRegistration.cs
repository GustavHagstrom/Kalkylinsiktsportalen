using BlazorServer.AppShell.Authentication.Entra;
using BlazorServer.AppShell.Authentication.Mock;
using BlazorServer.Composition;
using BlazorServer.Modules.Access.Public.Authorization;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.AppShell.Authentication;

public static class AuthenticationRegistration
{
    private const string DefaultMockOrganizationId = "MockedOrganization";

    public static AppShellAuthenticationSelection AddAppShellAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        AppRegistrationContext registrationContext)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(registrationContext, nameof(registrationContext));

        services.AddScoped<ICurrentAccessSubject, CurrentAccessSubject>();

        var authenticationMode = configuration["Auth:Mode"]?.Trim();
        return authenticationMode switch
        {
            "Mock" => AddMockAuthenticationSelection(services, configuration, registrationContext),
            "EntraId" => AddEntraAuthenticationSelection(services, configuration, registrationContext),
            var mode => throw new InvalidOperationException(
                $"Unsupported authentication mode '{(string.IsNullOrWhiteSpace(mode) ? "<missing>" : mode)}'. Configure Auth:Mode as 'Mock' or 'EntraId'.")
        };
    }

    private static AppShellAuthenticationSelection AddMockAuthenticationSelection(
        IServiceCollection services,
        IConfiguration configuration,
        AppRegistrationContext registrationContext)
    {
        var configuredOrganizationId = configuration["Auth:Mock:OrganizationId"]?.Trim();
        var mockOrganizationId = string.IsNullOrWhiteSpace(configuredOrganizationId)
            ? DefaultMockOrganizationId
            : configuredOrganizationId;
        services.AddMockAuthentication(mockOrganizationId, registrationContext);
        return AppShellAuthenticationSelection.Mock(mockOrganizationId);
    }

    private static AppShellAuthenticationSelection AddEntraAuthenticationSelection(
        IServiceCollection services,
        IConfiguration configuration,
        AppRegistrationContext registrationContext)
    {
        services.AddEntraAuthentication(configuration, registrationContext);
        return AppShellAuthenticationSelection.EntraId();
    }

    public static IServiceCollection SetLoginDisplay<T>(this IServiceCollection services)
        where T : ComponentBase
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        var requestedType = typeof(T);

        var descriptors = services
            .Where(x => x.ServiceType == typeof(LoginDisplayRecord))
            .ToArray();

        if (descriptors.Length == 0)
        {
            services.AddSingleton(LoginDisplayRecord.Create<T>());
            return services;
        }

        var registrations = descriptors
            .Select(x => x.ImplementationInstance)
            .OfType<LoginDisplayRecord>()
            .ToArray();

        if (registrations.Length != descriptors.Length)
        {
            throw new InvalidOperationException($"{nameof(LoginDisplayRecord)} has been registered using an unsupported registration form. Register it through {nameof(SetLoginDisplay)}.");
        }

        if (registrations.All(x => x.ComponentType == requestedType))
        {
            return services;
        }

        var registeredTypes = registrations
            .Select(x => x.ComponentType.FullName)
            .Distinct();

        throw new InvalidOperationException($"Login display component is already registered as {string.Join(", ", registeredTypes)}. Cannot register {requestedType.FullName}.");
    }
}

public sealed record AppShellAuthenticationSelection
{
    private AppShellAuthenticationSelection(string mode, string? mockOrganizationId)
    {
        Mode = mode;
        MockOrganizationId = mockOrganizationId;
    }

    public string Mode { get; }

    public string? MockOrganizationId { get; }

    public bool IsMock => MockOrganizationId is not null;

    public bool IsEntraId => Mode == "EntraId";

    public static AppShellAuthenticationSelection Mock(string organizationId)
        => new("Mock", organizationId);

    public static AppShellAuthenticationSelection EntraId()
        => new("EntraId", null);
}
