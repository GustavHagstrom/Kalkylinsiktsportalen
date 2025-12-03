using BlazorServer.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Shared.Registration;

public class FeatureRegistrator
{
    private readonly WebApplicationBuilder _builder;
    private readonly IServiceProvider _sp;
    private HashSet<IFeature> _features = new();
    

    public FeatureRegistrator(WebApplicationBuilder builder)
    {
        _builder = builder;
        _sp = builder.Services.BuildServiceProvider();
    }
    public RenderCollection RenderCollection { get; } = new();
    public ClaimInfoCollection ClaimInfoCollection { get; } = new();
    public PolicyCollection Policies { get; } = new();

    public void AddFeature<T>() where T : IFeature
    {
        var feature = ActivatorUtilities.CreateInstance<T>(_sp);
        _features.Add(feature);
        feature.RegisterServices(_builder.Services);
        feature.RegisterRenderObjects(RenderCollection);
        feature.RegisterClaims(ClaimInfoCollection);
        feature.AddAuthorizationPolicies(Policies);
    }
    public void AddAuthorizationPolicy(string name, Action<AuthorizationPolicyBuilder> builder)
    {
        Policies.AddPolicy(name, builder);
    }

}
