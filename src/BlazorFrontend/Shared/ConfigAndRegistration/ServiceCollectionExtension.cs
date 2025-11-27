using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorFrontend.Shared.ConfigAndRegistration;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {

        return services;
    }
    public static void ConfigureFeatures(this WebAssemblyHostBuilder builder, Action<FeatureBuilder> config)
    {
        var sp = builder.Services.BuildServiceProvider();
        var featureBuilder = new FeatureBuilder(builder, sp);
        config(featureBuilder);
        builder.Services.AddSingleton(featureBuilder.ComponentTypeCollection);

    }
}
