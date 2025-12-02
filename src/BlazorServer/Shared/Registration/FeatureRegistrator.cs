using BlazorServer.Shared.Abstractions;

namespace BlazorServer.Shared.Registration;

public class FeatureRegistrator
{
    private readonly WebApplicationBuilder _builder;
    private readonly IServiceProvider _sp;

    public FeatureRegistrator(WebApplicationBuilder builder)
    {
        _builder = builder;
        _sp = builder.Services.BuildServiceProvider();
    }
    public RenderCollection RenderCollection { get; set; } = new();
    public void AddFeature<T>() where T : IFeature
    {
        var feature = ActivatorUtilities.CreateInstance<T>(_sp);
        feature.RegisterServices(_builder.Services);
        feature.RegisterRenderObjects(RenderCollection);
    }
    
}
