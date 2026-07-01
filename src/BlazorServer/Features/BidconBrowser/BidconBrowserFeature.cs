using BlazorServer.Features.BidconBrowser.Components;
using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Composition;
using BlazorServer.Shared.FeatureRegistration;

namespace BlazorServer.Features.BidconBrowser;

public class BidconBrowserFeature : IFeature
{
    public void AddAuthorizationPolicies(PolicyCollection policies)
    {

    }

    public void RegisterClaims(ClaimInfoCollection collection)
    {
        collection.AddAccessClaimInfo(new AccessClaimInfo 
        { 
            Name = "Bidconutforskaren", 
            Description = "Använda kalkylutforskaren", 
            Value = FeatureConstants.Authorization.AccessClaimValue
        });
    }

    public void RegisterRenderObjects(RenderCollection collection)
    {
        collection.AddTopNavigation(
            text: "Kalkyler",
            href: FeatureConstants.Routes.Base,
            order: 1);
    }

    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<EstimationListState>();
    }
}
