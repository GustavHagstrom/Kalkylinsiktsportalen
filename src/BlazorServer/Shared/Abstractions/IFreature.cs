using BlazorServer.Shared.FeatureRegistration;

namespace BlazorServer.Shared.Abstractions;

public interface IFeature
{
    void RegisterServices(IServiceCollection services);
    void RegisterRenderObjects(RenderCollection collection);
}
