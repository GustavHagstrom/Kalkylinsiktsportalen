using BlazorServer.Shared.Abstractions;
using BlazorServer.Shared.Models;
using BlazorServer.Shared.Registration;

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
            Description = "Rättighet att bläddra bland klakyler i kalkylutforskaren", 
            Value = FeatureConstants.Authorization.AccessClaimValue
        });
    }

    public void RegisterRenderObjects(RenderCollection collection)
    {
        collection.AddAppBarNavMenuComponent<AppBarNavMenu>(1);
    }

    public void RegisterServices(IServiceCollection services)
    {

    }
}
