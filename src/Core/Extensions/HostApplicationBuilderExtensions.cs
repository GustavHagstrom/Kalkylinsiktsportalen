using Core.Abstractions;
using Core.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;
public static class HostApplicationBuilderExtensions
{

    public static void ConfigureMonolith(this WebApplicationBuilder builder, Action<MonolithConfigurator> configure)
    {
        var moduleBuilder = new MonolithConfigurator(builder);
        configure(moduleBuilder);
    }
    public static void AddApplicationInjectableComponent<T>(this IServiceCollection services, string key) where T : class, IInjectableComponent
    {
        services.AddKeyedTransient<IInjectableComponent, T>(key);
    }
    public static void AddAccessClaimInfo<T>(this IServiceCollection services) where T : class, IAccessClaimInfo
    {
        services.AddSingleton<IAccessClaimInfo, T>();
    }
}
