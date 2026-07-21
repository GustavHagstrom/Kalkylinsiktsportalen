using Microsoft.AspNetCore.Components;

namespace BlazorServer.AppShell.Authentication;

public static class ServiceCollectionExtensions
{
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
