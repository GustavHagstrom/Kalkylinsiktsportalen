
using BlazorFrontend.Shared.ConfigAndRegistration;

namespace BlazorFrontend.Shared.Abstractions;

public interface IFeature
{
    void RegisterServices(IServiceCollection services);
    void RegisterRenderObjects(RenderCollection collection);
}
