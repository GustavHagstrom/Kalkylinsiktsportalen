using BlazorServer.AppShell.Authentication.Entra;
using BlazorServer.AppShell.Authentication.Mock;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.AppShell.Authentication;

public static class AuthenticationRegistration
{
    public static AppShellAuthenticationSelection AddAppShellAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        if (configuration["Auth:Mode"] == "Mock")
        {
            var mockOrganizationId = configuration["Auth:Mock:OrganizationId"] ?? "MockedOrganization";
            services.AddMockAuthentication(mockOrganizationId);
            return AppShellAuthenticationSelection.Mock(mockOrganizationId);
        }
        else if (configuration["Auth:Mode"] == "EntraId")
        {
            services.AddEntraAuthentication(configuration);
            return AppShellAuthenticationSelection.EntraId();
        }

        return AppShellAuthenticationSelection.None();
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

    public static AppShellAuthenticationSelection None()
        => new("None", null);
}
