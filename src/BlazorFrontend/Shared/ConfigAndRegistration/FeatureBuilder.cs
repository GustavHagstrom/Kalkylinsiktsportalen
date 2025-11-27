using BlazorFrontend.Shared.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorFrontend.Shared.ConfigAndRegistration;

public class FeatureBuilder
{
    private readonly WebAssemblyHostBuilder _builder;
    private readonly IServiceProvider _sp;

    public FeatureBuilder(WebAssemblyHostBuilder builder, IServiceProvider sp)
    {
        _builder = builder;
        _sp = sp;
    }
    public RenderCollection ComponentTypeCollection { get; set; } = new();
    public void AddFeature<T>() where T : IFeature
    {
        var feature = ActivatorUtilities.CreateInstance<T>(_sp);
        feature.RegisterServices(_builder.Services);
        feature.RegisterRenderObjects(ComponentTypeCollection);
    }
    
}
