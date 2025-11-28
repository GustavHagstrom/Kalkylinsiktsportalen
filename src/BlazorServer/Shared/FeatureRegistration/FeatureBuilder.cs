using BlazorServer.Shared.Abstractions;

namespace BlazorServer.Shared.FeatureRegistration;

public class FeatureBuilder
{
    private readonly WebApplicationBuilder _builder;
    private readonly IServiceProvider _sp;

    public FeatureBuilder(WebApplicationBuilder builder, IServiceProvider sp)
    {
        _builder = builder;
        _sp = sp;
    }
    public RenderCollection RenderCollection { get; set; } = new();
    public void AddFeature<T>() where T : IFeature
    {
        var feature = ActivatorUtilities.CreateInstance<T>(_sp);
        feature.RegisterServices(_builder.Services);
        feature.RegisterRenderObjects(RenderCollection);
    }
    
}
