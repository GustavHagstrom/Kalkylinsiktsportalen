using BlazorServer.Shared.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace BlazorServer.Shared.Registration;

public class FeatureRegistrator
{
    private readonly WebApplicationBuilder builder;
    private readonly IServiceProvider serviceProvider;
    private readonly HashSet<IFeature> features = [];
    private readonly AppRegistrationContext registrationContext;
    

    public FeatureRegistrator(WebApplicationBuilder builder)
        : this(builder, new AppRegistrationContext())
    {
    }

    public FeatureRegistrator(WebApplicationBuilder builder, AppRegistrationContext registrationContext)
    {
        this.builder = builder;
        this.registrationContext = registrationContext;
        serviceProvider = builder.Services.BuildServiceProvider();
    }

    public RenderCollection RenderCollection => registrationContext.RenderCollection;

    public ClaimInfoCollection ClaimInfoCollection => registrationContext.ClaimInfoCollection;

    public PolicyCollection Policies => registrationContext.Policies;

    public void AddFeature<T>() where T : IFeature
    {
        var feature = ActivatorUtilities.CreateInstance<T>(serviceProvider);
        features.Add(feature);
        feature.RegisterServices(builder.Services);
        feature.RegisterRenderObjects(RenderCollection);
        feature.RegisterClaims(ClaimInfoCollection);
        feature.AddAuthorizationPolicies(Policies);
    }
    public void AddAuthorizationPolicy(string name, Action<AuthorizationPolicyBuilder> builder)
    {
        Policies.AddPolicy(name, builder);
    }

}
