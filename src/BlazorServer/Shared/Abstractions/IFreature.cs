using BlazorServer.Shared.Registration;

namespace BlazorServer.Shared.Abstractions;

public interface IFeature
{
    void RegisterServices(IServiceCollection services);
    void RegisterRenderObjects(RenderCollection collection);
    void RegisterClaims(ClaimInfoCollection collection);
    void AddAuthorizationPolicies(PolicyCollection policies);
}
